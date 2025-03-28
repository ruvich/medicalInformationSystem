using System.Text.Json.Serialization;

namespace medicalApp.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PatientSorting
    {
        NameAsc,
        NameDesc,
        CreateAsc,
        CreateDesc,
        InspectionAsc,
        InspectionDesc
    }
}
