namespace medicalApp.Models
{
    public class PatientPagedListModel
    {
        public List<PatientShowModel>? patients { get; set; }
        public PageInfoModel pagination { get; set; }
    }
}
