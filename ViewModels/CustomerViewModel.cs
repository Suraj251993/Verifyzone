using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class CustomerViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("industry")]
        public int Industry { get; set; }
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
        [JsonPropertyName("commencementdate")]
        public string CommencementDate { get; set; }
        [JsonPropertyName("closeddate")]
        public string Closeddate { get; set; }
        [JsonPropertyName("gstnumber")]
        public string GstNumber { get; set; }
        [JsonPropertyName("tannumber")]
        public string TanNumber { get; set; }
        [JsonPropertyName("pannumber")]
        public string PanNumber { get; set; }
        [JsonPropertyName("iseducation")]
        public bool IsEducation { get; set; }
        [JsonPropertyName("isemployment")]
        public bool IsEmployment { get; set; }
        [JsonPropertyName("isbgv")]
        public bool IsBGV { get; set; }
    }
}
