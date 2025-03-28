using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class DoctorEditModel
    {
        [Required]
        [EmailAddress]
        [MinLength(1)]
        public string email { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string name { get; set; }
        
        public DateTime? birthday { get; set; }
        [Required]
        public Gender gender { get; set; }

        [MinLength(1)]
        [Phone]
        public string? phone { get; set; }
    }
}
