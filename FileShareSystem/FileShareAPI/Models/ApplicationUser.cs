using Microsoft.AspNetCore.Identity;

namespace FileShareAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<UserFile> Files { get; set; } = new List<UserFile>();
    }
}