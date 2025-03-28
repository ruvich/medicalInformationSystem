using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace medicalApp.Models
{
    public class DoctorModel
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
        [Required]
        [EmailAddress]
        [MinLength(1)]
        public string email { get; set; }
        [MinLength(1)]
        [Phone]
        public string phone { get; set; }
        [Required]
        [JsonIgnore]
        public string passwordHash { get; set; }
        [Required]
        public Guid specialityId { get; set; }
        public virtual SpecialityModel speciality { get; set; }
        public virtual ICollection<InspectionDataBaseModel> inspections { get; set; } = new List<InspectionDataBaseModel>();
        public virtual ICollection<CommentDataBaseModel> сomments { get; set; } = new List<CommentDataBaseModel>();
    }
}
