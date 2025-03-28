using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class InspectionCommentCreateModel
    {
        [Required]
        [MaxLength(1000)]
        [MinLength(1)]
        public string content { get; set; }
    }
}
