using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class CompanyViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("contactname")]
        public string Contactname { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("contactnumber")]
        public string Contactnumber { get; set; }
        [JsonPropertyName("charges")]
        public double Charges { get; set; }
        [JsonPropertyName("gstnumber")]
        public string GstNumber { get; set; }
        [JsonPropertyName("tannumber")]
        public string TanNumber { get; set; }
        [JsonPropertyName("pannumber")]
        public string PanNumber { get; set; }
    }
}
