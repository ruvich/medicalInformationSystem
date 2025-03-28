using System.Text.Json.Serialization;

namespace medicalApp.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DiagnosisType
    {
        Main,
        Concomitant, //сопутствующее
        Complication //осложнение
    }
}
