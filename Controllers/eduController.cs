using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrgCheck.Services.Interfaces;
using OrgCheck.Services;
using OrgCheck.ViewModels;
using System.Threading;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace OrgCheck.Controllers
{
    public class eduController : Controller
    {        
        public eduController()
        {
        }
        [IgnoreAntiforgeryToken]
        public IActionResult Index()
        {
            return RedirectToActionPermanent("Index", "Home");
        }        
    }
}
