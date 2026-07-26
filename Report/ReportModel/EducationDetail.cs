namespace OrgCheck.Report.ReportModel
{
    public class EducationDetail
    {
        public string StudentName { get; set; }
        public string StudentId { get; set;}
        public string InstituteName { get; set; }
        public string University { get; set; }
        public string DegreeName { get; set; }
        public string MajorSubject { get; set; }
        public string PeriodFrom { get; set; }
        public string PeriodTo { get; set; }
        public string PassYear { get; set; }
        public string MarksObtained { get; set; }
        public string StudyMode { get; set; }
        public string AttainDegree { get; set; }
        public string VerifierName { get; set; }
        public string VerifierDesignation { get; set; }
        public string VerifierEmail { get; set; }
        public string VerificationDate { get; set; }
    }
    public class NonEducationDetail
    {
        public string StudentId { get; set; }
        public string InstituteName { get; set; }
        public string ReplyComments { get; set; }
        public string VerifierName { get; set; }
        public string VerifierDesignation { get; set; }
        public string VerifierEmail { get; set; }
    }
}
