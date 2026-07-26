using System;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class StudentApprovalViewModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("studentid")]
        public string StudentId { get; set; }
        
        [JsonPropertyName("studentname")]
        public string StudentName { get; set; }

        [JsonPropertyName("regno")]
        public string RegNo { get; set; }

        [JsonPropertyName("degreetype")]
        public string DegreeType { get; set; }

        [JsonPropertyName("majorsubject")]
        public string MajorSubject { get; set; }

        [JsonPropertyName("requestedby")]
        public string RequestedBy { get; set; }

        [JsonPropertyName("requestedorganisation")]
        public string RequestedOrganisation { get; set; }

        [JsonPropertyName("requesteddate")]
        public string RequestedDate { get; set; }

        [JsonPropertyName("approveddate")]
        public string ApprovedDate { get; set;}
    }
}
