using System.Text.Json;
using System.Text.Json.Serialization;

namespace medicalApp.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        Male,
        Female
    }
}
