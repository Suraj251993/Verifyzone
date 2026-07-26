using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class AbscondDetailViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("customername")]
        public string Customername { get; set; }
        [JsonPropertyName("employeecode")]
        public string Employeecode { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("joindate")]
        public string Joindate { get; set; }
        [JsonPropertyName("lastworkingdate")]
        public string Lastworkingdate { get; set; }
        [JsonPropertyName("mobileno")]
        public string Mobileno { get; set; }
        [JsonPropertyName("linkedinurl")]
        public string Linkedinurl { get; set; }
        [JsonPropertyName("uannumber")]
        public string Uannumber { get; set; }
        [JsonPropertyName("fathername")]
        public string Fathername { get; set; }
        [JsonPropertyName("resume")]
        public IFormFile Resume { get; set; }
        [JsonPropertyName("resumename")]
        public string Resumename { get; set; }
        [JsonPropertyName("emailid")]
        public string Emailid { get; set; }
        [JsonPropertyName("discrepancytype")]
        public int DiscrepancyType { get; set; }
        [JsonPropertyName("employeeid")]
        public int Employeeid { get; set; }
        [JsonPropertyName("remarks")]
        public string Remarks { get; set; }
    }
}
