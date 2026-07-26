using System;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class FileViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("filename")]
        public string Filename { get; set; }
        [JsonPropertyName("filesize")]
        public int Filesize { get; set; }
        [JsonPropertyName("uploadedby")]
        public int Uploadedby { get; set; }
        [JsonPropertyName("uploadeddate")]
        public string Uploadeddate { get; set; }
        [JsonPropertyName("customerid")]
        public int Customerid { get; set; }
        [JsonPropertyName("customername")]
        public string CustomerName { get; set; }
        [JsonPropertyName("uploadedstatus")]
        public string UploadedStatus { get; set; }
        [JsonPropertyName("totalrecords")]
        public int TotalRecords { get; set; }
        [JsonPropertyName("validrecords")]
        public int ValidRecords { get; set; }
        [JsonPropertyName("invalidrecords")]
        public int InvalidRecords { get; set; }
    }
}
