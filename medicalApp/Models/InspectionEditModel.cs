using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class InspectionEditModel
    {
        [MaxLength(5000)]
        public string? anamnesis { get; set; }
        [Required]
        [MaxLength(5000)]
        public string complaints { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(5000)]
        public string treatment { get; set; }
        [Required]
        public Conclusion conclusion { get; set; }
        public DateTime? nextVisitDay { get; set; }
        public DateTime? deathDate { get; set; }
        public List<DiagnosisCreateModel> diagnosis { get; set; }

    }
}
