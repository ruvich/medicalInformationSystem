using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class PatientShowModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        [Required]
        public string name { get; set; }
        public DateTime? birthday { get; set; }
        [Required]
        public Gender gender { get; set; }
    }
}
