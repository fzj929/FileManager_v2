using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileShareAPI.Data;
using FileShareAPI.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FileShareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FilesController> _logger;
        private readonly IConfiguration _configuration;

        public FilesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment,
            ILogger<FilesController> logger,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserFiles()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var files = await _context.UserFiles
                    .Where(f => f.UserId == user.Id)
                    .OrderByDescending(f => f.CreatedAt)
                    .Select(f => new
                    {
                        f.Id,
                        f.FileName,
                        f.OriginalName,
                        f.FileSize,
                        f.ContentType,
                        f.IsShared,
                        f.SharedToken,
                        f.SharedExpiry,
                        f.CreatedAt
                    })
                    .ToListAsync();

                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user files");
                return StatusCode(500, new { message = "Error retrieving files" });
            }
        }

        [HttpPost("upload")]
        [RequestSizeLimit(524288000)] // 500MB
        [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "No file provided" });

                // Blacklist dangerous file extensions
                var dangerousExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    ".exe", ".bat", ".cmd", ".sh", ".ps1", ".com", ".pif", ".scr",
                    ".msi", ".dll", ".vbs", ".hta", ".wsf", ".jar", ".cpl"
                };
                var ext = Path.GetExtension(file.FileName);
                if (dangerousExtensions.Contains(ext))
                    return BadRequest(new { message = "File type not allowed for security reasons" });

                var maxFileSize = _configuration.GetValue<long>("FileStorage:MaxFileSize", 104857600);
                if (file.Length > maxFileSize)
                    return BadRequest(new { message = $"File size exceeds maximum allowed size of {maxFileSize / 1048576}MB" });

                var uploadPath = Path.Combine(_environment.ContentRootPath, _configuration["FileStorage:UploadPath"] ?? "Uploads");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Sanitize file name to prevent path traversal attacks
                var safeOriginalName = Path.GetFileName(file.FileName);
                safeOriginalName = string.Concat(safeOriginalName.Split(Path.GetInvalidFileNameChars()));
                if (string.IsNullOrWhiteSpace(safeOriginalName))
                    safeOriginalName = "file";

                var fileName = $"{Guid.NewGuid()}_{safeOriginalName}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var userFile = new UserFile
                {
                    FileName = fileName,
                    OriginalName = safeOriginalName,
                    FilePath = filePath,
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserFiles.Add(userFile);
                await _context.SaveChangesAsync();

                _logger.LogInformation("File uploaded: {FileName} by user {UserName}", file.FileName, user.UserName);

                return Ok(new
                {
                    userFile.Id,
                    userFile.FileName,
                    userFile.OriginalName,
                    userFile.FileSize,
                    userFile.ContentType,
                    userFile.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return StatusCode(500, new { message = "Error uploading file" });
            }
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var file = await _context.UserFiles
                    .FirstOrDefaultAsync(f => f.Id == id && f.UserId == user.Id);

                if (file == null)
                    return NotFound(new { message = "File not found" });

                if (!System.IO.File.Exists(file.FilePath))
                    return NotFound(new { message = "File not found on disk" });

                var fileStream = System.IO.File.OpenRead(file.FilePath);
                return File(fileStream, file.ContentType, file.OriginalName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file");
                return StatusCode(500, new { message = "Error downloading file" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var file = await _context.UserFiles
                    .FirstOrDefaultAsync(f => f.Id == id && f.UserId == user.Id);

                if (file == null)
                    return NotFound(new { message = "File not found" });

                if (System.IO.File.Exists(file.FilePath))
                    System.IO.File.Delete(file.FilePath);

                _context.UserFiles.Remove(file);
                await _context.SaveChangesAsync();

                _logger.LogInformation("File deleted: {FileName} by user {UserName}", file.OriginalName, user.UserName);

                return Ok(new { message = "File deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file");
                return StatusCode(500, new { message = "Error deleting file" });
            }
        }

        [HttpPost("{id}/share")]
        public async Task<IActionResult> ShareFile(int id, [FromBody] ShareFileRequest request)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var file = await _context.UserFiles
                    .FirstOrDefaultAsync(f => f.Id == id && f.UserId == user.Id);

                if (file == null)
                    return NotFound(new { message = "File not found" });

                file.IsShared = true;
                file.SharedToken = Guid.NewGuid().ToString();
                file.SharedExpiry = request.ExpiryDays > 0
                    ? DateTime.UtcNow.AddDays(request.ExpiryDays)
                    : null;
                file.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var shareUrl = $"{Request.Scheme}://{Request.Host}/api/files/shared/{file.SharedToken}";

                return Ok(new
                {
                    ShareUrl = shareUrl,
                    Token = file.SharedToken,
                    ExpiresAt = file.SharedExpiry
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sharing file");
                return StatusCode(500, new { message = "Error sharing file" });
            }
        }

        [HttpDelete("{id}/share")]
        public async Task<IActionResult> UnshareFile(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var file = await _context.UserFiles
                    .FirstOrDefaultAsync(f => f.Id == id && f.UserId == user.Id);

                if (file == null)
                    return NotFound(new { message = "File not found" });

                file.IsShared = false;
                file.SharedToken = null;
                file.SharedExpiry = null;
                file.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("File unshared: {FileName} by user {UserName}", file.OriginalName, user.UserName);

                return Ok(new { message = "File unshared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsharing file");
                return StatusCode(500, new { message = "Error unsharing file" });
            }
        }

        [HttpGet("shared/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSharedFile(string token)
        {
            try
            {
                var file = await _context.UserFiles
                    .FirstOrDefaultAsync(f => f.SharedToken == token && f.IsShared);

                if (file == null)
                    return NotFound(new { message = "Shared file not found" });

                if (file.SharedExpiry.HasValue && file.SharedExpiry.Value < DateTime.UtcNow)
                {
                    file.IsShared = false;
                    file.SharedToken = null;
                    file.SharedExpiry = null;
                    await _context.SaveChangesAsync();
                    return BadRequest(new { message = "Share link has expired" });
                }

                if (!System.IO.File.Exists(file.FilePath))
                    return NotFound(new { message = "File not found on disk" });

                var fileStream = System.IO.File.OpenRead(file.FilePath);
                return File(fileStream, file.ContentType, file.OriginalName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing shared file");
                return StatusCode(500, new { message = "Error accessing shared file" });
            }
        }

        [HttpGet("shared/{token}/info")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSharedFileInfo(string token)
        {
            try
            {
                var file = await _context.UserFiles
                    .Select(f => new
                    {
                        f.Id,
                        f.OriginalName,
                        f.FileSize,
                        f.ContentType,
                        f.SharedExpiry,
                        f.IsShared,
                        f.SharedToken
                    })
                    .FirstOrDefaultAsync(f => f.SharedToken == token && f.IsShared);

                if (file == null)
                    return NotFound(new { message = "Shared file not found" });

                if (file.SharedExpiry.HasValue && file.SharedExpiry.Value < DateTime.UtcNow)
                {
                    var expiredFile = await _context.UserFiles.FindAsync(file.Id);
                    if (expiredFile != null)
                    {
                        expiredFile.IsShared = false;
                        expiredFile.SharedToken = null;
                        expiredFile.SharedExpiry = null;
                        await _context.SaveChangesAsync();
                    }
                    return BadRequest(new { message = "Share link has expired" });
                }

                return Ok(file);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shared file info");
                return StatusCode(500, new { message = "Error getting shared file info" });
            }
        }
    }

    public class ShareFileRequest
    {
        public int ExpiryDays { get; set; } = 0;
    }
}