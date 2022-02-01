using System.ComponentModel.DataAnnotations;

namespace JWTAuthTest.Models
{
    public class AuthenticateRequest
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
