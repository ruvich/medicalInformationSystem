namespace medicalApp.Models
{
    public class InspectionChainModel
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public DateTime date { get; set; }
        public Conclusion conclusion { get; set; } 
        public Guid? previousInspectionId { get; set; }
        public Guid doctorId { get; set; }
        public string doctor { get; set; }
        public Guid patientId { get; set; }
        public string patient { get; set; }
        public bool hasChain { get; set; }
        public bool hasNested { get; set; }
        public List<DiagnosisModel> diagnosis { get; set; }
    }
}
