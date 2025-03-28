using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class DiagnosisCreateModel
    {
        [Required]
        public Guid icdDiagnosisId { get; set; }
        [MaxLength(5000)]
        [MinLength(1)]
        public string? description { get; set; }
        [Required]
        public DiagnosisType type { get; set; }
    }
}
