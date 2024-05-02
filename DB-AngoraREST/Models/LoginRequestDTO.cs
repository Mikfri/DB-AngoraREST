using System.ComponentModel.DataAnnotations;

namespace DB_AngoraREST.Models
{
    public class LoginRequestDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
