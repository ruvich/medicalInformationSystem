using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class SpecialityModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        [Required]
        [MinLength(1)]
        public string name { get; set; }
        public virtual ICollection<DoctorModel> doctors { get; set; } = new List<DoctorModel>();
        public virtual ICollection<ConsultationDataBaseModel> consultations { get; set; } = new List<ConsultationDataBaseModel>();
    }
}
