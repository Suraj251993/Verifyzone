using System;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class LoginViewModel
    {
        [JsonPropertyName("loginname")]
        public string LoginName { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [JsonPropertyName("rememberme")]
        public bool RememberMe { get; set; }
        public bool isEducation { get; set; }
        public bool isEmployment { get; set; }
    }
}
