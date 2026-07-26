using System;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class CompanyCreditViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("companyid")]
        public int Companyid { get; set; }
        [JsonPropertyName("companyname")]
        public string CompanyName { get; set; }
        [JsonPropertyName("credit")]
        public int Credit { get; set; }
        [JsonPropertyName("transactiontype")]
        public string Transactiontype { get; set; }
        [JsonPropertyName("referenceno")]
        public string Referenceno { get; set; }
        [JsonPropertyName("remarks")]
        public string Remarks { get; set; }
        [JsonPropertyName("creditdate")]
        public DateTime CreditDate { get; set; }
    }
}
