using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace medicalApp.Models
{
    public class PatientModel
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
        public virtual ICollection<InspectionDataBaseModel> inspections { get; set; } = new List<InspectionDataBaseModel>();
    }
}
