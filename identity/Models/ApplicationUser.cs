using Microsoft.AspNetCore.Identity;

namespace identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? City { get; set; }
        public string? Gender { get; set; }
    }
}
