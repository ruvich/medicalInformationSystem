using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class CommentDataBaseModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        public DateTime? modifyTime { get; set; }
        [Required]
        [MinLength(1)]
        public string content { get; set; }
        [Required]
        public Guid authorId { get; set; }
        public virtual DoctorModel author { get; set; }
        [Required]
        [MinLength(1)]
        public string authorName { get; set; }
        public Guid? parentId { get; set; }
        public Guid consultationId { get; set; }
        public virtual ConsultationDataBaseModel consultation { get; set; }
    }
}
