using System.Collections.Generic;

namespace OrgCheck.Report.ReportModel
{
    public class VerificationDetails
    {
        public string Employer { get; set; }
        public string CandidateName { get; set; }
        public string Desigination { get; set; }

        public string DateOfJoining { get; set; }
        public string ReportingManagerName { get; set; }
        public string ReasonforLeaving { get; set; }
        public string EmployeeCode { get; set; }
        public string Location { get; set; }
        public string DateOfLeaving { get; set; }
        public string ReportingManagerDesigination { get; set; }
        public string LastSalary { get; set; }
        public string Comments { get; set; }
        public string HRName { get; set; }
        public string HRDesigination { get; set; }
        public string HREmailId { get; set; }
        public string VerificationFacilatedBy { get; set; }
        public string ReportGeneratedOn { get; set; }   
        public string DateOfVerification { get; set; }
        public string Status { get; set; }
        public List<HRComments> HrComments { get; set; }

    }
    public class NonVerificationDetails
    {
        public string EmpCode { get; set; }
        public string CustomerName { get; set; }
        public string HRComments { get; set; }
        public string VerifierName { get; set; }
        public string VerifierDesignation { get; set; }
        public string VerifierEmail { get; set; }
    }
}
