using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrgCheck.Services.Interfaces;
using OrgCheck.Services;
using OrgCheck.ViewModels;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrgCheck.Controllers
{
    public class SupportController : Controller
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly Middleware.ExecutionContext _executionContext;
        private readonly LogService _logService;
        public SupportController(IServiceProvider serviceProvider, LogService logService, IWebHostEnvironment appEnvironment, Middleware.ExecutionContext executionContext)
        {
            _serviceProvider = serviceProvider;
            _logService = logService;
            _appEnvironment = appEnvironment;
            _executionContext = executionContext;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult EmployeeUpload()
        {
            var users = _serviceProvider.GetRequiredService<IUserService>().GetAllUsers(2);
            var userlist = users.Select(_ => new SelectListItem()
            {
                Text = _.DisplayName + " (" + _.Customername + ")",
                Value = _.Id.ToString(),
                Selected = false
            }).ToList();
            ViewBag.Users = userlist;
            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult EmployeeUpload(UploadViewModel collection)
        {
            IFormFile file = collection.file;
            if (file != null)
            {
                Random r = new Random();
                int randNum = r.Next(100000);
                string sixDigitNumber = randNum.ToString("D6");
                var viewmodel = new UploadSummaryViewModel();
                var result = _serviceProvider.GetRequiredService<IEmployeeService>().Validate(file, sixDigitNumber);
                if (result == "")
                {
                    var userdata = _serviceProvider.GetRequiredService<IUserService>().GetUser(Convert.ToInt32(collection.user));
                    viewmodel = _serviceProvider.GetRequiredService<IEmployeeService>().ParseFile(file, sixDigitNumber, userdata.CustomerId, Convert.ToInt32(collection.user));
                }
                //return PartialView("_UploadSummary", viewmodel);
                return Json(viewmodel);
            }
            else
                return Json("");
        }
        [Authorize]
        public IActionResult GetLatestUploads(int user)
        {
            var result = _serviceProvider.GetRequiredService<IFileService>().GetUploadedFiles(user);
            return Json(result);
        }
        [Authorize]
        public FileResult DownloadTemplate()
        {
            //get file from respective table
            string reportpath = $"{_appEnvironment.WebRootPath + @"\Files\Template.csv"}";

            byte[] fileBytes = System.IO.File.ReadAllBytes(reportpath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template.csv");

        }
        [Authorize]
        [HttpPost]
        public IActionResult ApproveorRejectFile([FromBody] FileApprove model, string user)
        {
            bool result = false;
            try
            {
                if (model.Mode == "Approve")
                    result = _serviceProvider.GetRequiredService<IEmployeeService>().ApproveFile(model.FileId, Convert.ToInt32(user));
                else
                    result = _serviceProvider.GetRequiredService<IEmployeeService>().RejectFile(model.FileId);

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
            ViewBag.SearchUserTypes = _serviceProvider.GetRequiredService<IUserService>().GetUserTypes();
            return View();
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetUsers(int usertypeId)
        {
            var allUsers = _serviceProvider.GetRequiredService<IUserService>().GetAllUsers(usertypeId);
            return Json(allUsers);
        }
        [Authorize]
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
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Json(true);
        }
        public IActionResult GetChangePassword()
        {
            return PartialView("_ChangePassword");
        }
        [HttpPost]
        public IActionResult ChangePassword([FromBody] ChangePassword viewModel)
        {
            try
            {
                var isvalid = _serviceProvider.GetRequiredService<IUserService>().CheckOldPassword(_executionContext.UserId, viewModel.OldPassword, 4);
                if (!isvalid)
                    return Json(false);
                var update = _serviceProvider.GetRequiredService<IUserService>().UpdatePassword(_executionContext.UserId, viewModel.NewPassword);
                return Json(update);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(false);
            }
        }
    }
}
