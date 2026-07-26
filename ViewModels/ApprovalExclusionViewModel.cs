using System.Text.Json.Serialization;
namespace OrgCheck.ViewModels
{
    public class ApprovalExclusionViewModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("customerid")]
        public int customerId { get; set; }
        [JsonPropertyName("employeeid")]
        public int employeeId { get; set; }
        [JsonPropertyName("empcode")]
        public string empCode { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("excludedby")]
        public string excludedBy { get; set; }
        [JsonPropertyName("excludeddate")]
        public string excludedDate { get; set; }
    }
}
