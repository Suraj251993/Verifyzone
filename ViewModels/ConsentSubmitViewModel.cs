using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class ConsentSubmitViewModel
    {
        [JsonPropertyName("token")]
        [Required(ErrorMessage = "Token Required")]
        public string Token { get; set; }

        [JsonPropertyName("optionalemail")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Optionalemail { get; set; }

        [JsonPropertyName("consentaccepted")]
        public bool Consentaccepted { get; set; }

        [JsonPropertyName("documentviewed")]
        public bool Documentviewed { get; set; }
    }
}
