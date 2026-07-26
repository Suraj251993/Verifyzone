using System;

namespace OrgCheck.ViewModels
{
    public class QuestionaireMappingViewModel
    {
        public string Id { get; set; }
        public string QuestionId { get; set; }
        public string Question { get; set; }
        public string CompanyId { get; set; }
        public bool IsSelected { get; set; }
    }
}
