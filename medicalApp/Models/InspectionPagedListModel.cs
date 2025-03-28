namespace medicalApp.Models
{
    public class InspectionPagedListModel
    {
        public List<InspectionChainModel>? inspections { get; set; }
        public PageInfoModel pagination { get; set; }
    }
}
