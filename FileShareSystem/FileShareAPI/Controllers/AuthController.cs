using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FileShareAPI.Models;
using FileShareAPI.Services;

namespace FileShareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                    return BadRequest(new { message = "Email already registered" });

                var existingUsername = await _userManager.FindByNameAsync(model.Username);
                if (existingUsername != null)
                    return BadRequest(new { message = "Username already taken" });

                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User registered: {user.UserName}");
                    var token = _jwtService.GenerateToken(user.Id, user.UserName!, user.Email!);

                    return Ok(new AuthResponse
                    {
                        Token = token,
                        Username = user.UserName!,
                        Email = user.Email!,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(1440)
                    });
                }

                return BadRequest(new { message = "Registration failed", errors = result.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error");
                return StatusCode(500, new { message = "Registration failed" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                ApplicationUser? user = null;

                if (model.UsernameOrEmail.Contains('@'))
                {
                    user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
                }
                else
                {
                    user = await _userManager.FindByNameAsync(model.UsernameOrEmail);
                }

                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User logged in: {user.UserName}");
                    var token = _jwtService.GenerateToken(user.Id, user.UserName!, user.Email!);

                    return Ok(new AuthResponse
                    {
                        Token = token,
                        Username = user.UserName!,
                        Email = user.Email!,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(1440)
                    });
                }

                return Unauthorized(new { message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                return StatusCode(500, new { message = "Login failed" });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully" });
        }
    }
}