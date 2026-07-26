using System;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class EmployeeRequestViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("searchid")]
        public int Searchid { get; set; }
        [JsonPropertyName("empcode")]
        public string Empcode { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("customerid")]
        public int Customerid { get; set; }
        [JsonPropertyName("customername")]
        public string Customername { get; set; }
        [JsonPropertyName("requestcomments")]
        public string Requestcomments { get; set; }
        [JsonPropertyName("raisedby")]
        public int Raisedby { get; set; }
        [JsonPropertyName("raisedbyname")]
        public string RaisedByName { get; set; }
        [JsonPropertyName("raiseddate")]
        public string Raiseddate { get; set; }
        [JsonPropertyName("responsetype")]
        public string ResponseType { get; set; }
        [JsonPropertyName("hrcomments")]
        public string Hrcomments { get; set; }
        [JsonPropertyName("repliedby")]
        public int Repliedby { get; set; }
        [JsonPropertyName("repliername")]
        public string ReplierName { get; set; }
        [JsonPropertyName("replieddate")]
        public DateTime? Replieddate { get; set; }
        public string Reportname { get; set; }
    }
}
