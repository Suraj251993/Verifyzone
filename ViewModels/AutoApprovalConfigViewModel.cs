using System;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class AutoApprovalConfigViewModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("startdate")]
        public string startDate {  get; set; }
        [JsonPropertyName("enddate")]
        public string endDate { get; set; }
    }
}
