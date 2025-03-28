using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class ConsultationModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        public Guid inspectionId { get; set; }
        public SpecialityShowModel speciality { get; set; }
        [Required]
        public List<CommentModel>? comments { get; set; }
    }
}
