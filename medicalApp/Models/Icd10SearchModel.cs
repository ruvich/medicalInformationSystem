namespace medicalApp.Models
{
    public class Icd10SearchModel
    {
        public List<Icd10Model>? records { get; set; }
        public PageInfoModel pagination { get; set; }
    }
}
