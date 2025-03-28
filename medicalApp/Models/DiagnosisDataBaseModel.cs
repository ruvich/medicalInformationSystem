using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class DiagnosisDataBaseModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        [Required]
        [MinLength(1)]
        public string code { get; set; }
        [Required]
        [MinLength(1)]
        public string name { get; set; }
        public string? description { get; set; }
        [Required]
        public DiagnosisType type { get; set; }
        public Guid inspectionId { get; set; }
        public virtual InspectionDataBaseModel inspection { get; set; }
        public Guid icd10Id { get; set; }
        public virtual Icd10DataBaseModel icd10 { get; set; }
    }
}
