using System.ComponentModel.DataAnnotations;

namespace medicalApp.Models
{
    public class TokenResponseModel
    {
        [Required]
        public string Token { get; set; }
    }
}
