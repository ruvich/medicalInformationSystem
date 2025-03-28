﻿using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class InspectionShortModel
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public DateTime createTime { get; set; }
        [Required]
        public DateTime date { get; set; }
        [Required]
        public List<DiagnosisModel>? diagnosis { get; set; }
    }
}
