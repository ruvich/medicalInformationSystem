using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace medicalApp.Models
{
    public class DiagnosisModel
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
        [JsonIgnore]
        public Guid icd10Id { get; set; }
    }
}
