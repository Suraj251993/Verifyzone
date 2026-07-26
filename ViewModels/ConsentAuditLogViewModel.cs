using System;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class ConsentAuditLogViewModel
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }
        [JsonPropertyName("oldstatus")]
        public string Oldstatus { get; set; }
        [JsonPropertyName("newstatus")]
        public string Newstatus { get; set; }
        [JsonPropertyName("performedby")]
        public string Performedby { get; set; }
        [JsonPropertyName("ipaddress")]
        public string Ipaddress { get; set; }
        [JsonPropertyName("remarks")]
        public string Remarks { get; set; }
        [JsonPropertyName("createddate")]
        public DateTime Createddate { get; set; }
    }
}
