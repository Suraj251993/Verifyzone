using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class ConsentRequestViewModel
    {
        [JsonPropertyName("firstname")]
        [Required(ErrorMessage = "First Name Required")]
        public string Firstname { get; set; }

        [JsonPropertyName("lastname")]
        [Required(ErrorMessage = "Last Name Required")]
        public string Lastname { get; set; }

        [JsonPropertyName("employeecode")]
        public string Employeecode { get; set; }

        [JsonPropertyName("employeeemail")]
        [Required(ErrorMessage = "Employee Email Required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Employeeemail { get; set; }
    }
}
