using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OrgCheck.Services;
using OrgCheck.Services.Interfaces;
using OrgCheck.ViewModels;
using System;

namespace OrgCheck.Controllers
{
    public class ConsentController : Controller
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LogService _logService;
        public ConsentController(IServiceProvider serviceProvider, LogService logService)
        {
            _serviceProvider = serviceProvider;
            _logService = logService;
        }

        [HttpGet]
        public IActionResult Index(string token)
        {
            var result = _serviceProvider.GetRequiredService<IConsentService>().ValidateToken(token);
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit([FromBody] ConsentSubmitViewModel model)
        {
            try
            {
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                string userAgent = Request.Headers["User-Agent"].ToString();
                bool success = _serviceProvider.GetRequiredService<IConsentService>().SubmitConsent(model, ipAddress, userAgent, out string message);
                return Json(new { success, message });
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(new { success = false, message = "Unexpected error occured. Please contact support !" });
            }
        }
    }
}
