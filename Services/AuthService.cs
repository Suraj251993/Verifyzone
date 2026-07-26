using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NPOI.POIFS.Crypt.Dsig;
using OrgCheck.DataAccess.Interfaces;
using OrgCheck.Models;
using OrgCheck.Services.Interfaces;
using OrgCheck.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;

namespace OrgCheck.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserViewModel CurrentUser { get; set; }
        private readonly PostgresContext _orgCheckContext;
        public AuthService(IHttpContextAccessor httpContextAccessor, PostgresContext orgCheckContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _orgCheckContext = orgCheckContext;
        }
        public ClaimsPrincipal GetClaimsPrincipal(string Uname, int UserId, string Role, int CustomerId, int CustomerType)
        {
            ClaimsIdentity identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, Uname),
                    new Claim(ClaimTypes.Role,Role),
                    new Claim(ClaimTypes.NameIdentifier,UserId.ToString()),
                    new Claim(ClaimTypes.Sid, CustomerId.ToString()),
                    new Claim(ClaimTypes.PrimarySid, CustomerType.ToString())
                }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            return principal;
            //var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
        public bool SetupUser(int Id)
        {
            var data = _orgCheckContext.Logins.Include(x => x.Customer).Include(x => x.Usertype).Include(x => x.Customertype)
                .AsNoTracking().Where(_ => _.Id == Id && _.Status > 0).FirstOrDefault();
            if (data != null)
            {
                CurrentUser = new UserViewModel()
                {
                    Id = data.Id,
                    LoginName = data.Loginname,
                    DisplayName = data.Displayname,
                    Emailid = data.Emailid,
                    UserTypename = data.Usertype.Name,
                    UserType = data.Usertypeid,
                    CustomerId = data.Customerid.HasValue ? data.Customerid.Value : 0,
                    CustomerType = data.Customertypeid.HasValue ? data.Customertypeid.Value : 0,
                    Customername = data.Customer?.Name ?? String.Empty,
                    Contactnumber = data.Contactnumber,
                    Designation = data.Designation,
                    Category = data.Category.GetValueOrDefault(),
                    IsEmployment = false,
                    IsEducation = false,
                    IsBGV = false
                };
                if (data.Usertypeid > 1)
                {
                    if (data.Customer.Isemployment.Value)
                        CurrentUser.IsEmployment = true;
                    if (data.Customer.Iseducation.Value)
                        CurrentUser.IsEducation = true;
                    if (data.Customer.Isbgv.Value)
                        CurrentUser.IsBGV = true;
                }
            }
            return true;
        }
        public int GetCurrentUserId()
        {
            int _Id;
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(userId, out _Id);
            return _Id;
        }
    }
}
