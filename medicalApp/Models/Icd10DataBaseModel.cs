using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class Icd10DataBaseModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public Guid? parentId { get; set; }
        public virtual ICollection<DiagnosisDataBaseModel> diagnoses { get; set; } = new List<DiagnosisDataBaseModel>();
    }
}
