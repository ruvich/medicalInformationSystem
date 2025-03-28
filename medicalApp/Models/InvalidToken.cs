namespace medicalApp.Models
{
    public class InvalidToken
    {
        public Guid id { get; set; }
        public string token { get; set; }
        public DateTime finish {  get; set; }
    }
}
