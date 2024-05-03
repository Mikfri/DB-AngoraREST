using System.ComponentModel.DataAnnotations;

namespace DB_AngoraREST.DTOs
{
    public class RegisterRequestDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNum { get; set; }

        [Required]
        public string BreederRegNo { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string RoadNameAndNo { get; set; }

        [Required]
        public string City { get; set; }        

        [Required]
        public int ZipCode { get; set; }
      
        // Andre nødvendige felter...
    }
}
