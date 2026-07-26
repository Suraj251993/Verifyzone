using System;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class EmployeeApprovalViewModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("employeeid")]
        public string EmployeeId { get; set; }
        
        [JsonPropertyName("employeename")]
        public string EmployeeName { get; set; }

        [JsonPropertyName("empcode")]
        public string EmpCode { get; set; }

        [JsonPropertyName("requestedby")]
        public string RequestedBy { get; set; }

        [JsonPropertyName("requestedorganisation")]
        public string RequestedOrganisation { get; set; }

        [JsonPropertyName("requesteddate")]
        public string RequestedDate { get; set; }

        [JsonPropertyName("approveddate")]
        public string ApprovedDate { get; set;}

        [JsonPropertyName("isedit")]
        public string IsEdit { get; set; }
    }
}
