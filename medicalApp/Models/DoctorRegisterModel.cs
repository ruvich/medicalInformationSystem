using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class DoctorRegisterModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string name { get; set; }
        [Required]
        [MinLength(6)]
        public string password { get; set; }
        [Required]
        [EmailAddress]
        [MinLength(1)]
        public string email { get; set; }
        public DateTime? birthday { get; set; }
        [Required]
        public Gender gender { get; set; }
        
        [MinLength(1)]
        [Phone]
        public string? phone { get; set; }
        [Required]
        public Guid speciality { get; set; }
    }
}
