using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class InspectionDataBaseModel
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
        public Guid patientID { get; set; }
        public virtual PatientModel patient { get; set; }
        public Guid doctorID { get; set; }
        public virtual DoctorModel doctor { get; set; }
        public virtual ICollection<DiagnosisDataBaseModel> diagnoses { get; set; } = new List<DiagnosisDataBaseModel>();
        public virtual ICollection<ConsultationDataBaseModel> consultations { get; set; } = new List<ConsultationDataBaseModel>();
        public bool hasChain { get; set; } = false;
        public bool hasNested { get; set; } = false;
    }
}
