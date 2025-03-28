using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class InspectionCreateModel
    {
        [Required]
        public DateTime date { get; set; }
        /// <summary>
        /// Date and time of the inspection (UTC)
        /// </summary>

        [Required]
        [MaxLength(5000)]
        [MinLength(1)]
        public string anamnesis { get; set; }
        [Required]
        [MaxLength(5000)]
        [MinLength(1)]
        public string complaints { get; set; }
        [Required]
        [MaxLength(5000)]
        [MinLength(1)]
        public string treatment { get; set; }
        [Required]
        public Conclusion conclusion { get; set; }
        public DateTime? nextVisitDay { get; set; }
        /// <summary>
        /// Date and time of the next visit in case of Disease conclusion (UTC)
        /// </summary>

        public DateTime? deathDate { get; set; }
        /// <summary>
        /// Date and time of the death in case of Death conclusion (UTC)
        /// </summary>
        
        public Guid? previousInspectionId { get; set; }
        [Required]
        //[MinItems(1)] ???
        public List<DiagnosisCreateModel> diagnosis { get; set; }
        public List<ConsultationCreateModel>? consultations { get; set; }
    }
}
