using Amazon.S3.Model.Internal.MarshallTransformations;
using CsvHelper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using OrgCheck.Services;
using OrgCheck.Services.Interfaces;
using OrgCheck.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace OrgCheck.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAuthService _authService;
        private readonly Middleware.ExecutionContext _executionContext;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly LogService _logService;
        private readonly Constants _constants;
        public CustomerController(IServiceProvider serviceProvider, Middleware.ExecutionContext executionContext,
            IWebHostEnvironment appEnvironment, IHttpContextAccessor contextAccessor, LogService logService, Constants constants, IAuthService authService)
        {
            _serviceProvider = serviceProvider;
            _executionContext = executionContext;
            _appEnvironment = appEnvironment;
            _contextAccessor = contextAccessor;
            _logService = logService;
            _constants = constants;
            _authService = authService;
        }
        [Authorize]
        public IActionResult Index()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "HomeDashboard";
                ViewBag.IsBGV = false;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                ViewBag.Category = _authService.CurrentUser.Category;
                var countmodel = _serviceProvider.GetRequiredService<IEmployeeService>().GetDashboardCount(DateTime.Now.Month, DateTime.Now.Year, _executionContext.UserId);
                countmodel.Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month);
                countmodel.Year = DateTime.Now.Year;
                return View(countmodel);
            }
        }

        [Authorize]
        public IActionResult EIndex()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "HomeEdu";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.IsEducation = _authService.CurrentUser.IsEducation;
                ViewBag.IsEmployment = _authService.CurrentUser.IsEmployment;
                var countmodel = _serviceProvider.GetRequiredService<IStudentService>().GetDashboardCount(DateTime.Now.Month, DateTime.Now.Year);
                countmodel.BalanceCount = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomerBalance(_executionContext.CustomerId);
                return View(countmodel);
            }
        }

        [Authorize]
        public IActionResult VIndex()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "HomeVDashboard";
                ViewBag.IsBGV = false;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                var countmodel = _serviceProvider.GetRequiredService<IEmployeeService>().GetDashboardCount(DateTime.Now.Month, DateTime.Now.Year, _executionContext.UserId);
                countmodel.Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month);
                countmodel.Year = DateTime.Now.Year;
                countmodel.BalanceCount = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomerBalance(_executionContext.CustomerId);
                return View(countmodel);
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetDashboardData(string year)
        {
            var data = _serviceProvider.GetRequiredService<IEmployeeService>().GetDashboardData(Convert.ToInt32(year), _executionContext.CustomerId);
            return Json(data);
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

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Json(true);
        }
        [Authorize]
        public IActionResult EmployeeForm()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "EEVForm";
                ViewBag.IsBGV = false;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                ViewBag.Category = _authService.CurrentUser.Category;
                var allQuestions = _serviceProvider.GetRequiredService<IQuestionaireService>().GetQuestions();
                EmployeeViewModel data = new EmployeeViewModel()
                {
                    Mode = "Add"
                };
                data.EmployeeQuestions = allQuestions.Select(x => new EmployeeQuestionaireViewModel
                {
                    QuestionId = x.Value,
                    Questionname = x.Text,
                    Answer = string.Empty
                }).ToList();
                return View(data);
            }
        }
        [Authorize]
        public IActionResult ExRequest()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "emp");
            else
            {
                ViewData["page"] = "VEVSR";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.IsEducation = _authService.CurrentUser.IsEducation;
                ViewBag.IsEmployment = _authService.CurrentUser.IsEmployment;
                if (_authService.CurrentUser.CustomerType == 1 || _authService.CurrentUser.CustomerType == 3)
                    ViewBag.ExZone = true;
                else
                    ViewBag.ExZone = false;
                if (_authService.CurrentUser.CustomerType == 2 || _authService.CurrentUser.CustomerType == 3)
                    ViewBag.VZone = true;
                else
                    ViewBag.VZone = false;
                var allCustomers = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomers(true, true);
                var customers = allCustomers.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
                customers.Insert(0, new SelectListItem() { Value = "0", Text = "-- Please choose --" });
                ViewBag.Customers = customers;
                
                var allQuestions = _serviceProvider.GetRequiredService<IQuestionaireService>().GetQuestions();
                RequestViewModel data = new RequestViewModel();
                data.EmployeeQuestions = allQuestions.Select(x => new EmployeeQuestionaireViewModel
                {
                    QuestionId = x.Value,
                    Questionname = x.Text,
                    Answer = string.Empty
                }).ToList();
                ViewBag.AllQuestions = allQuestions;
                return View(data);
            }
        }
        [Authorize]
        public IActionResult ExRequestStatus()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "RPSRS";
                ViewBag.IsBGV = false;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                return View();
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GenerateReportByRequestId(int id)
        {
            try
            {
                var isGenerated = _serviceProvider.GetRequiredService<ICompanyService>().GenerateReportByRequestId(id);
                return Json(isGenerated);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(false);
            }
        }

        public FileResult DownloadRequestReportFile(int id)
        {
            var searchRecord = _serviceProvider.GetRequiredService<IEmployeeService>().GetEmployeeVerificationRequest(id);
            //get file from respective table
            string reportpath = $"{_constants.Reports}{searchRecord.Reportname}";

            byte[] fileBytes = System.IO.File.ReadAllBytes(reportpath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, searchRecord.Reportname);

        }

        [Authorize]
        [HttpGet]
        public IActionResult GetVerificationRequestStatus(string requestnumber, string finalresult)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<IEmployeeService>().GetEmployeeVerificationRequests(finalresult, requestnumber, _executionContext.CustomerId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(null);
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult SendExRequest([FromBody] RequestViewModel model)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<IEmployeeService>().AddEmpVerificationRequest(model, _authService.GetCurrentUserId());
                return Json("true");
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(ex.Message);
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult SaveEmployeeForm([FromBody] EmployeeViewModel model)
        {
            model.IsEdit = false;
            try
            {
                model.Customerid = _executionContext.CustomerId;
                var result = _serviceProvider.GetRequiredService<IEmployeeService>().AddEmployee(model, "Manual entry");
                var emp = _serviceProvider.GetRequiredService<IEmployeeService>().ViewEmployee(model.Employeecode, _executionContext.CustomerId, string.Empty, false);
                if (model.Emailid != "" || model.Mobileno != "" || model.Uannumber != "")
                {
                    var viewModel = new AbscondDetailViewModel()
                    {
                        Emailid = model.Emailid,
                        Mobileno = model.Mobileno,
                        Uannumber = model.Uannumber,
                        Fathername = model.Fathername,
                        Linkedinurl = model.Linkedinurl,
                        Employeeid = Convert.ToInt32(emp.Id),
                        Resume = model.Resume,
                    };
                    result = _serviceProvider.GetRequiredService<IEmployeeService>().AddAbscondDetail(viewModel).ToString();
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json("Unexpected error occured. Please contact support.");
            }
        }
        [Authorize]
        public IActionResult EmployeeApproval()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "EEARVerify";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                ViewBag.Category = _authService.CurrentUser.Category;
                var result = _serviceProvider.GetRequiredService<IEmployeeService>().GetEmployeeApprovals(_executionContext.CustomerId);
                ViewBag.Results = result;
                var data = new EmployeeViewModel();
                var allQuestions = _serviceProvider.GetRequiredService<IQuestionaireService>().GetQuestions();                
                data.EmployeeQuestions = allQuestions.Select(x => new EmployeeQuestionaireViewModel
                {
                    QuestionId = x.Value,
                    Questionname = x.Text,
                    Answer = string.Empty
                }).ToList();
                return View(data);
            }
        }
        [HttpPost]
        public IActionResult EmployeeApproval(List<int> Ids)
        {
            try
            {
                foreach (var id in Ids)
                {
                    _serviceProvider.GetRequiredService<IEmployeeService>().UpdateEmployeeApproval(id, _executionContext.UserId);
                }

                return Json("True");
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json("Error: " + ex.Message);
            }
        }
        [Authorize]
        public IActionResult SearchEmployee()
        {
            ViewData["page"] = "EEWEmp";
            ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
            ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
            ViewBag.Category = _authService.CurrentUser.Category;
            var result = new EmployeeViewModel();
            result.EmployeeQuestions = _serviceProvider.GetRequiredService<IQuestionaireService>().GetQuestions()
                .Select(_ => new EmployeeQuestionaireViewModel()
                {
                    QuestionId = _.Value,
                    Questionname = _.Text
                }).ToList();
            return View(result);
        }
        [Authorize]
        public IActionResult ViewEmployee()
        {
            ViewData["page"] = "EEWEmployees";
            ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
            ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
            ViewBag.Category = _authService.CurrentUser.Category;
            return View();
        }
        [Authorize]
        public IActionResult EmployeeUpload()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "EEVUpload";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                ViewBag.Category = _authService.CurrentUser.Category;
                UploadSummaryViewModel _view = new UploadSummaryViewModel();
                return View(_view);
            }
        }
        [Authorize]
        [HttpPost]
        public JsonResult EmployeeUpload(IFormFile file)
        {
            if (file != null)
            {
                Random r = new Random();
                int randNum = r.Next(100000);
                string sixDigitNumber = randNum.ToString("D6");
                var viewmodel = new UploadSummaryViewModel();
                var result = _serviceProvider.GetRequiredService<IEmployeeService>().Validate(file, sixDigitNumber);
                if (result == "")
                {
                    viewmodel = _serviceProvider.GetRequiredService<IEmployeeService>().ParseFile(file, sixDigitNumber, _executionContext.CustomerId, _executionContext.UserId);
                }

                return Json(viewmodel);
            }
            else
                return Json("");
        }
        [Authorize]
        public IActionResult GetEmployee(string Empcode, string Mode)
        {
            var result = new EmployeeViewModel();
            if (Mode == "AddEx" || Mode == "AddNew")
            {
                result.EmployeeQuestions = _serviceProvider.GetRequiredService<IQuestionaireService>().GetQuestions()
                .Select(_ => new EmployeeQuestionaireViewModel()
                {
                    QuestionId = _.Value,
                    Questionname = _.Text
                }).ToList();
            }
            else
            {
                var customerId = _executionContext.CustomerId;
                result = _serviceProvider.GetRequiredService<IEmployeeService>().ViewEmployee(Empcode, customerId, string.Empty, false);
                if (result == null || string.IsNullOrEmpty(result.Name))
                    result.EmployeeQuestions = _serviceProvider.GetRequiredService<IQuestionaireService>().GetQuestions()
                    .Select(_ => new EmployeeQuestionaireViewModel()
                    {
                        QuestionId = _.Value,
                        Questionname = _.Text
                    }).ToList();
            }
            result.Mode = Mode;
            //return PartialView("/Views/Shared/_EmployeeView.cshtml", result);
            return Json(result);
        }
        [HttpGet]
        public IActionResult GetAllEmployees()
        {
            var result = _serviceProvider.GetRequiredService<IEmployeeService>().GetAllEmployees(_executionContext.CustomerId);
            return Json(result);
        }
        [Authorize]
        [HttpGet]
        public IActionResult ExportEmployees()
        {
            var result = _serviceProvider.GetRequiredService<IEmployeeService>().GetAllEmployeesWithQuestions(_executionContext.CustomerId);
            var builder = new StringBuilder();
            builder.AppendLine("EmpCode,Name,Designation,From date (DD/MM/YYYY),To date (DD/MM/YYYY),Reason for leaving,Exit Type(Voluntary/Involuntary),Work location,Jobtype (Permanent/Contract...),Last drawn salary,Reporting to,ManagerDesignation,Duties and Responsibilities handled,Attitude & Personal Reputation of the Candidate,Performance at Work,Notice Served? Yes/No/Waived off,Exit formalities completed?,Any dues pending? (If yes, please elaborate),Eligible for Rehire,Specific remarks if any,Additional Comments");

            foreach(var record in result)
            {
                var row = $"{record.Employeecode},{record.Name},{record.Designation},{record.Fromdate},{record.Todate},{record.Reasonforleaving},{record.ExitType},{record.Location},{record.Jobtype},{record.Lastdrawnsalary},{record.Reportingto},{record.Managerdesignation}";
                var qns = new StringBuilder();
                foreach (var ans in record.EmployeeQuestions)
                {
                    qns.Append($"{ans.Answer},");
                }
                row += qns + record.Comments;
                builder.AppendLine(row);
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "employeerecords.csv");
        }
        //[HttpGet]
        //public IActionResult BindEmployeeForm(string Empcode)
        //{
        //    var customerId = _executionContext.CustomerId;
        //    var result = _serviceProvider.GetRequiredService<IEmployeeService>().ViewEmployee(Empcode, customerId, false);
        //    return PartialView("_EmployeeForm", result);
        //}
        [Authorize]
        [HttpGet]
        public IActionResult GetPendingApproval()
        {
            var result = _serviceProvider.GetRequiredService<IEmployeeService>().GetEmployeeApprovals(_executionContext.CustomerId);
            return Json(result);
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetEditPendingApproval()
        {
            var result = _serviceProvider.GetRequiredService<IEmployeeService>().GetEditEmployeeApprovals(_executionContext.CustomerId);
            return Json(result);
        }       
        [Authorize]
        [HttpPost]
        public IActionResult ApproveorRejectFile([FromBody] FileApprove model)
        {
            bool result = false;
            try
            {
                if (model.Mode == "Approve")
                    result = _serviceProvider.GetRequiredService<IEmployeeService>().ApproveFile(model.FileId, _executionContext.UserId);
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
        [HttpPost]
        public IActionResult SaveEmployeeQuestions([FromBody] EmployeeViewModel viewModel)
        {
            try
            {
                _serviceProvider.GetRequiredService<IEmployeeService>().SaveEmployeeQuestionaries(viewModel.Employeecode, viewModel.EmployeeQuestions);
                var approval = _serviceProvider.GetRequiredService<IEmployeeService>().GetEmployeeApproval(Convert.ToInt32(viewModel.Id));
                _serviceProvider.GetRequiredService<IEmployeeService>().UpdateEmployeeApproval(Convert.ToInt32(approval.Id), _executionContext.UserId);
                return Json("True");
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json("Unexpected error occured. Please contact support.");
            }
        }
        [Authorize]
        public IActionResult GetLatestUploads()
        {
            var result = _serviceProvider.GetRequiredService<IFileService>().GetUploadedFiles(_executionContext.UserId);
            return Json(result);
        }
        public FileResult DownloadTemplate()
        {
            //get file from respective table
            string reportpath = Path.Combine(_appEnvironment.WebRootPath, "Files", "Template.csv");

            byte[] fileBytes = System.IO.File.ReadAllBytes(reportpath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template.csv");

        }
        public FileResult GetDump()
        {
            //get file from respective table
            string reportpath = _serviceProvider.GetRequiredService<IEmployeeService>().GenerateEmployeeDetails(_executionContext.CustomerId);
            
            byte[] fileBytes = System.IO.File.ReadAllBytes(reportpath);
            var info = new FileInfo(reportpath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, info.Name);

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
                var isvalid = _serviceProvider.GetRequiredService<IUserService>().CheckOldPassword(_executionContext.UserId, viewModel.OldPassword, 2);
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
        [Authorize]
        public IActionResult EmployeeRequest()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "EEARRequest";
                ViewBag.IsBGV = false;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                ViewBag.Category = _authService.CurrentUser.Category;
                var result = _serviceProvider.GetRequiredService<IEmployeeService>().GetOpenRequests(_executionContext.CustomerId);
                return View(result);
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult ApproveEmployeeRequest([FromBody] EmployeeViewModel model)
        {
            model.IsEdit = false;
            try
            {
                model.Customerid = _executionContext.CustomerId;
                var result = _serviceProvider.GetRequiredService<IEmployeeService>().ApproveEmployee(model, _executionContext.UserId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(ex.Message);
            }
        }
        [Authorize]
        public IActionResult EmployeeNewRequest()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "VEVVNRequest";
                ViewBag.IsBGV = false;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                var allQuestions = _serviceProvider.GetRequiredService<IQuestionaireService>().GetQuestions();
                RequestViewModel data = new RequestViewModel()
                {
                    Mode = "AddEx",
                    Emailbody = _serviceProvider.GetRequiredService<ICustomerService>().GetEmailTemplate(_executionContext.CustomerId.ToString(), 1)
                };
                data.EmployeeQuestions = allQuestions.Select(x => new EmployeeQuestionaireViewModel
                {
                    QuestionId = x.Value,
                    Questionname = x.Text,
                    Answer = string.Empty
                }).ToList();
                return View(data);
            }
        }
        [Authorize]
        public IActionResult GetTempEmployee(string Empcode, string CustId)
        {
            var customerId = _executionContext.CustomerId;
            if (CustId != "")
                customerId = Convert.ToInt32(CustId);
            var result = _serviceProvider.GetRequiredService<IEmployeeService>().ViewTempEmployee(Empcode, customerId);
            if (result == null || string.IsNullOrEmpty(result.Name))
                result.EmployeeQuestions = _serviceProvider.GetRequiredService<IQuestionaireService>().GetQuestions()
                .Select(_ => new EmployeeQuestionaireViewModel()
                {
                    QuestionId = _.Value,
                    Questionname = _.Text
                }).ToList();
            result.Mode = "AddNew";
            return Json(result);
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetEmployeeRequests()
        {
            var result = _serviceProvider.GetRequiredService<IEmployeeService>().GetOpenRequests(_executionContext.CustomerId);
            return Json(result);
        }
        [Authorize]
        public IActionResult AttritionReport()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "ERPAR";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                ViewBag.Category = _authService.CurrentUser.Category;
                return View();
            }
        }
        [Authorize]
        public IActionResult ManagerApproval()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "EMGRAPPR";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.IsEducation = _authService.CurrentUser.IsEducation;
                ViewBag.IsEmployment = _authService.CurrentUser.IsEmployment;
                if (_authService.CurrentUser.CustomerType == 1 || _authService.CurrentUser.CustomerType == 3)
                    ViewBag.ExZone = true;
                else
                    ViewBag.ExZone = false;
                if (_authService.CurrentUser.CustomerType == 2 || _authService.CurrentUser.CustomerType == 3)
                    ViewBag.VZone = true;
                else
                    ViewBag.VZone = false;
                return View();
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetAttritionReport(string fromDate, string toDate)
        {            
            var result = _serviceProvider.GetRequiredService<IEmployeeService>()
                .GetEmployeeSearchAttrition(Convert.ToDateTime(DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)), Convert.ToDateTime(DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)), _executionContext.CustomerId, "Generated");
            return Json(result);
        }
        [HttpPost]
        public IActionResult ApproveEmployee([FromBody] EmployeeViewModel model)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<IEmployeeService>().ApproveEmployee(model, _executionContext.UserId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json("false");
            }
        }
        [HttpGet]
        public IActionResult RejectEmployee(int id, string comments)
        {
            var result = _serviceProvider.GetRequiredService<IEmployeeService>().RejectEmployee(Convert.ToInt32(id), comments, _executionContext.UserId);
            return Json(result.ToString());
        }
        public IActionResult GetCustomers()
        {
            var allCustomers = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomers(true, true);
            return Json(allCustomers);
        }
        [HttpGet]
        public IActionResult GetWalletTransactions()
        {
            var transactions = _serviceProvider.GetRequiredService<ICustomerService>().GetWalletTransactions(_executionContext.CustomerId);
            return Json(transactions);
        }
        public IActionResult GetInstitutions()
        {
            var allCustomers = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomers(true, false);
            return Json(allCustomers);
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetCustomerBalance()
        {
            int balance = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomerBalance(_executionContext.CustomerId);
            return Json(balance);
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetCustomer(string search)
        {
            var allCustomers = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomers(search.ToLower())
                .Select(_ => new SelectListItem()
                {
                    Text = _.Name,
                    Value = _.Id.ToString()
                }).ToList();
            return Json(allCustomers);
        }
        [Authorize]
        public IActionResult CandidateSearch()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "VEVCS";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                return View();
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetCandidate(string CustomerId, string Empcode, string Lastworkingdate)
        {
            var result = _serviceProvider.GetRequiredService<IEmployeeService>().ViewEmployee(Empcode, Convert.ToInt32(CustomerId), Lastworkingdate, true);
            return Json(result);
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetEmployeeApproval(string EmpId, string searchId, string isedit)
        {
            var result = _serviceProvider.GetRequiredService<IEmployeeService>().AddEmployeeApproval(Convert.ToInt32(EmpId), Convert.ToInt32(searchId), Convert.ToBoolean(isedit));
            return Json(result);
        }
        [Authorize]
        [HttpGet]
        public IActionResult GenerateReportBySearchId(int searchid)
        {
            var isGenerated = _serviceProvider.GetRequiredService<ICompanyService>().GenerateReportBySearchId(searchid, "customer");
            return Json(isGenerated);
        }
        [Authorize]
        [HttpGet]
        public IActionResult GenerateReportDataBySearchId(int searchid, string clientname)
        {
            if(string.IsNullOrEmpty(clientname))
            {
                clientname = _authService.CurrentUser.Customername;
            }
            var isGenerated = _serviceProvider.GetRequiredService<ICompanyService>().GetReportData(searchid, clientname);
            return Json(isGenerated);
        }

        [Authorize]
        public IActionResult PastSearches()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "RPPS";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                return View();
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetSearchHistory(string fromdate, string todate, string finalresult)
        {
            try
            {
                DateTime startdate = DateTime.ParseExact(fromdate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime enddate = DateTime.ParseExact(todate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var result = _serviceProvider.GetRequiredService<IEmployeeService>().GetEmployeeSearchHistory(startdate, enddate, finalresult);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(null);
            }
        }
        public FileResult DownloadFile(int searchid)
        {
            var searchRecord = _serviceProvider.GetRequiredService<IEmployeeService>().GetEmployeeSearch(searchid);
            //get file from respective table
            string reportpath = $"{_constants.Reports}{searchRecord.Reportlink}";

            byte[] fileBytes = System.IO.File.ReadAllBytes(reportpath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, searchRecord.Reportlink);

        }
        [Authorize]
        public IActionResult CandidateRequest()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "EVCRS";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.IsEducation = _authService.CurrentUser.IsEducation;
                ViewBag.IsEmployment = _authService.CurrentUser.IsEmployment;
                return View();
            }
        }
        
        [Authorize]
        public IActionResult AutoApprovalConfig()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "CONRVAC";
                ViewData["title"] = "Exzone - Auto approval configuration";
                //Get the current claims principal
                var identity = _contextAccessor.HttpContext.User;

                // Get the claims values
                var customertype = identity.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                                   .Select(c => c.Value).SingleOrDefault();
                ViewBag.CustomerType = customertype;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                ViewBag.Category = _authService.CurrentUser.Category;
                return View();
            }
        }
        [Authorize]
        public IActionResult AutoApprovalExclude()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "CONRVAE";
                ViewData["title"] = "Exzone - Exclusion from auto approval";
                //Get the current claims principal
                var identity = _contextAccessor.HttpContext.User;

                // Get the claims values
                var customertype = identity.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                                   .Select(c => c.Value).SingleOrDefault();
                ViewBag.CustomerType = customertype;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                ViewBag.Category = _authService.CurrentUser.Category;
                return View();
            }
        }
        [Authorize]
        public IActionResult GetApprovalConfig()
        {
            var result = _serviceProvider.GetRequiredService<ICustomerService>().GetAutoApprovalConfigs(_executionContext.UserId);
            return Json(result);
        }
        [HttpPost]
        public IActionResult AddApprovalConfig([FromBody] AutoApprovalConfigViewModel viewModel)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<ICustomerService>().AddAutoApprovalConfig(viewModel);
                if (result)
                    return Json(true);
                else
                    return Json(false);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(false);
            }
        }
        [HttpGet]
        public IActionResult DeleteApprovalConfig(string id)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<ICustomerService>().DeleteAutoApprovalConfig(Convert.ToInt32(id));
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(false);
            }
        }
        
        [Authorize]
        public IActionResult AbscondDetail()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "EEVCSAD";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.IsEducation = _authService.CurrentUser.IsEducation;
                ViewBag.IsEmployment = _authService.CurrentUser.IsEmployment;
                ViewBag.DiscrepancyTypes = _serviceProvider.GetRequiredService<IEmployeeService>().GetLookupDiscrepancyTypes();
                return View();
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult AddAbscondDetail([FromBody] AbscondDetailViewModel viewModel)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<IEmployeeService>().AddAbscondDetail(viewModel);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(false);
            }
        }
        [Authorize]
        public IActionResult SearchRecord()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "VEVCSSR";
                ViewBag.IsBGV = false;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                return View();
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetResults(string email, string mobile, string name, string uan, string others)
        {
            var result = _serviceProvider.GetRequiredService<IEmployeeService>().GetAbscondDetails(name, mobile, email, uan, others);
            return Json(result);
        }

        [Authorize]
        public IActionResult ApprovalHistory()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "ERPVHIS";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                ViewBag.Category = _authService.CurrentUser.Category;
                return View();
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetApprovalHistory(string fromdate, string todate)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<IEmployeeService>()
                    .GetApprovedData(DateTime.ParseExact(fromdate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        DateTime.ParseExact(todate, "dd-MM-yyyy", CultureInfo.InvariantCulture), _executionContext.UserId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(null);
            }
        }

        [Authorize]
        public IActionResult StudentUpload()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "SVSU";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.IsEducation = _authService.CurrentUser.IsEducation;
                ViewBag.IsEmployment = _authService.CurrentUser.IsEmployment;
                return View();
            }
        }
        [HttpPost]
        public JsonResult StudentUpload(IFormFile file)
        {
            if (file != null)
            {
                Random r = new Random();
                int randNum = r.Next(100000);
                string sixDigitNumber = randNum.ToString("D6");
                var viewmodel = new UploadSummaryViewModel();
                var result = _serviceProvider.GetRequiredService<IStudentService>().Validate(file, sixDigitNumber);
                if (result == "")
                {
                    viewmodel = _serviceProvider.GetRequiredService<IStudentService>().ParseFile(file, sixDigitNumber, _executionContext.CustomerId, _executionContext.UserId);
                }
                return Json(viewmodel);
            }
            else
                return Json("");
        }
        [Authorize]
        [HttpPost]
        public IActionResult ApproveorRejectStudentFile([FromBody] FileApprove model)
        {
            bool result = false;
            try
            {
                if (model.Mode == "Approve")
                    result = _serviceProvider.GetRequiredService<IStudentService>().ApproveFile(model.FileId, _executionContext.UserId);
                else
                    result = _serviceProvider.GetRequiredService<IStudentService>().RejectFile(model.FileId);

                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(false);
            }
        }

        [Authorize]
        public IActionResult StudentApproval()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "SVRV";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.IsEducation = _authService.CurrentUser.IsEducation;
                ViewBag.IsEmployment = _authService.CurrentUser.IsEmployment;
                var result = _serviceProvider.GetRequiredService<IStudentService>().GetStudentApprovals(_executionContext.CustomerId);
                return View(result);
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult StudentApproval(List<int> Ids)
        {
            try
            {
                foreach (var id in Ids)
                {
                    _serviceProvider.GetRequiredService<IStudentService>().UpdateStudentApproval(id, _executionContext.UserId);
                }

                return Json("True");
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json("Unexpected error occured. Please contact support.");
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetPendingStudentApproval()
        {
            var result = _serviceProvider.GetRequiredService<IStudentService>().GetStudentApprovals(_executionContext.CustomerId);
            return Json(result);
        }
        [HttpGet]
        public IActionResult BindStudentForm(string Id)
        {
            var result = _serviceProvider.GetRequiredService<IStudentService>().ViewStudentById(Convert.ToInt32(Id));
            return PartialView("_StudentForm", result);
        }

        [Authorize]
        public IActionResult StudentView()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "SVSV";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.IsEducation = _authService.CurrentUser.IsEducation;
                ViewBag.IsEmployment = _authService.CurrentUser.IsEmployment;
                var viewModel = new StudentViewModel();
                return View(viewModel);
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetStudents(string studentid)
        {
            var jsonData = _serviceProvider.GetRequiredService<IStudentService>().ViewStudents(studentid, _executionContext.CustomerId);
            return Json(jsonData);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetStudent(string id)
        {
            var jsonData = _serviceProvider.GetRequiredService<IStudentService>().ViewStudent(Convert.ToInt32(id), false);
            return Json(jsonData);
        }
        public FileResult DownloadStudentTemplate()
        {
            //get file from respective table
            string reportpath = $"{_appEnvironment.WebRootPath + @"\Files\StudentTemplate.csv"}";

            byte[] fileBytes = System.IO.File.ReadAllBytes(reportpath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template.csv");

        }
        [Authorize]
        public IActionResult StudentRequest()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "VEDVSRS";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.IsEducation = _authService.CurrentUser.IsEducation;
                ViewBag.IsEmployment = _authService.CurrentUser.IsEmployment;
                return View();
            }
        }
        [Authorize]
        public IActionResult GetStudentRequestsByCustomer([FromQuery] string filtervalue)
        {
            bool openOnly = false, repliedOnly = false;
            if (filtervalue == "openOnly") openOnly = true;
            if (filtervalue == "replyOnly") repliedOnly = true;
            var result = _serviceProvider.GetRequiredService<IStudentService>().GetStudentRequestByCustomer(_executionContext.CustomerId, openOnly, repliedOnly);
            return Json(result);
        }

        [Authorize]
        public IActionResult StudentSearch()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "VEDVSS";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.IsEducation = _authService.CurrentUser.IsEducation;
                ViewBag.IsEmployment = _authService.CurrentUser.IsEmployment;
                ViewBag.Institutions = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomers(true, false)
                .Select(_ => new SelectListItem()
                {
                    Text = _.Name,
                    Value = _.Id.ToString()
                }).ToList();
                return View();
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetStudentDetails(string custid, string studentid)
        {
            var jsonData = _serviceProvider.GetRequiredService<IStudentService>().ViewStudents(studentid, Convert.ToInt32(custid));
            return Json(jsonData);
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetStudentById(string id)
        {
            var jsonData = _serviceProvider.GetRequiredService<IStudentService>().ViewStudent(Convert.ToInt32(id), true);
            return Json(jsonData);
        }
        [Authorize]
        [HttpGet]
        public IActionResult GenerateStudentReportBySearchId(int searchid, string stuid, string custid, string studentid)
        {
            var isGenerated = _serviceProvider.GetRequiredService<ICompanyService>().GenerateStudentReport(Convert.ToInt32(stuid), searchid, "customer");
            return Json(isGenerated);
        }

        public FileResult DownloadStudentFile(int searchid)
        {
            var searchRecord = _serviceProvider.GetRequiredService<IStudentService>().GetStudentSearch(searchid);
            //get file from respective table
            string reportpath = $"{_constants.Reports}{searchRecord.Reportlink}";

            byte[] fileBytes = System.IO.File.ReadAllBytes(reportpath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, searchRecord.Reportlink);

        }
        [Authorize]
        [HttpGet]
        public IActionResult GetStudentApproval(string stuId, string searchId, string isedit)
        {
            var result = _serviceProvider.GetRequiredService<IStudentService>().AddStudentApproval(Convert.ToInt32(stuId), Convert.ToInt32(searchId), Convert.ToBoolean(isedit));
            return Json(result);
        }
        [Authorize]
        [HttpPost]
        public IActionResult StudentRaiseRequest([FromBody] StudentRequestViewModel viewModel)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<IStudentService>().AddStudentRequest(viewModel);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(false);
            }
        }

        [Authorize]
        public IActionResult StudentSearches()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "VEDVPS";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.IsEducation = _authService.CurrentUser.IsEducation;
                ViewBag.IsEmployment = _authService.CurrentUser.IsEmployment;
                ViewBag.Institutions = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomers(true, false)
                .Select(_ => new SelectListItem()
                {
                    Text = _.Name,
                    Value = _.Id.ToString()
                }).ToList();
                return View();
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetStudentSearchHistory(string fromDate, string toDate, string finalResult)
        {
            try
            {
                DateTime startdate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime enddate = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var result = _serviceProvider.GetRequiredService<IStudentService>().GetStudentSearchHistory(startdate, enddate, finalResult);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(new List<StudentSearchViewModel>());
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetStudentSearchById(string id)
        {
            var result = _serviceProvider.GetRequiredService<IStudentService>().GetStudentSearch(Convert.ToInt32(id));
            return Json(result);
        }

        [Authorize]
        public IActionResult StudentInfoRequest()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "SVSIR";
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.IsEducation = _authService.CurrentUser.IsEducation;
                ViewBag.IsEmployment = _authService.CurrentUser.IsEmployment;
                var result = _serviceProvider.GetRequiredService<IStudentService>().GetOpenRequests(_executionContext.CustomerId);
                return View(result);
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetStudentRequests()
        {
            var result = _serviceProvider.GetRequiredService<IStudentService>().GetOpenRequests(_executionContext.CustomerId);
            return Json(result);
        }
        [Authorize]
        [HttpPost]
        public IActionResult UpdateStudentRequest([FromBody] StudentRequestViewModel viewModel)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<IStudentService>().UpdateStudentRequest(viewModel);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(false);
            }
        }

        [Authorize]
        public IActionResult EmpReports()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "RPCR";
                ViewBag.IsBGV = false;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                return View();
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetEmployeeReports(string month, string year)
        {
            var result = _serviceProvider.GetRequiredService<IEmployeeService>().GetGeneratedRecordsByCustomer(Convert.ToInt32(month), Convert.ToInt32(year), _executionContext.CustomerId);
            return Json(result);
        }

        [Authorize]
        public IActionResult EmailSetting()
        {
            ViewData["page"] = "EmailSetting";
            ViewBag.IsBGV = false;
            ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;            
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetCustomerEmailSetting()
        {
            var jsonData = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomerEmailSettings(_executionContext.CustomerId);
            return Json(jsonData);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddEmailSetting([FromBody] CustomerEmailSettingViewModel viewModel)
        {
            var result = _serviceProvider.GetRequiredService<ICustomerService>().AddCustomerEmailSetting(viewModel);
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetEmailTemplate(int templateId, string customerId)
        {
            var setting = _serviceProvider.GetRequiredService<ICustomerService>().GetEmailTemplate(customerId, templateId);
            return Json(setting);
        }

        [HttpGet]
        public IActionResult SendRecertificationReminder(int searchId)
        {
            var result = _serviceProvider.GetRequiredService<IEmployeeService>().SendEmploymentVerificationApprovalReminder(searchId);
            return Json(result);
        }

        [HttpGet]
        public IActionResult ExcludeEmployee(string Id)
        {
            var approval = new ApprovalExclusionViewModel()
            {
                customerId = _executionContext.CustomerId,
                employeeId = Convert.ToInt32(Id),
                excludedBy = _executionContext.UserId.ToString(),
            };

            var result = _serviceProvider.GetRequiredService<ICustomerService>().AddApprovalExclusion(approval);
            return Json(result);
        }
        [HttpGet]
        public IActionResult GetAllExclusions()
        {
            var result = _serviceProvider.GetRequiredService<ICustomerService>().GetApprovalExclusionsByCustomer(_executionContext.CustomerId);
            return Json(result);
        }
        [HttpGet]
        public IActionResult DeleteApprovalExclusion(int id)
        {
            var result = _serviceProvider.GetRequiredService<ICustomerService>().DeleteApprovalExclusion(id);
            return Ok(new
            {
                success = true,
                message = result.ToString()
            });
        }
        [Authorize]
        public IActionResult EmployeeConsent()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewData["page"] = "ECM";
                ViewBag.IsBGV = false;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                ViewBag.Category = _authService.CurrentUser.Category;
                return View();
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetConsentStatuses()
        {
            var result = _serviceProvider.GetRequiredService<IConsentService>().GetConsentStatuses();
            return Json(result);
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetConsentRequests(string name, string empcode, string email, int statusId, string fromdate, string todate)
        {
            try
            {
                DateTime? startdate = string.IsNullOrEmpty(fromdate) ? (DateTime?)null : DateTime.ParseExact(fromdate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime? enddate = string.IsNullOrEmpty(todate) ? (DateTime?)null : DateTime.ParseExact(todate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var result = _serviceProvider.GetRequiredService<IConsentService>().GetConsentRequests(name, empcode, email, statusId, startdate, enddate);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(null);
            }
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendConsentRequest([FromBody] ConsentRequestViewModel model)
        {
            try
            {
                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                var consentRequestId = _serviceProvider.GetRequiredService<IConsentService>().SendConsentRequest(model, baseUrl);
                return Json(new { success = true, consentrequestid = consentRequestId });
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult CancelConsentRequest(int id)
        {
            try
            {
                var result = _serviceProvider.GetRequiredService<IConsentService>().CancelConsentRequest(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
                return Json(false);
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetConsentAuditLogs(int id)
        {
            var result = _serviceProvider.GetRequiredService<IConsentService>().GetAuditLogs(id);
            return Json(result);
        }
        [Authorize]
        public IActionResult ExzoneRoadmap()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                ViewBag.Category = _authService.CurrentUser.Category;
                return View();
            }
        }
        [Authorize]
        public IActionResult VzoneRoadmap()
        {
            if (_executionContext.UserId == 0)
                return RedirectToAction("Index", "Home");
            else
            {
                ViewBag.IsBGV = _authService.CurrentUser.IsBGV;
                ViewBag.AccessLevel = _authService.CurrentUser.CustomerType;
                ViewBag.Category = _authService.CurrentUser.Category;
                return View();
            }
        }
    }
}
