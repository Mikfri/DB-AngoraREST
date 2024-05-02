using System.ComponentModel.DataAnnotations;

namespace DB_AngoraREST.Models
{
    public class RegisterRequestDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string City { get; set; }

        // Andre nødvendige felter...
    }
}
