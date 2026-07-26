using System;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class CustomerCreditViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("customerid")]
        public int Customerid { get; set; }
        [JsonPropertyName("customername")]
        public string CustomerName { get; set; }
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
