using System;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class GeneratedReportViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("verificationtype")]
        public string VerificationType { get; set; }
        [JsonPropertyName("searchrequestid")]
        public string Searchrequestid { get; set; }
        [JsonPropertyName("customerid")]
        public int Customerid { get; set; }
        [JsonPropertyName("customername")]
        public string CustomerName { get; set; }
        [JsonPropertyName("employeecode")]
        public string Employeecode { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("reportlink")]
        public string Reportlink { get; set; }
        [JsonPropertyName("reportdate")]
        public DateTime? Reportdate { get; set; }
        [JsonPropertyName("searchresult")]
        public string Searchresult { get; set; }
        [JsonPropertyName("transactionamount")]
        public double Transactionamount { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("createdby")]
        public int Createdby { get; set; }
        [JsonPropertyName("createdbyname")]
        public string CreatedbyName { get; set; }
        [JsonPropertyName("createddate")]
        public string Createddate { get; set; }
        public string Finalresult { get; set; }
        [JsonPropertyName("approveddate")]
        public string ApprovedDate { get; set; }
    }
}
