using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OrgCheck.Services;
using OrgCheck.Services.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OrgCheck.Middleware
{
    public class ExecutionContextMiddleware
    {        
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RequestDelegate nextRequest;
        private readonly LogService _logService;
        public ExecutionContextMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor, LogService logService)
        {
            nextRequest = next;
            _httpContextAccessor = httpContextAccessor;
            _logService = logService;
        }
        public async Task Invoke(HttpContext context, ExecutionContext executionContext, IAuthService _authService)
        {
            int _Id;
            try
            {                
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var customerId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
                var companyId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.GroupSid);
                int.TryParse(userId, out _Id);
                executionContext.UserId = _Id;
                executionContext.RoleId = 0; // yet to implement
                executionContext.RoleName = role;
                executionContext.CompanyId = Convert.ToInt32(companyId);
                executionContext.CustomerId = Convert.ToInt32(customerId);
                if (_Id > 0)
                    _authService.SetupUser(_Id);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
            }
            finally
            {
                await nextRequest(context);
            }
        }
    }
}
