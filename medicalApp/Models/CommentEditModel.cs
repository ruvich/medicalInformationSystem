using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class CommentEditModel
    {
        [Required]
        [MaxLength(5000)]
        [MinLength(1)]
        public string content { get; set; }
    }
}
