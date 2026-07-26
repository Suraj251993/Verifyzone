using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using OrgCheck.Middleware;
using OrgCheck.Services;
using OrgCheck.Services.Interfaces;
using OrgCheck.ViewModels;
using System;
using System.Collections.Generic;

namespace OrgCheck.Controllers
{
    public class AdminController : Controller
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ExecutionContext _executionContext;
        private readonly LogService _logService;
        public AdminController(IServiceProvider serviceProvider, ExecutionContext executionContext, LogService logService)
        {
            _serviceProvider = serviceProvider;
            _executionContext = executionContext;
            _logService = logService;
        }
        [Authorize]
        public IActionResult Index()
        {
            ViewData["page"] = "ADMH";
            ViewBag.IsAdmin = true;
            return View();
        }        
        [HttpGet]
        public IActionResult GetDashboardData(string month, string year)
        {
            var report = _serviceProvider.GetRequiredService<IEmployeeService>().GetAdminDashboardData(Convert.ToInt32(month), Convert.ToInt32(year));
            return Json(report);
        }

        [HttpGet]
        public IActionResult vout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Json(true);
        }
        [Authorize]
        public IActionResult Customers()
        {
            ViewData["page"] = "ADMCU";
            ViewBag.IsAdmin = true;
            //ViewBag.YesNo = new List<SelectListItem>()
            //{
            //    new(){ Text="Yes", Value="Yes", Selected=false},
            //    new(){ Text="No", Value="No", Selected=false},
            //};
            return View(new CustomerViewModel());
        }
        public IActionResult GetCustomers()
        {
            var allCustomers = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomers(true, true);
            return Json(allCustomers);
        }
        [HttpGet]
        public IActionResult GetCustomer(int id)
        {
            var customer = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomer(id);
            return Json(customer);
        }
        [HttpPost]
        public IActionResult AddCustomer([FromBody] CustomerViewModel viewModel)
        {
            try
            {
                //var result = _serviceProvider.GetRequiredService<ICustomerService>().AddCustomer(viewModel);
                return Json("exists");
            }
            catch(Exception ex)
            {
                _logService.Log(ex);
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }            
        }
        [HttpPut]
        public IActionResult UpdateCustomer([FromBody] CustomerViewModel viewModel)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<ICustomerService>().UpdateCustomer(viewModel);
                return Ok(new
                {
                    success = true,
                    message = result
                });
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [Authorize]
        public IActionResult Questions()
        {
            ViewData["page"] = "ADMQ";
            ViewBag.IsAdmin = true;
            return View(new QuestionViewModel());
        }
        public IActionResult GetQuestions()
        {
            var allQuestions = _serviceProvider.GetRequiredService<IQuestionaireService>().GetQuestions();
            return Json(allQuestions);
        }
        [HttpGet]
        public IActionResult GetQuestion(int id)
        {
            var item = _serviceProvider.GetRequiredService<IQuestionaireService>().GetQuestion(id);
            return Json(item);
        }
        [HttpPost]
        public IActionResult AddQuestion([FromBody] QuestionViewModel viewModel)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<IQuestionaireService>().AddQuestion(viewModel);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpPut]
        public IActionResult UpdateQuestion([FromBody] QuestionViewModel viewModel)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<IQuestionaireService>().UpdateQuestion(viewModel);
                return Ok(new
                {
                    success = true,
                    message = result
                });
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [Authorize]        
        public IActionResult GetCompanies()
        {
            var allCompanies = _serviceProvider.GetRequiredService<ICompanyService>().GetCompanies();
            return Json(allCompanies);
        }
        public IActionResult GetCompanyQuestions(int companyId)
        {
            var list = _serviceProvider.GetRequiredService<IQuestionaireService>().GetQuestionaireMappingByCompany(companyId);
            return PartialView("_QuestionCompanyMapper", list);
        }
        [Authorize]
        public IActionResult GetQuestionsByCompany(int companyId)
        {
            var list = _serviceProvider.GetRequiredService<IQuestionaireService>().GetQuestionaireMappingByCompany(companyId);
            return Json(list);
        }        
        [HttpPost]
        public IActionResult AddCompanyQuestions([FromBody] List<QuestionaireMappingViewModel> listModel)
        {
            try
            {
                bool result = _serviceProvider.GetRequiredService<IQuestionaireService>().AddQuestionareMapping(listModel);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(false);
            }
        }

        [Authorize]
        public IActionResult Users()
        {
            ViewData["page"] = "ADMU";
            ViewBag.IsAdmin = true;
            //ViewBag.UserTypes = _serviceProvider.GetRequiredService<IUserService>().GetUserTypes();
            //ViewBag.CustomerTypes = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomerTypes();
            //ViewBag.SearchUserTypes = ViewBag.UserTypes;
            //ViewBag.Companies = _serviceProvider.GetRequiredService<ICompanyService>().GetCompanies()
            //    .ConvertAll(a =>
            //    {
            //        return new SelectListItem()
            //        {
            //            Text = a.Name,
            //            Value = a.Id.ToString(),
            //            Selected = false
            //        };
            //    });
            //ViewBag.Customers = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomers(true, true)
            //    .ConvertAll(a =>
            //    {
            //        return new SelectListItem()
            //        {
            //            Text = a.Name,
            //            Value = a.Id.ToString(),
            //            Selected = false
            //        };
            //    });
            return View(new UserViewModel());
        }
        [HttpPost]
        public IActionResult AddUser([FromBody] UserViewModel viewModel)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<IUserService>().AddUser(viewModel);
                return Ok(new
                {
                    success = true,
                    message = result
                });
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error occurred while saving the records"
                });
            }
        }
        [HttpPut]
        public IActionResult UpdateUser([FromBody] UserViewModel viewModel)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<IUserService>().UpdateUser(viewModel);
                return Ok(new
                {
                    success = true,
                    message = result
                });
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error occurred while saving the records"
                });
            }
        }
        [HttpGet]
        public IActionResult GetUsers(int usertypeId)
        {
            var allUsers = _serviceProvider.GetRequiredService<IUserService>().GetAllUsers(usertypeId);
            return Json(allUsers);
        }
        [HttpGet]
        public IActionResult GetUser(int id)
        {
            var user = _serviceProvider.GetRequiredService<IUserService>().GetUser(id);
            return Json(user);
        }
        [HttpGet]
        public IActionResult ResetUser(string id)
        {
            try
            {
                _serviceProvider.GetRequiredService<IUserService>().ResetPassword(Convert.ToInt32(id));
                return Json(true);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(false);
            }
        }

        public IActionResult GetChangePassword()
        {
            return PartialView("_ChangePassword");
        }
        
        [Authorize]
        public IActionResult CustomerCredit()
        {
            ViewData["page"] = "ADMCUCR";
            ViewBag.IsAdmin = true;
            ViewBag.Customers = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomers(true, true)
                .ConvertAll(a =>
                {
                    return new SelectListItem()
                    {
                        Text = a.Name,
                        Value = a.Id.ToString(),
                        Selected = false
                    };
                });
            ViewBag.SearchCustomers = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomers(true, true)
                .ConvertAll(a =>
                {
                    return new SelectListItem()
                    {
                        Text = a.Name,
                        Value = a.Id.ToString(),
                        Selected = false
                    };
                });
            return View(new CustomerCreditViewModel());
        }
        [HttpGet]
        public IActionResult GetCustomerCredits(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                customerId = "0";
            var result = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomerCredits(Convert.ToInt32(customerId));
            return Json(result);
        }
        [HttpPost]
        public IActionResult AddCustomerCredit([FromBody] CustomerCreditViewModel viewModel)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<ICustomerService>().AddCustomerCredit(viewModel);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(false);
            }
        }
        
        [HttpPost]
        public IActionResult ChangePassword([FromBody] ChangePassword viewModel)
        {
            try
            {
                var isvalid = _serviceProvider.GetRequiredService<IUserService>().CheckOldPassword(_executionContext.UserId, viewModel.OldPassword, 1);
                if (!isvalid) return Json(false);
                var update = _serviceProvider.GetRequiredService<IUserService>().UpdatePassword(_executionContext.UserId, viewModel.NewPassword);
                return Json(update);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(false);
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {                
                var result = _serviceProvider.GetRequiredService<IUserService>().GetUserProfile(_executionContext.UserId);
                return Json(result);
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult UpdateProfile([FromBody] UserProfileViewModel viewModel)
        {
            var result = _serviceProvider.GetRequiredService<IUserService>().UpdateUserProfile(viewModel);
            return Json(result);
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Json(true);
        }
    }
}
