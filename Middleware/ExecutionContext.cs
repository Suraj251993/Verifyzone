using OrgCheck.ViewModels;

namespace OrgCheck.Middleware
{
    public class ExecutionContext
    {
        public UserViewModel CurrentUser { get; set; }
        public int UserId { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
    }
}
