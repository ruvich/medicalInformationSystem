using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class LoginCredentialsModel
    {
        [Required]
        [EmailAddress]
        [MinLength(1)]
        public string email { get; set; }

        [Required]
        [MinLength(1)]
        public string password { get; set; }

    }
}
