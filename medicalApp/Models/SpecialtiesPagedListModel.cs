namespace medicalApp.Models
{
    public class SpecialtiesPagedListModel
    {
        public List<SpecialityShowModel>? specialities { get; set; }
        public PageInfoModel pagination { get; set; }
    }
}
