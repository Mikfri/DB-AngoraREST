using System.ComponentModel.DataAnnotations;

namespace DB_AngoraREST.DTOs
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
