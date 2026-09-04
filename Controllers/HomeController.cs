using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrgCheck.Models;
using OrgCheck.Services;
using OrgCheck.Services.Interfaces;
using OrgCheck.ViewModels;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OrgCheck.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly LogService _logService;
        private IHttpContextAccessor _accessor;
        
        public HomeController(ILogger<HomeController> logger, LogService logService, IServiceProvider serviceProvider, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _accessor = accessor;
            _logService = logService;
        }
        
        public IActionResult Index()
        {
            var model = new LoginViewModel()
            {
                LoginName = "",
                Password = ""
            };
            if (Request.Cookies["vzun"] != null)
            {
                model = new LoginViewModel()
                {
                    LoginName = Request.Cookies["vzun"],
                    Password = Request.Cookies["vzpd"],
                    RememberMe = true
                };                
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            try
            {
                model.isEducation = false;
                model.isEmployment = false;
                var result = _serviceProvider.GetRequiredService<IUserService>().GetUser(model);
                if (result.Id == 0)
                {
                    model.Password = "";
                    // Invalid user
                    ViewBag.error = "Invalid credentials !";
                    return View(model);
                }
                else
                {
                    var principal = _serviceProvider.GetRequiredService<IAuthService>().GetClaimsPrincipal(result.DisplayName, result.Id, result.UserTypename, result.CustomerId, result.CustomerType);
                    Thread.CurrentPrincipal = principal;
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    if (model.RememberMe)
                    {
                        CookieOptions option = new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(7.0),
                            HttpOnly = true,
                            //Secure = true,
                            IsEssential = true
                        };
                        Response.Cookies.Append("vzun", model.LoginName, option);
                        Response.Cookies.Append("vzpd", model.Password, option);
                    }
                    if (result.UserType == 1)
                    {
                        return RedirectToActionPermanent("Index", "Admin");
                    }
                    else if (result.UserType == 2)
                    {
                        return RedirectToActionPermanent("Landing", "Home");
                    }
                    else if (result.UserType == 4)
                    {
                        return RedirectToActionPermanent("Index", "Support");
                    }
                    ViewBag.error = "Unexpected error occured";
                    model.Password = "";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                model.Password = "";
                // Invalid user
                ViewBag.error = "Unexpected error occured. Please contact support !";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", "Home")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var model = new LoginViewModel() { LoginName = "", Password = "" };
            try
            {
                var externalResult = await HttpContext.AuthenticateAsync("ExternalGoogleCookie");
                await HttpContext.SignOutAsync("ExternalGoogleCookie");

                if (!externalResult.Succeeded || externalResult.Principal == null)
                {
                    ViewBag.error = "Google sign-in failed. Please try again.";
                    return View("Index", model);
                }

                var email = externalResult.Principal.FindFirstValue(ClaimTypes.Email);
                var googleId = externalResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var displayName = externalResult.Principal.FindFirstValue(ClaimTypes.Name);

                if (string.IsNullOrEmpty(email))
                {
                    ViewBag.error = "Your Google account does not have a verified email.";
                    return View("Index", model);
                }

                var result = _serviceProvider.GetRequiredService<IUserService>().GetOrCreateGoogleUser(googleId, email, displayName);
                if (result == null || result.Id == 0)
                {
                    ViewBag.error = "Unable to sign in with Google. Please contact support.";
                    return View("Index", model);
                }

                var principal = _serviceProvider.GetRequiredService<IAuthService>().GetClaimsPrincipal(result.DisplayName, result.Id, result.UserTypename, result.CustomerId, result.CustomerType);
                Thread.CurrentPrincipal = principal;
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (result.UserType == 1)
                {
                    return RedirectToActionPermanent("Index", "Admin");
                }
                else if (result.UserType == 2)
                {
                    return RedirectToActionPermanent("Landing", "Home");
                }
                else if (result.UserType == 4)
                {
                    return RedirectToActionPermanent("Index", "Support");
                }
                ViewBag.error = "Unexpected error occured";
                return View("Index", model);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                ViewBag.error = "Unexpected error occured. Please contact support !";
                return View("Index", model);
            }
        }

        public IActionResult Landing()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return Json(true);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult ForgotPassword(string emailId)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<IUserService>().ForgotPassword(emailId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(false);
            }
        }
    }
}
