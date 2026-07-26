using System.Text.Json.Serialization;
using System;

namespace OrgCheck.ViewModels
{
    public class StudentRequestViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("searchid")]
        public int Searchid { get; set; }
        [JsonPropertyName("regno")]
        public string Regno { get; set; }
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
        public DateTime Raiseddate { get; set; }
        [JsonPropertyName("responsetype")]
        public string ResponseType { get; set; }
        [JsonPropertyName("replycomments")]
        public string Replycomments { get; set; }
        [JsonPropertyName("repliedby")]
        public int Repliedby { get; set; }
        [JsonPropertyName("repliername")]
        public string ReplierName { get; set; }
        [JsonPropertyName("replieddate")]
        public DateTime? Replieddate { get; set; }
    }
}
