using System.Collections.Generic;

namespace OrgCheck.ViewModels
{
    public class HRDashboardViewModel
    {
        public int requestcount { get; set; }
        public List<int> approvalcount { get; set; }
        public List<int> reportcount { get; set; }
    }
}
