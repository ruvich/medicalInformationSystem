using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class InspectionConsultationModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        public Guid inspectionId { get; set; }
        public SpecialityShowModel speciality { get; set; }
        public InspectionCommentModel? rootComment { get; set; }
        public int commentsNumber { get; set; }
    }
}
