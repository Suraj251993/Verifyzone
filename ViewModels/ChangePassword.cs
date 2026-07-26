using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class ChangePassword
    {
        [JsonPropertyName("oldpassword")]
        [Required(ErrorMessage = "OldPassword Required")]
        public string OldPassword { get; set; }
        [JsonPropertyName("newpassword")]
        [Required(ErrorMessage = "NewPassword Required")]
        public string NewPassword { get; set; }
        [JsonPropertyName("confirmpassword")]
        [Required(ErrorMessage = "ConfirmPassword Required")]
        public string ConfirmPassword { get; set; }
    }
}
