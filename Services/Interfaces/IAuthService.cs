using OrgCheck.ViewModels;
using System;
using System.Security.Claims;

namespace OrgCheck.Services.Interfaces
{
    public interface IAuthService
    {
        public ClaimsPrincipal GetClaimsPrincipal(string Uname, int UserId, string Role, int CustomerId, int CustomerType);
        public int GetCurrentUserId();
        public bool SetupUser(int userId);
        public UserViewModel CurrentUser { get; set; }
    }
}
