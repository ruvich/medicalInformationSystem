using System.Text.Json.Serialization;

namespace medicalApp.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Conclusion
    {
        Disease,
        Recovery,
        Death
    }
}
