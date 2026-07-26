using System;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class ConsentListViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("consentrequestid")]
        public string Consentrequestid { get; set; }
        [JsonPropertyName("employeename")]
        public string Employeename { get; set; }
        [JsonPropertyName("employeecode")]
        public string Employeecode { get; set; }
        [JsonPropertyName("employeeemail")]
        public string Employeeemail { get; set; }
        [JsonPropertyName("requestdate")]
        public DateTime Requestdate { get; set; }
        [JsonPropertyName("statusid")]
        public int Statusid { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("consentdate")]
        public DateTime? Consentdate { get; set; }
        [JsonPropertyName("lastupdated")]
        public DateTime? Lastupdated { get; set; }
    }
}
