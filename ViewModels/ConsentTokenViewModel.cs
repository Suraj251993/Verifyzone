using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class ConsentTokenViewModel
    {
        [JsonPropertyName("valid")]
        public bool Valid { get; set; }
        [JsonPropertyName("expired")]
        public bool Expired { get; set; }
        [JsonPropertyName("consumed")]
        public bool Consumed { get; set; }
        [JsonPropertyName("employeename")]
        public string Employeename { get; set; }
        [JsonPropertyName("employeeemail")]
        public string Employeeemail { get; set; }
        [JsonPropertyName("companyname")]
        public string Companyname { get; set; }
        [JsonPropertyName("hrcontact")]
        public string Hrcontact { get; set; }
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
