using AutoMapper;
using OrgCheck.ViewModels;
using System;
using System.Linq;
using OrgCheck.Middleware;
using OrgCheck.Models;
using OrgCheck.DataAccess.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using OrgCheck.Services.Interfaces;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.Threading.Tasks;
using System.Diagnostics;
using OrgCheck.DataAccess;
using NPOI.SS.Formula.Functions;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using CsvHelper.Configuration;

namespace OrgCheck.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ExecutionContext _executionContext;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly EmailService _emailService;
        private readonly IMapper _mapper;
        private readonly Constants _constants;
        public EmployeeService(IServiceProvider serviceProvider, IWebHostEnvironment appEnvironment, ExecutionContext executionContext, IMapper mapper,
            EmailService emailService, Constants constants)
        {
            _serviceProvider = serviceProvider;
            _appEnvironment = appEnvironment;
            _executionContext = executionContext;
            _mapper = mapper;
            _emailService = emailService;
            _constants = constants;
        }
        public HRDashboardViewModel GetDashboardData(int year, int customerId)
        {
            var result = new HRDashboardViewModel()
            {
                approvalcount = new List<int>(),
                reportcount = new List<int>()
            };
            result.requestcount = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeApprovals(customerId, true).Count;
            result.requestcount += _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeApprovals(customerId, false).Count;
            var resultset = _serviceProvider.GetRequiredService<IEmployeeDA>().GetApprovedData(0, year, customerId);
            var reportresult = _serviceProvider.GetRequiredService<IEmployeeDA>().GetGeneratedReportsByCustomerMonth(0, year, customerId);
            for (int month = 1; month <= 12; month++)
            {
                result.approvalcount.Add(resultset.Count(_ => _.Approveddate.Value.Month == month));
                result.reportcount.Add(reportresult.Count(_ => _.Reportdate.Value.Month == month));
            }
            return result;
        }
        public CustomerDashboardCount GetDashboardCount(int month, int year, int userId)
        {
            var dashboardCount = new CustomerDashboardCount();
            dashboardCount.CompletedCount = _serviceProvider.GetRequiredService<IEmployeeDA>().GetApprovalGivenCount(month, year, userId);
            dashboardCount.ApprovalCount = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeApprovals(_executionContext.CustomerId, false).Count;
            dashboardCount.DownloadCount = _serviceProvider.GetRequiredService<IEmployeeDA>().GetGeneratedReportsCountByCustomerMonth(month, year, userId);
            dashboardCount.RequestCount = _serviceProvider.GetRequiredService<IEmployeeDA>().GetOpenRequestsByCustomer(_executionContext.CustomerId).Count;
            return dashboardCount;
        }
        public string AddEmployee(EmployeeViewModel viewModel, string inputFormat, bool isApproved = true)
        {
            var questionarelist = new List<Employeequestionaire>();
            viewModel.Id = "0";
            viewModel.Customerid = _executionContext.CustomerId;
            var existingrecord = _serviceProvider.GetRequiredService<IEmployeeDA>().ViewEmployee(viewModel.Employeecode, _executionContext.CustomerId, string.Empty);
            if (existingrecord != null && existingrecord.Id > 0)
                return "exists";

            var record = _mapper.Map<Employee>(viewModel);
            record.Fromdate = DateTime.ParseExact(viewModel.Fromdate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            record.Todate = DateTime.ParseExact(viewModel.Todate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            record.Name = _serviceProvider.GetRequiredService<CryptoService>().Encrypt(viewModel.Name);
            record.Isedit = true;
            record.Isapproved = isApproved;
            record.Createdby = _executionContext.UserId;
            record.Createddate = DateTime.Now;
            var transaction = new Customerwallettransaction()
            {
                Customerid = viewModel.Customerid,
                Transactiontype = 1,
                Remarks = inputFormat,
                Createdby = _executionContext.UserId,
                Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Status = 1
            };
            if (viewModel.EmployeeQuestions != null)
            {
                string[] invalids = new string[] { "-", "/", "--", "---" };  //, "NA", "N/A", "N.A", "NOT APPLICABLE", "N A"
                int qnsCount = 0;
                foreach (var qn in viewModel.EmployeeQuestions)
                {
                    if (qn.Answer.Trim() != "" && !invalids.Contains(qn.Answer.Trim().ToUpper()))
                        qnsCount++;
                    questionarelist.Add(new Employeequestionaire()
                    {
                        Questionid = Convert.ToInt32(qn.QuestionId.Replace("qns_", "")),
                        Answer = qn.Answer
                    });
                }
                if(qnsCount > _constants.CreditQuestionThreshold)
                {
                    record.Isedit = false;
                    transaction.Credits = 1.0;
                }
                else
                {
                    transaction.Credits = 0.5;
                }
            }
            else
            {
                transaction.Credits = 0.5;
            }
            _serviceProvider.GetRequiredService<IEmployeeDA>().AddEmployee(record, questionarelist);
            _serviceProvider.GetRequiredService<ICustomerDA>().AddCustomerWallet(transaction);
            return "true";
        }
        public EmployeeViewModel ViewEmployee(string empCode, int customerId, string lastWorkingdate, bool addSearch)
        {
            var record = _serviceProvider.GetRequiredService<IEmployeeDA>().ViewEmployee(empCode, customerId, lastWorkingdate);
            var result = _mapper.Map<EmployeeViewModel>(record);
            if (record != null)
            {
                result.Name = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(record.Name);
                result.Fromdate = record.Fromdate.ToString("dd-MM-yyyy");
                result.Todate = record.Todate.ToString("dd-MM-yyyy");
            }
            var search = new Employeesearch();
            if (addSearch)
                search = new Employeesearch()
                {
                    Searchrequestid = _serviceProvider.GetRequiredService<IEmployeeDA>().GenerateSearchRequestNumber(),
                    Employeecode = empCode,
                    Customerid = customerId,
                    Employeeid = (result == null ? null : record.Id),
                    Name = (result == null ? "" : record.Name),
                    Createdby = _executionContext.UserId,
                    Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                    Finalresult = "Searched",
                    Transactionamount = 0.0,
                    Reportlink = "",
                    Status = 1
                };
            if (result != null)
            {
                var approval = _serviceProvider.GetRequiredService<IEmployeeDA>().GetLatestApproval(record.Id);
                if (approval != null)
                {
                    result.AuthorizedBy = approval.ApprovedbyNavigation?.Displayname + " (" + approval.ApprovedbyNavigation?.Emailid + ")";
                    result.AuthorizedDate = approval.Approveddate?.ToString("dd-MM-yyyy");
                }
                //search.Name = record.Name;
                search.Searchresult = "F";
                
                if (addSearch)
                {
                    result.EmployeeQuestions = ViewEmployeeQuestions(empCode, customerId);
                    result.SearchId = _serviceProvider.GetRequiredService<IEmployeeDA>().AddEmployeeSearch(search);
                }
                else
                {
                    result.EmployeeQuestions = ViewAllEmployeeQuestions(empCode, customerId);
                }
            }
            else
            {
                result = new EmployeeViewModel();
                search.Searchresult = "N";
                if (addSearch)
                    result.SearchId = _serviceProvider.GetRequiredService<IEmployeeDA>().AddEmployeeSearch(search);
                var customer = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomer(customerId);
                string emailBody = $"<br><br>The employee record {empCode} in {customer.Name} was not found";
                string emailSubject = "VerifyZone - No record found";
                string emailTo = _constants.NoRecordNotificationEmail;
                _emailService.SendEmail(emailTo, string.Empty, string.Empty, emailSubject, emailBody);
            }                            
            return result;
        }
        public List<EmployeeViewModel> GetAllEmployees(int customerId)
        {
            List<EmployeeViewModel> result = new List<EmployeeViewModel>();
            var records = _serviceProvider.GetRequiredService<IEmployeeDA>().GetAllEmployees(customerId);
            foreach(var record in records)
            {
                var emp = _mapper.Map<EmployeeViewModel>(record);
                emp.Name = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(record.Name);
                emp.Fromdate = record.Fromdate.ToString("dd-MM-yyyy");
                emp.Todate = record.Todate.ToString("dd-MM-yyyy");
                result.Add(emp);
            }
            return result;
        }
        public List<EmployeeViewModel> GetAllEmployeesWithQuestions(int customerId)
        {
            List<EmployeeViewModel> result = new List<EmployeeViewModel>();
            var records = _serviceProvider.GetRequiredService<IEmployeeDA>().GetAllEmployeesWithDetails(customerId);
            foreach (var record in records)
            {
                var emp = _mapper.Map<EmployeeViewModel>(record);
                emp.Name = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(record.Name);
                emp.Fromdate = record.Fromdate.ToString("dd-MMM-yyyy");
                emp.Todate = record.Todate.ToString("dd-MMM-yyyy");
                if(emp.ExitType == "1") emp.ExitType = "Voluntary";
                else if (emp.ExitType == "2") emp.ExitType = "Involuntary";
                emp.EmployeeQuestions = new List<EmployeeQuestionaireViewModel>();
                foreach(var qns in record.Employeequestionaires)
                {
                    emp.EmployeeQuestions.Add(new EmployeeQuestionaireViewModel()
                    {
                        Questionname = qns.Question.Question,
                        Answer = qns.Answer
                    });
                }                
                result.Add(emp);
            }
            return result;
        }

        public List<EmployeeQuestionaireViewModel> ViewAllEmployeeQuestions(string empCode, int customerId)
        {
            var lists = _serviceProvider.GetRequiredService<IEmployeeDA>().GetAllEmployeeQuestions(empCode, customerId);
            var data = new List<EmployeeQuestionaireViewModel>();
            foreach (var record in lists)
            {
                data.Add(new EmployeeQuestionaireViewModel()
                {
                    QuestionId = record.Questionid.ToString(),
                    Questionname = record.Question.Question,
                    Answer = string.IsNullOrEmpty(record.Answer) ? "" : record.Answer
                });
            }
            return data;
        }
        public List<EmployeeQuestionaireViewModel> ViewEmployeeQuestions(string empCode, int customerId)
        {
            var lists = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeQuestions(empCode, customerId);
            var data = new List<EmployeeQuestionaireViewModel>();
            foreach(var record in lists)
            {
                data.Add(new EmployeeQuestionaireViewModel()
                {
                    Questionname = record.Question.Question,
                    Answer = record.Answer
                });
            }
            return data;
        }
        public bool AddEmployeeApproval(int employeeId, int searchId, bool isEdit)
        {
            var record = new Employeeapproval()
            {
                Employeeid = employeeId,
                Requestedby = _executionContext.UserId,
                Requesteddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Employeesearchid = searchId,
                Isedit = isEdit
            };            
            _serviceProvider.GetRequiredService<IEmployeeDA>().AddApproval(record);

            // Check for auto approval
            var custId = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeById(employeeId).Customerid;            
            var approvalsList = _serviceProvider.GetRequiredService<ICustomerDA>().GetAutoapprovalconfigsByCustomer(custId);
            var leastapproval = approvalsList.Where(_ => _.Enddate >= DateTime.Now.Date && _.Status == 1).OrderBy(_ => _.Id).Take(1).SingleOrDefault();
            if (leastapproval != null)
            {
                var exclusionlists = _serviceProvider.GetRequiredService<ICustomerDA>().GetAllExclusions(custId);
                var count = exclusionlists.Count(_ => _.Employeeid == employeeId);
                if (count == 0)
                {
                    UpdateEmployeeApproval(record.Id, leastapproval.Createdby);
                    var hr_user = _serviceProvider.GetRequiredService<IUserDA>().GetUser(leastapproval.Createdby);
                    var empl = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeById(employeeId);
                    var emplname = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(empl.Name);
                    string emailBody = $"Dear {hr_user.Displayname},<br><br>A Re-certification request for your ex-employee {emplname} ({empl.Employeecode}) was approved automatically.<br><br>This is FYI.";
                    string emailSubject = "VerifyZone : Ex-Employee Re-certification was auto approved";
                    string emailTo = hr_user.Emailid;
                    _emailService.SendEmail(emailTo, string.Empty, string.Empty, emailSubject, emailBody);
                    return true;
                }
            }

            // Update the corresponding status on employee search table
            var searchrecord = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeSearchById(searchId);
            searchrecord.Finalresult = "Sent for approval";
            _serviceProvider.GetRequiredService<IEmployeeDA>().UpdateEmployeeSearch(searchrecord);
            if (record.Approvedby == null)
            {
                // Send email notification to the respective HRs (if multiple)                
                var hr_users = _serviceProvider.GetRequiredService<IUserDA>().GetUsersByCustomer(custId);
                var customersetting = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomerEmailsetting(custId.ToString(), 2, _executionContext.CustomerId);
                foreach (var user in hr_users)
                {
                    string emailBody = "";
                    if (customersetting == null || customersetting.Templatecontent == "")
                    {
                        emailBody = $"Dear HR,<br><br>You have received a background re-certification request for your ex-employee.";
                        emailBody += $"<br>Kindly login to Verifyzone portal, through this link <a href='https://app.verifyzone.in/'>VerifyZone</a> and re-certify it.";
                        emailBody += $"<br><br>Thank you,<br>VerifyZone support team";
                    }
                    else
                        emailBody = customersetting.Templatecontent;
                        string emailSubject = "VerifyZone : Ex-Employee Re-certification";
                    string emailTo = user.Emailid;
                    _emailService.SendEmail(emailTo, string.Empty, string.Empty, emailSubject, emailBody);
                }
            }
            return true;        
        }
        public bool UpdateClientEmployeeSearch(EmployeeSearchViewModel viewModel)
        {
            var search = new Employeesearch()
            {
                Id = viewModel.Id,
                Clientname = viewModel.Clientname,
            };
            _serviceProvider.GetRequiredService<IEmployeeDA>().UpdateClientEmployeeSearch(search);
            return true;
        }
        public bool UpdateEmployeeApproval(int id, int userId)
        {
            var record = new Employeeapproval()
            {
                Id = id,
                Approvedby = userId,
                Approveddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
            };
            _serviceProvider.GetRequiredService<IEmployeeDA>().UpdateApproval(record);

            var approval = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeApprovalById(id);

            // Update the corresponding status on employee search table
            var searchrecord = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeSearchById(approval.Employeesearchid.Value);
            searchrecord.Finalresult = "Approved";
            _serviceProvider.GetRequiredService<IEmployeeDA>().UpdateEmployeeSearch(searchrecord);

            // Send email notification to the respective BGV user
            
            var user = _serviceProvider.GetRequiredService<IUserDA>().GetUser(approval.Requestedby.Value);
            string emailBody = $"Dear {user.Displayname},<br><br>Your request for employee re-verify in VerifyZone portal was given by HR";
            emailBody += $"<br>Kindly login into the VerifyZone portal and proceed.";
            emailBody += $"<br>Thank you,<br>VerifyZone support team";
            string emailSubject = "VerifyZone - Employee re-verify request given";
            string emailTo = user.Emailid;
            _emailService.SendEmail(emailTo, string.Empty, string.Empty, emailSubject, emailBody);
            return true;
        }
        public List<EmployeeApprovalViewModel> GetEmployeeApprovals(int customerId)
        {
            var list = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeApprovals(customerId, false);
            var data = new List<EmployeeApprovalViewModel>();
            foreach(var item in list)
            {
                var newModel = new EmployeeApprovalViewModel()
                {
                    Id = item.Id.ToString(),
                    EmployeeId = item.Employeeid.ToString(),
                    EmpCode = item.Employee.Employeecode,
                    EmployeeName = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(item.Employee.Name),
                    RequestedBy = item.RequestedbyNavigation.Displayname,
                    RequestedDate = item.Requesteddate.Value.ToString("dd-MM-yyyy"),
                    IsEdit = "false"
                };
                var customer = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomer(item.RequestedbyNavigation.Customerid.Value);
                newModel.RequestedOrganisation = customer.Name;
                data.Add(newModel);
            }
            var unapprovedlist = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeApprovals(customerId, true);
            foreach (var item in unapprovedlist)
            {
                var newModel = new EmployeeApprovalViewModel()
                {
                    Id = item.Id.ToString(),
                    EmployeeId = item.Employeeid.ToString(),
                    EmpCode = item.Employee.Employeecode,
                    EmployeeName = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(item.Employee.Name),
                    RequestedBy = item.RequestedbyNavigation.Displayname,
                    RequestedDate = item.Requesteddate.Value.ToString("dd-MM-yyyy"),
                    IsEdit = "true"
                };
                var customer = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomer(item.RequestedbyNavigation.Customerid.Value);
                newModel.RequestedOrganisation = customer.Name;
                data.Add(newModel);
            }
            return data;
        }
        public int GetApprovalGivenCount(int month, int year, int userId)
        {
            return _serviceProvider.GetRequiredService<IEmployeeDA>().GetApprovalGivenCount(month, year, userId);
        }
        public List<EmployeeApprovalViewModel> GetApprovedData(DateTime fromDate, DateTime toDate, int userId)
        {
            var list = _serviceProvider.GetRequiredService<IEmployeeDA>().GetApprovedData(fromDate, toDate, userId);
            return list.Select(_ => new EmployeeApprovalViewModel()
            {
                Id = _.Id.ToString(),
                EmployeeId = _.Employeeid.ToString(),
                EmpCode = _.Employee.Employeecode,
                EmployeeName = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(_.Employee.Name),
                ApprovedDate = _.Approveddate.Value.ToString("dd-MM-yyyy")
            }).ToList();
        }
        public List<EmployeeApprovalViewModel> GetEditEmployeeApprovals(int customerId)
        {
            var list = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeApprovals(customerId, true);
            return list.Select(_ => new EmployeeApprovalViewModel()
            {
                Id = _.Id.ToString(),
                EmployeeId = _.Employeeid.ToString(),
                EmpCode = _.Employee.Employeecode,
                EmployeeName = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(_.Employee.Name),
                RequestedBy = _.RequestedbyNavigation.Displayname,
                RequestedOrganisation = _.RequestedbyNavigation.Customer.Name,
                RequestedDate = _.Requesteddate.Value.ToString("dd-MM-yyyy")
            }).ToList();
        }
        public List<EmployeeSearchViewModel> GetEmployeeSearchHistory(DateTime fromDate, DateTime toDate, string finalResult)
        {
            var list = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeSearch(fromDate, toDate, _executionContext.UserId, finalResult);            
            var result = new List<EmployeeSearchViewModel>();
            foreach (var record in list)
            {
                var entry = new EmployeeSearchViewModel()
                {
                    Searchrequestid = record.Searchrequestid,
                    Customerid = record.Customerid,
                    Name = (!string.IsNullOrEmpty(record.Name) ? _serviceProvider.GetRequiredService<CryptoService>().Decrypt(record.Name) : ""),
                    Employeecode = record.Employeecode,
                    Searchresult = (record.Searchresult == "F" ? "Record found" : "Record not found"),
                    Id = record.Id,
                    Finalresult = record.Finalresult,
                    Reportdate = record.Reportdate,
                    Reportlink = record.Reportlink,
                    CustomerName = (record.Clientname == null ? "" : record.Clientname),
                    Transactionamount = record.Transactionamount,
                    CreatedbyName = record.CreatedbyNavigation.Displayname,
                    Createddate = record.Createddate.ToString("dd-MM-yyyy"),
                    ApprovedDate = ""
                };
                switch(entry.Finalresult)
                {
                    case "Searched":
                    case "Approved":
                        entry.ActionStatus = "2";
                        break;
                    case "Generated":
                        entry.ActionStatus = "1";
                        break;
                    default:
                        entry.ActionStatus = "";
                        break;
                }
                if (record.Searchresult == "F")
                {
                    var employee = _serviceProvider.GetRequiredService<IEmployeeDA>().ViewEmployee(record.Employeecode, record.Customerid, string.Empty);
                    var maxApprovedEntry = _serviceProvider.GetRequiredService<IEmployeeDA>().GetLatestApproval(employee.Id);
                    if (maxApprovedEntry != null && maxApprovedEntry.Approveddate.HasValue)
                        entry.ApprovedDate = maxApprovedEntry.Approveddate.Value.ToString("dd-MM-yyyy");
                }
                result.Add(entry);
            }
            return result;
        }
        public List<EmployeeSearchViewModel> GetEmployeeSearchAttrition(DateTime fromDate, DateTime toDate, int customerId, string finalResult)
        {
            var list = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeSearchData(fromDate, toDate, customerId, finalResult);
            var result = new List<EmployeeSearchViewModel>();
            foreach (var record in list)
            {
                var entry = new EmployeeSearchViewModel()
                {
                    Searchrequestid = record.Searchrequestid,
                    Customerid = record.Customerid,
                    Name = (!string.IsNullOrEmpty(record.Name) ? _serviceProvider.GetRequiredService<CryptoService>().Decrypt(record.Name) : ""),
                    Employeecode = record.Employeecode,
                    Searchresult = (record.Searchresult == "F" ? "Record found" : "Record not found"),
                    Id = record.Id,
                    Finalresult = record.Finalresult,
                    Reportdate = record.Reportdate,
                    Reportlink = record.Reportlink,
                    //CustomerName = (string.IsNullOrEmpty(record.Clientname) ? record.CreatedbyNavigation.Customer.Name
                    //    : record.CreatedbyNavigation.Customer.Name + " (on behalf of " + record.Clientname + ")"),
                    CustomerName = record.CreatedbyNavigation.Customer.Name,
                    Clientname = (record.Clientname ?? ""),
                    Transactionamount = record.Transactionamount,
                    CreatedbyName = record.CreatedbyNavigation.Displayname,
                    Createddate = record.Createddate.ToString("dd-MM-yyyy"),
                    ApprovedDate = ""
                };
                //switch (entry.Finalresult)
                //{
                //    case "Searched":
                //    case "Approved":
                //        entry.ActionStatus = "2";
                //        break;
                //    case "Generated":
                //        entry.ActionStatus = "1";
                //        break;
                //    default:
                //        entry.ActionStatus = "";
                //        break;
                //}
                //if (record.Searchresult == "F")
                //{
                //    var employee = _serviceProvider.GetRequiredService<IEmployeeDA>().ViewEmployee(record.Employeecode, record.Customerid);
                //    var maxApprovedEntry = _serviceProvider.GetRequiredService<IEmployeeDA>().GetLatestApproval(employee.Id);
                //    if (maxApprovedEntry != null && maxApprovedEntry.Approveddate.HasValue)
                //        entry.ApprovedDate = maxApprovedEntry.Approveddate.Value.ToString("dd/MM/yyyy");
                //}
                result.Add(entry);
            }
            return result;
        }
        public EmployeeApprovalViewModel GetEmployeeApproval(int empId)
        {
            var approval = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeApprovalByEmployeeId(empId);
            return new EmployeeApprovalViewModel()
            {
                Id = approval.Id.ToString(),
                EmployeeId = approval.Employeeid.ToString(),
                RequestedBy = approval.Requestedby.ToString(),
                RequestedDate = approval.Requesteddate.ToString(),
            };
        }

        public EmployeeSearchViewModel GetEmployeeSearch(int searchId)
        {
            var search = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeSearchById(searchId);
            var result = new EmployeeSearchViewModel()
            {
                Id = search.Id,
                Searchrequestid = search.Searchrequestid,
                Employeecode = search.Employeecode,
                Customerid = search.Customerid,
                CustomerName = search.Customer.Name,
                Searchresult = search.Searchresult,
                Reportlink = search.Reportlink,
                Transactionamount = search.Transactionamount,
                Finalresult = search.Finalresult
            };
            return result;
        }
        public string Validate(IFormFile file, string strSixDigitNumber)
        {
            string _result = string.Empty;
            if (file.Length == 0)
                _result = "Corrupted file";
            else
            {
                var info = new FileInfo(file.FileName);
                //var fileNamealone = Path.GetFileName(info.FullName);
                if (info.Extension != ".csv" && !file.ContentType.Contains("excel"))
                    _result = "Invalid file. Only csv file is allowed to upload";
                else
                {
                    var filePath = Path.Combine(_appEnvironment.WebRootPath, "uploads", Path.GetFileNameWithoutExtension(file.FileName) + "_" + strSixDigitNumber + ".csv"); //$"{folderPath}{Path.GetFileNameWithoutExtension(file.FileName)}_{strSixDigitNumber}.csv";
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    var Lines = System.IO.File.ReadLines(filePath).ToList();
                    if (Lines.Count <= 1)
                        _result = "Empty file.";
                }
            }
            return _result;
        }
        public UploadSummaryViewModel ParseFile(IFormFile file, string strSixDigitNumber, int customerId, int userId)
        {
            var resultModel = new UploadSummaryViewModel();
            resultModel.errors = new List<ErrorLogViewModel>();
            var validdatalist = new List<EmployeeViewModel>();

            var filePath = Path.Combine(_appEnvironment.WebRootPath, "uploads", Path.GetFileNameWithoutExtension(file.FileName) + "_" + strSixDigitNumber + ".csv");
            var existingEmployeeLists = _serviceProvider.GetRequiredService<IEmployeeDA>().GetAllEmployees(customerId);

            var Lines = System.IO.File.ReadLines(filePath).ToList();
            int lineCount = 1;
            var currentRow = Lines[lineCount];
            while (currentRow != "")
            {
                var values = currentRow.Split(',');
                var dr = new EmployeeViewModel();
                dr.EmployeeQuestions = new List<EmployeeQuestionaireViewModel>();
                dr.Employeecode = values[0].Trim();
                dr.Name = values[1].Trim();
                dr.Designation = values[2].Trim();
                dr.Fromdate = values[3];
                dr.Todate = values[4];
                dr.Reasonforleaving = values[5].Trim();
                if (values[6].Trim() == "Voluntary")
                    dr.ExitType = "1";
                else if (values[6].Trim() == "Involuntary")
                    dr.ExitType = "2";
                dr.Location = values[7].Trim();
                dr.Jobtype = values[8].Trim();
                dr.Lastdrawnsalary = values[9].Trim();
                dr.Reportingto = values[10].Trim();
                dr.Managerdesignation = values[11].Trim();                

                dr.EmployeeQuestions.Add(new EmployeeQuestionaireViewModel()
                {
                    QuestionId = "1",
                    Answer = values[12].Trim()
                });
                dr.EmployeeQuestions.Add(new EmployeeQuestionaireViewModel()
                {
                    QuestionId = "2",
                    Answer = values[13].Trim()
                });
                dr.EmployeeQuestions.Add(new EmployeeQuestionaireViewModel()
                {
                    QuestionId = "3",
                    Answer = values[14].Trim()
                });
                dr.EmployeeQuestions.Add(new EmployeeQuestionaireViewModel()
                {
                    QuestionId = "4",
                    Answer = values[15].Trim()
                });
                dr.EmployeeQuestions.Add(new EmployeeQuestionaireViewModel()
                {
                    QuestionId = "5",
                    Answer = values[16].Trim()
                });
                dr.EmployeeQuestions.Add(new EmployeeQuestionaireViewModel()
                {
                    QuestionId = "6",
                    Answer = values[17].Trim()
                });
                dr.EmployeeQuestions.Add(new EmployeeQuestionaireViewModel()
                {
                    QuestionId = "7",
                    Answer = values[18].Trim()
                });
                dr.Comments = values[19].Trim();

                var errors = ValidateEmployee(dr);
                if (errors.Count == 0)
                {
                    // Check for duplication
                    var existingEmployee = existingEmployeeLists.Find(_ => _.Employeecode == dr.Employeecode.Trim());
                    if (existingEmployee != null && !string.IsNullOrEmpty(existingEmployee.Name))
                    {
                        resultModel.invalidrecords++;
                        resultModel.errors.Append(new ErrorLogViewModel()
                        {
                            errorcode = dr.Employeecode + ", " + dr.Name,
                            errordescription = "Employee code already exists."
                        });
                    }
                    else
                    {
                        resultModel.validrecords++;
                        validdatalist.Add(dr);
                    }
                }
                else
                {
                    resultModel.invalidrecords++;
                    foreach (var record in errors)
                        resultModel.errors.Append(record);
                }
                lineCount++;
                if (lineCount >= Lines.Count)
                    currentRow = "";
                else
                    currentRow = Lines[lineCount];
            }
            resultModel.totalrecords = resultModel.validrecords + resultModel.invalidrecords;

            // Add entry for the file
            var fInfo = new FileInfo(filePath);
            var newfile = new OrgCheck.Models.File()
            {
                Filename = fInfo.Name,
                Filesize = (int)fInfo.Length,
                Customerid = customerId,
                Uploadedby = _executionContext.UserId,
                Uploadeddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Totalrecords = resultModel.totalrecords,
                Validrecords = resultModel.validrecords,
                Invalidrecords = resultModel.invalidrecords,
                Uploadedstatus = 1,
                Status = 1
            };
            newfile.Id = _serviceProvider.GetRequiredService<IFileDA>().AddFile(newfile);
            resultModel.fileid = newfile.Id;
            // Add valid employees in the file to DB
            
            int listindex = 0;
            foreach (var item in validdatalist)
            {
                string fromDate, toDate = string.Empty;
                string[] date = item.Fromdate.Replace("-", "/").Split('/');
                fromDate = date[0].PadLeft(2, '0') + "/" + date[1].PadLeft(2, '0') + "/" + date[2];
                date = item.Todate.Replace("-", "/").Split('/');
                toDate = date[0].PadLeft(2, '0') + "/" + date[1].PadLeft(2, '0') + "/" + date[2];
                var record = new Tempemployee()
                {
                    Name = item.Name,
                    Employeecode = item.Employeecode,
                    Designation = item.Designation,
                    Fromdate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Todate = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Customerid = customerId,
                    Jobtype = item.Jobtype,
                    Lastdrawnsalary = item.Lastdrawnsalary,
                    Exittype = item.ExitType,
                    Location = item.Location,
                    Reportingto = item.Reportingto,
                    Managerdesignation = item.Managerdesignation,
                    Reasonforleaving = item.Reasonforleaving,
                    Comments = item.Comments ?? "",
                    Fileid = newfile.Id,
                    Createdby = userId,
                    Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                };
                var tempEmp = _serviceProvider.GetRequiredService<IEmployeeDA>().AddTempEmployee(record);
                var empQuestions = new Tempemployeequestionaire[item.EmployeeQuestions.Count];
                foreach(var qnitem in item.EmployeeQuestions) 
                {
                    var subitem = new Tempemployeequestionaire()
                    {
                        Tempemployeeid = tempEmp.Id,
                        Questionid = Convert.ToInt32(qnitem.QuestionId),
                        Answer = qnitem.Answer
                    };
                    empQuestions[listindex] = subitem;
                    listindex++;
                }
                _serviceProvider.GetRequiredService<IEmployeeDA>().AddTempEmployeeQuestions(empQuestions);                
            }
            
            return resultModel;
        }
        public List<ErrorLogViewModel> ValidateEmployee(EmployeeViewModel viewModel)
        {
            var result = new List<ErrorLogViewModel>();
            if (string.IsNullOrEmpty(viewModel.Employeecode))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Employeecode + ", " + viewModel.Name,
                    errordescription = "Employee code is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Name))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Employeecode + ", " + viewModel.Name,
                    errordescription = "Employee name is empty"
                });
            else if (viewModel.Name.Trim().Length <= 2)
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Employeecode + ", " + viewModel.Name,
                    errordescription = "Employee name is invalid"
                });
            if (string.IsNullOrEmpty(viewModel.Designation))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Employeecode + ", " + viewModel.Name,
                    errordescription = "Designation is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Fromdate))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Employeecode + ", " + viewModel.Name,
                    errordescription = "Employee FromDate is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Todate))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Employeecode + ", " + viewModel.Name,
                    errordescription = "Employee ToDate is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Reasonforleaving))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Employeecode + ", " + viewModel.Name,
                    errordescription = "'Reason for leaving' is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Location))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Employeecode + ", " + viewModel.Name,
                    errordescription = "Location is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Jobtype))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Employeecode + ", " + viewModel.Name,
                    errordescription = "Employee job type is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Lastdrawnsalary))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Employeecode + ", " + viewModel.Name,
                    errordescription = "Last salary drawn is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Reportingto))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Employeecode + ", " + viewModel.Name,
                    errordescription = "'Reporting To' is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Managerdesignation))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Employeecode + ", " + viewModel.Name,
                    errordescription = "Manager Designation is empty"
                });

            return result;
        }
        public int AddTempEmployee(RequestViewModel item, int userId)
        {
            if (!string.IsNullOrEmpty(item.Customername))
            {
                var viewModel = new CustomerViewModel()
                {
                    Name = item.Customername,
                    Contactname = item.Hrname,
                    Email = item.Hremail,
                    IsEducation = false,
                    IsEmployment = true,
                    IsBGV = false
                };
                // Create new customer
                _serviceProvider.GetRequiredService<ICustomerService>().AddCustomer(viewModel);
                var lists = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomers("");
                item.Customerid = lists.FirstOrDefault(x => x.Name == item.Customername).Id;
            }
            var record = new Tempemployee()
            {
                Name = item.Name,
                Employeecode = item.Employeecode,
                Designation = item.Designation,
                Fromdate = DateTime.ParseExact(item.Fromdate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                Todate = DateTime.ParseExact(item.Todate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                Customerid = item.Customerid,
                Jobtype = item.Jobtype ?? "",
                Lastdrawnsalary = item.Lastdrawnsalary ?? "",
                Location = item.Location ?? "",
                Reportingto = item.Reportingto ?? "",
                Managerdesignation = item.Managerdesignation ?? "",
                Reasonforleaving = item.Reasonforleaving ?? "",
                Comments = item.Comments ?? "",
                Createdby = userId,
                Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
            };
            var tempEmp = _serviceProvider.GetRequiredService<IEmployeeDA>().AddTempEmployee(record);
            var empQuestions = new Tempemployeequestionaire[item.EmployeeQuestions.Count];
            int empQuestionsCount = 0;
            bool isSubItemAvailable = false;
            foreach (var qnitem in item.EmployeeQuestions)
            {
                if (!string.IsNullOrEmpty(qnitem.Answer))                
                    isSubItemAvailable = true;
                var subitem = new Tempemployeequestionaire()
                {
                    Tempemployeeid = tempEmp.Id,
                    Questionid = Convert.ToInt32(qnitem.QuestionId.Replace("qns_", "")),
                    Answer = qnitem.Answer
                };
                empQuestions[empQuestionsCount] = subitem;
                empQuestionsCount++;                
            }
            if (isSubItemAvailable)
            {
                _serviceProvider.GetRequiredService<IEmployeeDA>().AddTempEmployeeQuestions(empQuestions);
            }
            return tempEmp.Id;
        }
        public EmployeeViewModel ViewTempEmployee(string empCode, int customerId)
        {
            var record = _serviceProvider.GetRequiredService<IEmployeeDA>().ViewTempEmployee(empCode, customerId);
            var result = new EmployeeViewModel()
            {
                Id = record.Id.ToString(),
                Customerid = record.Customerid.Value,
                Employeecode = record.Employeecode,
                Name = record.Name,
                Designation = record.Designation,
                Location = record.Location,
                Fromdate = (record.Fromdate.HasValue ? record.Fromdate.Value.ToString("dd/MM/yyyy") : ""),
                Todate = (record.Todate.HasValue ? record.Todate.Value.ToString("dd/MM/yyyy") : ""),
                Jobtype = record.Jobtype,
                Lastdrawnsalary = record.Lastdrawnsalary,
                Reportingto = record.Reportingto,
                Reasonforleaving = record.Reasonforleaving,
                Managerdesignation = record.Managerdesignation,
                Comments = record.Comments,
            };
            result.EmployeeQuestions = _serviceProvider.GetRequiredService<IEmployeeDA>().GetAllTempEmployeeQuestions(empCode, customerId)
                .Select(x => new EmployeeQuestionaireViewModel()
                {
                    QuestionId = x.Questionid.ToString(),
                    Answer = x.Answer,
                    Questionname=x.Question.Question
                }).ToList();
            return result;
        }
        public List<EmployeeViewModel> GetTempemployees(int fileId)
        {
            var results = new List<EmployeeViewModel>();
            var list = _serviceProvider.GetRequiredService<IEmployeeDA>().GetTempemployees(fileId);
            foreach (var item in list)
            {
                var empViewModel = new EmployeeViewModel()
                {
                    Id = item.Id.ToString(),
                    Name = item.Name,
                    Designation = item.Designation,
                    Employeecode = item.Employeecode,
                    Location = item.Location,
                    Fromdate = item.Fromdate.Value.ToString("dd-MM-yyyy"),
                    Todate = item.Todate.Value.ToString("dd-MM-yyyy"),
                    Lastdrawnsalary = item.Lastdrawnsalary,
                    Reasonforleaving = item.Reasonforleaving,
                    Reportingto = item.Reportingto,
                    Managerdesignation = item.Managerdesignation,
                    Jobtype = item.Jobtype,
                    Comments = item.Comments,
                    Customerid = item.Customerid.GetValueOrDefault()
                };
                empViewModel.EmployeeQuestions = new List<EmployeeQuestionaireViewModel>();
                foreach(var qns in item.Tempemployeequestionaires)
                {
                    var qnViewModel = new EmployeeQuestionaireViewModel()
                    {
                        QuestionId = qns.Questionid.ToString(),
                        Answer = qns.Answer,
                    };
                    empViewModel.EmployeeQuestions.Add(qnViewModel);
                }
                results.Add(empViewModel);
            }
            return results;
        }
        public bool ApproveFile(int fileId, int userId)
        {
            var records = GetTempemployees(fileId);
            if(userId == 0)
            {
                var record = _serviceProvider.GetRequiredService<IEmployeeDA>().GetTempemployees(fileId).FirstOrDefault();
                userId = record.Createdby;
            }
            //var emplists = new Employee[records.Count];
            //int index = 0;
            var file = new OrgCheck.Models.File()
            {
                Id = fileId,
                Uploadedstatus = 2,
            };
            var transaction = new Customerwallettransaction()
            {
                Customerid = records[0].Customerid,
                Transactiontype = 2,
                Credits = 0.0,
                Remarks = "Bulk upload",
                Createdby = _executionContext.UserId,
                Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Status = 1
            };
            foreach (var record in records)
            {
                record.IsEdit = true;

                var emp = new Employee()
                {
                    Name = _serviceProvider.GetRequiredService<CryptoService>().Encrypt(record.Name),
                    Customerid = record.Customerid,
                    Employeecode = record.Employeecode,
                    Designation = record.Designation,
                    Fromdate = DateTime.ParseExact(record.Fromdate.Replace("-","/"), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Todate = DateTime.ParseExact(record.Todate.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Reasonforleaving = record.Reasonforleaving,
                    Exittype = record.ExitType,
                    Location = record.Location,
                    Jobtype = record.Jobtype,
                    Lastdrawnsalary = record.Lastdrawnsalary,
                    Reportingto = record.Reportingto,
                    Managerdesignation = record.Managerdesignation,
                    Comments = record.Comments,
                    Isapproved = true,
                    Isedit = false,
                    Createdby = userId,
                    Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                };
                var employeeQuestions = new List<Employeequestionaire>();
                string[] invalids = new string[] { "-", "/", "NA", "N/A", "N.A", "NOT APPLICABLE", "N A", "--", "---" };
                int qnsCount = 0;
                foreach (var qns in record.EmployeeQuestions)
                {
                    if (qns.Answer.Trim() != "" && !invalids.Contains(qns.Answer.Trim().ToUpper()))
                        qnsCount++;
                    employeeQuestions.Add(new Employeequestionaire()
                    {
                        Questionid = Convert.ToInt32(qns.QuestionId),
                        Answer = qns.Answer
                    });
                }
                if (qnsCount > 4)
                {
                    transaction.Credits += 1.0;
                }
                else
                {
                    emp.Isedit = true;
                    transaction.Credits += 0.5;
                }
                _serviceProvider.GetRequiredService<IEmployeeDA>().AddEmployee(emp, employeeQuestions);                
            }
            if (records.Count > 0)
                _serviceProvider.GetRequiredService<ICustomerDA>().AddCustomerWallet(transaction);

            _serviceProvider.GetRequiredService<IFileDA>().UpdateFile(file);
            _serviceProvider.GetRequiredService<IEmployeeDA>().DeleteTempEmployees(fileId);
            return true;
        }
        public bool RejectFile(int fileId)
        {
            var records = GetTempemployees(fileId);
            var file = new OrgCheck.Models.File()
            {
                Id = fileId,
                Uploadedstatus = 3
            };
            _serviceProvider.GetRequiredService<IFileDA>().UpdateFile(file);
            _serviceProvider.GetRequiredService<IEmployeeDA>().DeleteTempEmployees(fileId);
            return true;
        }
        public bool SaveEmployeeQuestionaries(string empCode, List<EmployeeQuestionaireViewModel> questions)
        {
            var questionarelist = new List<Employeequestionaire>();
            foreach (var qn in questions)
            {
                questionarelist.Add(new Employeequestionaire()
                {
                    Questionid = Convert.ToInt32(qn.QuestionId.Replace("qt", "")),
                    Answer = qn.Answer
                });
            }
            _serviceProvider.GetRequiredService<IEmployeeDA>().SaveEmployeeQuestions(empCode, _executionContext.CustomerId, questionarelist);
            return true;
        }

        public string GenerateEmployeeDetails(int customerId)
        {
            var data = _serviceProvider.GetRequiredService<IEmployeeDA>().GetAllEmployeesWithDetails(customerId);
            var filename = $"{_appEnvironment.WebRootPath}\\Files\\{DateTime.Now.ToString("ddMMyyyyhhmmss")}.csv";
            var configPersons = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };
            using (StreamWriter streamWriter = new StreamWriter(filename, true))
            using (CsvWriter writer = new CsvWriter(streamWriter, configPersons))
            {
                var line = "Name,Employeecode,Designation,Fromdate,Todate,Reasonforleaving,Location,Jobtype,Lastdrawnsalary,ReportingTo,ManagerDesignation,Comments";
                foreach(var qns in data[0].Employeequestionaires)                
                    line += "," + qns.Question.Question;                
                writer.WriteRecord(line);
            }

            return filename;
        }

        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetLookupVerificationResponses()
        {
            return _serviceProvider.GetRequiredService<IEmployeeDA>().GetAllLookupVerifications().Select(_ => new SelectListItem()
            {
                Text = _.Name,
                Value = _.Id.ToString(),
                Selected = false
            }).ToList();
        }
        
        public string AddEmpVerificationRequest(RequestViewModel model, int userId)
        {
            var existingrecord = _serviceProvider.GetRequiredService<IEmployeeDA>().ViewEmployee(model.Employeecode, model.Customerid, string.Empty);
            if(existingrecord != null) { return "exists"; }
            int tempEmpId = AddTempEmployee(model, userId);
            var request = new Empverificationrequest()
            {
                Tempemployeeid = tempEmpId,
                Requeststatus = "Open",
                Requestnumber = _serviceProvider.GetRequiredService<IEmployeeDA>().GenerateRequestNumber(),
                Createdby = userId,
                Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Active = 1
            };
            _serviceProvider.GetRequiredService<IEmployeeDA>().AddEmpVerificationRequest(request);

            var customersetting = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomerEmailsetting(model.Customerid.ToString(), 1, _executionContext.CustomerId);
            if (customersetting != null && customersetting.Id > 0)
            {
                // Update the emailcontent to the existing record
                customersetting.Templatecontent = model.Emailbody;
                _serviceProvider.GetRequiredService<ICustomerDA>().UpdateCustomerEmailSetting(customersetting);
            }
            else
            {
                // Add the emailcontent as new record
                customersetting = new Customeremailsetting()
                {
                    Templateid = 1,
                    Customerid = model.Customerid,
                    Templatecontent = model.Emailbody,
                    Createdby = _executionContext.UserId,
                    Createdcustomerid = _executionContext.CustomerId,
                    Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                };
                _serviceProvider.GetRequiredService<ICustomerDA>().AddCustomerEmailSetting(customersetting);
            }

            // Sending email
            var users = _serviceProvider.GetRequiredService<IUserDA>().GetUsersByCustomer(model.Customerid);
            string toEmails = string.Join(",", users.Select(x => x.Emailid).ToList());
            string emailBody = model.Emailbody;
            emailBody = emailBody.Replace("\n", "<br>");
            _emailService.SendEmail(toEmails, string.Empty, string.Empty, "Verifyzone : Ex-employee verification request", emailBody);
            return "true";
        }
        public bool SendEmploymentVerificationApprovalReminder(int searchId)
        {
            var search = GetEmployeeSearch(searchId);
            var setting = _serviceProvider.GetRequiredService<ICustomerService>().GetEmailTemplate(search.Customerid.ToString(), 2);

            // Sending email
            var users = _serviceProvider.GetRequiredService<IUserDA>().GetUsersByCustomer(search.Customerid);
            string toEmails = string.Join(";", users.Select(x => x.Emailid).ToList());
            string emailBody = setting;
            emailBody = emailBody.Replace("\n", "<br>");
            _emailService.SendEmail(toEmails, string.Empty, string.Empty, "Verifyzone : Ex-employee verification request followup", emailBody);
            return true;
        }
        public EmployeeRequestViewModel GetEmployeeVerificationRequest(int id)
        {
            var request = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmpverificationrequestById(id);
            var data = new EmployeeRequestViewModel()
            {
                Id = request.Id,
                Reportname = request.Reportname,
            };
            if (request.Employee != null)
            {
                data.Empcode = request.Employee.Employeecode;
                data.Hrcomments = request.Employee.Comments;
            }
            else if(request.Tempemployee != null)
            {
                data.Empcode = request.Tempemployee.Employeecode;
                data.Hrcomments = request.Tempemployee.Comments;
            }
            else if (request.Invalidemployee != null)
            {
                data.Empcode = request.Invalidemployee.Employeecode;
                data.Hrcomments = request.Invalidemployee.Comments;
            }
            return data;
        }
        public List<RequestViewModel> GetEmployeeVerificationRequests(string status, string ticketnumber, int customerId)
        {
            var data = new List<RequestViewModel>();
            var lists = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmpverificationrequests(status, ticketnumber, customerId).ToList();
            foreach (var item in lists)
            {
                var request = new RequestViewModel()
                {
                    Id = item.Id.ToString(),
                    Comments = item.Requestnumber,
                    AuthorizedBy = item.CreatedbyNavigation.Loginname + " (" + item.CreatedbyNavigation.Designation + ")",
                    AuthorizedDate = item.Createddate.Value.ToString("dd-MM-yyyy"),
                    Location = item.Requeststatus
                };
                if(item.Tempemployee != null)
                {
                    request.Employeecode = item.Tempemployee.Employeecode;
                    request.Name = item.Tempemployee.Name;
                    request.Customername = item.Tempemployee.Customer.Name;
                }
                else if (item.Employee != null)
                {
                    request.Employeecode = item.Employee.Employeecode;
                    request.Name = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(item.Employee.Name);
                    request.Customername = item.Employee.Customer.Name;
                }
                else if (item.Invalidemployee != null)
                {
                    request.Employeecode = item.Invalidemployee.Employeecode;
                    request.Name = item.Invalidemployee.Name;
                    request.Customername = item.Invalidemployee.Customer.Name;
                }
                data.Add(request);
            }
            return data;
        }
        public List<EmployeeRequestViewModel> GetOpenRequests(int customerId)
        {
            var returnlist = new List<EmployeeRequestViewModel>();
            var list = _serviceProvider.GetRequiredService<IEmployeeDA>().GetOpenRequestsByCustomer(customerId);
            foreach(var item in list)
            {
                var user = _serviceProvider.GetRequiredService<IUserDA>().GetUser(item.Createdby);
                returnlist.Add(new EmployeeRequestViewModel()
                {
                    Id = item.Id,
                    Customerid = item.Customerid.Value,
                    Customername = item.Customer.Name,
                    Empcode = item.Employeecode,
                    Name = item.Name,
                    Requestcomments = item.Name,
                    Raisedby = item.Createdby,
                    RaisedByName = user.Displayname + " (" + user.Customer.Name + ")",
                    Raiseddate = item.Createddate.ToString("dd-MM-yyyy")
                });
            }
            return returnlist;
        }        
        //public List<EmployeeSearchViewModel> GetGeneratedRecords(int month, int year, int companyId)
        //{
        //    var list = _serviceProvider.GetRequiredService<IEmployeeDA>().GetGeneratedReportsByCompanyMonth(month, year, companyId);
        //    var result = list.Select(_ => new EmployeeSearchViewModel()
        //    {
        //        Searchrequestid = _.Searchrequestid,
        //        CustomerName = _.Customer.Name,
        //        Employeecode = _.Employeecode,
        //        Reportdate = _.Reportdate.Value,
        //        CreatedbyName = _.CreatedbyNavigation.Loginname
        //    }).OrderBy(x => x.Reportdate).ToList();

        //    return result;
        //}
        public List<EmployeeSearchViewModel> GetGeneratedRecordsByCustomer(int month, int year, int customerId)
        {
            var list = _serviceProvider.GetRequiredService<IEmployeeDA>().GetGeneratedReportsByCustomerMonth(month, year, customerId);
            var result = list.Select(_ => new EmployeeSearchViewModel()
            {
                Searchrequestid = _.Searchrequestid,
                Searchresult = _.Searchresult,
                Employeecode = _.Employeecode,
                Reportdate = _.Reportdate.Value,
                CreatedbyName = _.CreatedbyNavigation.Displayname + " (" + _.CreatedbyNavigation.Customer.Name + ")",
                Clientname = _.Clientname,
                Finalresult = "-",
            }).OrderBy(x => x.Reportdate).ToList();
            foreach(var item in result)
            {
                if(item.Searchresult == "F")
                {
                    var emp = _serviceProvider.GetRequiredService<IEmployeeDA>().ViewEmployee(item.Employeecode, customerId, string.Empty);
                    item.Finalresult = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(emp.Name);
                }
            }
            return result;
        }
        public AdminReportViewModel GetAdminDashboardData(int month, int year)
        {
            var viewmodel = new AdminReportViewModel()
            {
                rcviewmodels = new List<ReportCountViewModel>(),
                //cmpviewmodels = new List<CompanyCountViewModel>(),
                yearwisecount = new List<ReportCountViewModel>(),
                data = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>()
            };
            var customers = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomers(true, true);
            //var companies = _serviceProvider.GetRequiredService<ICompanyDA>().GetCompanies();
            var users = _serviceProvider.GetRequiredService<IUserService>().GetAllUsers(0);
            var rptcount = _serviceProvider.GetRequiredService<IEmployeeDA>().GetMonthwiseGeneratedReportsCount(DateTime.Now.Month, DateTime.Now.Year);
            viewmodel.data.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Customers", Value = customers.Count.ToString() });
            //viewmodel.data.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Companies", Value = companies.Count.ToString() });
            viewmodel.data.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Users", Value = users.Count.ToString() });
            viewmodel.data.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Reports", Value = rptcount.ToString() });
            foreach(var c in customers)
            {
                var count = _serviceProvider.GetRequiredService<IEmployeeDA>().GetGeneratedReportsCountByCustomerMonth(month, year, c.Id);
                viewmodel.rcviewmodels.Add(new ReportCountViewModel()
                {
                    CustomerName = c.Name,
                    Count = count
                });
            }
            //foreach (var c in companies)
            //{
            //    var count = _serviceProvider.GetRequiredService<IEmployeeDA>().GetSearchCountByCompanyMonth(month, year, c.Id);
            //    viewmodel.cmpviewmodels.Add(new CompanyCountViewModel()
            //    {
            //        CompanyName = c.Name,
            //        Count = count
            //    });
            //}
            for (int monthindex = 1; monthindex <= 12; monthindex++)
            {
                DateTime date = new DateTime(year, monthindex, 1);
                var count = _serviceProvider.GetRequiredService<IEmployeeDA>().GetMonthwiseGeneratedReportsCount(monthindex, year);
                viewmodel.yearwisecount.Add(new ReportCountViewModel()
                {
                    CustomerName = date.ToString("MMM"),
                    Count = count
                });
            }
            return viewmodel;
        }
        
        public bool ReconcileCustomerCredit(int customerId)
        {
            _serviceProvider.GetRequiredService<ICustomerDA>().ReconcileCustomerCredit(customerId, _executionContext.UserId);
            return true;
        }

        public string ApproveEmployee(EmployeeViewModel model, int userId)
        {
            var request = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmpverificationrequestByTempId(Convert.ToInt32(model.Id));
            string tempEmployeeId = model.Id;
            var result = AddEmployee(model, "Ex-employee request", true);
            if (result == "true")
            {
                var employee = _serviceProvider.GetRequiredService<IEmployeeDA>().ViewEmployee(model.Employeecode, model.Customerid, string.Empty);
                _serviceProvider.GetRequiredService<IEmployeeDA>().UpdateEmpVerificationRequest(request.Id, employee.Id, "Approved", string.Empty);
                _serviceProvider.GetRequiredService<IEmployeeDA>().DeleteTempEmployeeById(Convert.ToInt32(tempEmployeeId));

                var login = _serviceProvider.GetRequiredService<IUserDA>().GetUser(request.Createdby);
                string toEmails = login.Emailid;
                string emailBody = $"Dear user,{Environment.NewLine}{Environment.NewLine}Your request has been approved.{Environment.NewLine}";
                emailBody += $"Request no.: {request.Requestnumber}{Environment.NewLine}{Environment.NewLine}Thanks and Regards,{Environment.NewLine}Verifyzone IT Support team";
                _emailService.SendEmail(toEmails, string.Empty, string.Empty, "Your candidate request approved in VerifyZone", emailBody);
                return "true";
            }
            else
                return "false";
        }
        public bool RejectEmployee(int Id, string Comments, int userId)
        {
            var request = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmpverificationrequestByTempId(Id);
            var tempemployee = _serviceProvider.GetRequiredService<IEmployeeDA>().GetTempemployeeById(Id);

            var record = new Invalidemployee()
            {
                Name = tempemployee.Name,
                Employeecode = tempemployee.Employeecode,
                Designation = tempemployee.Designation,
                Fromdate = DateTime.SpecifyKind(tempemployee.Fromdate.Value, DateTimeKind.Utc),
                Todate = DateTime.SpecifyKind(tempemployee.Todate.Value, DateTimeKind.Utc),
                Customerid = tempemployee.Customerid,
                Jobtype = tempemployee.Jobtype,
                Lastdrawnsalary = tempemployee.Lastdrawnsalary,
                Location = tempemployee.Location,
                Reportingto = tempemployee.Reportingto,
                Managerdesignation = tempemployee.Managerdesignation,
                Reasonforleaving = tempemployee.Reasonforleaving,
                Comments = Comments,
                Createdby = userId,
                Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
            };
            record = _serviceProvider.GetRequiredService<IEmployeeDA>().AddInvalidEmployee(record);
            var empQuestions = new Invalidemployeequestionaire[tempemployee.Tempemployeequestionaires.Count];
            int empQuestionsCount = 0;
            bool isSubItemAvailable = false;
            foreach (var qnitem in tempemployee.Tempemployeequestionaires)
            {
                if (!string.IsNullOrEmpty(qnitem.Answer))
                    isSubItemAvailable = true;
                var subitem = new Invalidemployeequestionaire()
                {
                    Invalidemployeeid = record.Id,
                    Questionid = qnitem.Questionid,
                    Answer = qnitem.Answer
                };
                empQuestions[empQuestionsCount] = subitem;
                empQuestionsCount++;
            }
            if (isSubItemAvailable)
            {
                _serviceProvider.GetRequiredService<IEmployeeDA>().AddInvalidEmployeeQuestions(empQuestions);
            }
            _serviceProvider.GetRequiredService<IEmployeeDA>().UpdateEmpVerificationRequest(request.Id, record.Id, "Rejected", string.Empty);
            _serviceProvider.GetRequiredService<IEmployeeDA>().DeleteTempEmployeeById(Id);

            var login = _serviceProvider.GetRequiredService<IUserDA>().GetUser(request.Createdby);
            string toEmails = login.Emailid;
            string emailBody = $"Dear user,{Environment.NewLine}{Environment.NewLine}Your request for ex-employee has been answered.{Environment.NewLine}";
            emailBody += $"Request no.: {request.Requestnumber}{Environment.NewLine}{Environment.NewLine}Thanks and Regards,{Environment.NewLine}Verifyzone IT Support team";
            _emailService.SendEmail(toEmails, string.Empty, string.Empty, "Ex-employee request answered in VerifyZone", emailBody);
            return true;
        }

        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetLookupDiscrepancyTypes()
        {
            return _serviceProvider.GetRequiredService<IEmployeeDA>().GetDiscrepancytypes().Select(_ => new SelectListItem()
            {
                Text = _.Name,
                Value = _.Id.ToString(),
                Selected = false
            }).ToList();
        }
        public bool AddAbscondDetail(AbscondDetailViewModel viewModel)
        {
            var record = new Absconddetail()
            {
                Employeeid = viewModel.Employeeid,
                Fathername = viewModel.Fathername,
                Emailid = viewModel.Emailid,
                Mobileno = viewModel.Mobileno,
                Uannumber = viewModel.Uannumber,
                Linkedinurl = viewModel.Linkedinurl,
                Resume = viewModel.Resume.FileName,
                Discrepancetype = Convert.ToInt32(viewModel.DiscrepancyType),                
                Status = 1,
                Createdby = _executionContext.UserId,
                Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
            };
            _serviceProvider.GetRequiredService<IEmployeeDA>().AddabscondDetail(record);
            return true;
        }
        public List<AbscondDetailViewModel> GetAbscondDetails(string name, string mobile, string email, string uan, string others)
        {
            return _serviceProvider.GetRequiredService<IEmployeeDA>().GetAbsconddetails(name, mobile, email, uan, others)
                .Select(_ => new AbscondDetailViewModel()
                {
                    Id = _.Id,
                    Customername = _.CreatedbyNavigation.Customer.Name,
                    Name = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(_.Employee.Name),
                    Employeecode = _.Employee.Employeecode,
                    Fathername = _.Fathername,
                    Emailid = (string.IsNullOrEmpty(_.Emailid) ? "-" : _.Emailid),
                    Mobileno = (string.IsNullOrEmpty(_.Mobileno) ? "-" : _.Mobileno),
                    Uannumber = (string.IsNullOrEmpty(_.Uannumber) ? "-" : _.Uannumber),
                    Linkedinurl = (string.IsNullOrEmpty(_.Linkedinurl) ? "-" : _.Linkedinurl),
                    Joindate = _.Employee.Fromdate.ToString("dd-MM-yyyy"),
                    Lastworkingdate = _.Employee.Todate.ToString("dd-MM-yyyy"),
                    Resumename = (string.IsNullOrEmpty(_.Resume) ? "-" : _.Resume),
                    Remarks = _.Employee.Comments,
                }).ToList();
        }
        public async Task<byte[]> DownloadObjectFromBucketAsync(int id, string objectName)
        {
            var credentials = new BasicAWSCredentials(_constants.AWSAccessKey, _constants.AWSSecretKey);
            IAmazonS3 s3 = new AmazonS3Client(credentials, Amazon.RegionEndpoint.APSouth1);

            // Create a GetObject request
            var request = new GetObjectRequest
            {
                BucketName = _constants.AWSBucketName,
                Key = "prod/" + id.ToString() + "/" + objectName,
            };

            // Issue request and remember to dispose of the response
            using GetObjectResponse response = await s3.GetObjectAsync(request);

            try
            {
                return ReadFully(response.ResponseStream);
            }
            catch (AmazonS3Exception ex)
            {
                return null;
            }
        }
        public byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        public async Task<bool> UploadFileAsync(string bucketName, string objectName, string filePath)
        {
            var credentials = new BasicAWSCredentials(_constants.AWSAccessKey, _constants.AWSSecretKey);
            IAmazonS3 s3 = new AmazonS3Client(credentials, Amazon.RegionEndpoint.APSouth1);
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectName,
                FilePath = filePath,
            };

            var response = await s3.PutObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                //Console.WriteLine($"Successfully uploaded {objectName} to {bucketName}.");
                return true;
            }
            else
            {
                //Console.WriteLine($"Could not upload {objectName} to {bucketName}.");
                return false;
            }
        }        
    }
}
