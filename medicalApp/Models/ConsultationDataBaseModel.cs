using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class ConsultationDataBaseModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        public Guid inspectionId { get; set; }
        public Guid specialityId { get; set; }
        public virtual InspectionDataBaseModel inspection { get; set; }

        public virtual SpecialityModel speciality { get; set; }
        public virtual ICollection<CommentDataBaseModel> comments { get; set; } = new List<CommentDataBaseModel>();
    }
}

