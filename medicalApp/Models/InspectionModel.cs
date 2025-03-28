using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class InspectionModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        public DateTime date { get; set; }
        public string? anamnesis { get; set; }
        public string? complaints { get; set; }
        public string? treatment { get; set; }
        public Conclusion conclusion { get; set; }
        public DateTime? nextVisitDate { get; set; }
        public DateTime? deathDate { get; set; }
        public Guid? baseInspectionId { get; set; }
        public Guid? previousInspectionId { get; set; }
        public bool hasChain { get; set; }
        public bool hasNested { get; set; }
        public PatientShowModel patient { get; set; }
        public DoctorProfileModel doctor { get; set; }
        public List<DiagnosisModel>? diagnosis { get; set; }
        public List<InspectionConsultationModel>? consultations { get; set; }
    }
}
