using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class SpecialityShowModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        [MinLength(1)]
        public string name { get; set; }
    }
}
