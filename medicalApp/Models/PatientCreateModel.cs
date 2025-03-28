using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class PatientCreateModel
    {
        [Required]
        [MaxLength(1000)]
        [MinLength(1)]
        public string name { get; set; }
        public DateTime? birthday { get; set; }
        [Required]
        public Gender gender { get; set; }
    }
}
