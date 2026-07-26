using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Ocsp;
using OrgCheck.DataAccess.Interfaces;
using OrgCheck.Middleware;
using OrgCheck.Models;
using OrgCheck.Report.ReportModel;
using OrgCheck.Services.Interfaces;
using OrgCheck.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using ThirdParty.Json.LitJson;

namespace OrgCheck.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ExecutionContext _executionContext;
        private readonly IMapper _mapper;
        private readonly Constants _constants;
        public CompanyService(IServiceProvider serviceProvider, ExecutionContext executionContext, IMapper mapper, Constants constants)
        {
            _serviceProvider = serviceProvider;
            _executionContext = executionContext;
            _mapper = mapper;
            _constants = constants;
        }
        public List<CompanyViewModel> GetCompanies()
        {
            var record = _serviceProvider.GetRequiredService<ICompanyDA>().GetCompanies();
            return record.Select(_ => new CompanyViewModel()
            {
                Id = _.Id,
                Name = _.Name,
                Address = _.Address,
                Contactname = _.Contactname,
                Contactnumber = _.Contactnumber,
                Email = _.Email
            }).ToList();
        }
        public CompanyViewModel GetCompany(int id)
        {
            var record = _serviceProvider.GetRequiredService<ICompanyDA>().GetCompany(id);
            var result = _mapper.Map<CompanyViewModel>(record);
            return result;
        }
        public string AddCompany(CompanyViewModel viewModel)
        {
            var record = _mapper.Map<Company>(viewModel);
            record.Createdby = _executionContext.UserId;
            record.Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            record.Charges = 100.0;
            record.Educharges = 100.0;
            if (_serviceProvider.GetRequiredService<ICompanyDA>().IsDuplicateCompany(0, viewModel.Name))
                return "exists";
            var company = _serviceProvider.GetRequiredService<ICompanyDA>().AddCompany(record);
            // Adding default entry in Company wallet table
            var walletEntry = new Companywallet()
            {
                Companyid = company.Id,
                Totalcredit = 0,
                Status = 1
            };
            _serviceProvider.GetRequiredService<ICompanyDA>().AddCompanyWallet(walletEntry);
            return "true";
        }
        public string UpdateCompany(CompanyViewModel viewModel)
        {
            var record = _mapper.Map<Company>(viewModel);
            record.Modifiedby = _executionContext.UserId;
            record.Modifieddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            if (_serviceProvider.GetRequiredService<ICompanyDA>().IsDuplicateCompany(viewModel.Id, viewModel.Name))
                return "exists";
            _serviceProvider.GetRequiredService<ICompanyDA>().UpdateCompany(record);
            return "true";
        }
        public List<CompanyCreditViewModel> GetCompanyCredits(int CompanyId)
        {
            var list = new List<CompanyCreditViewModel>();
            var results = _serviceProvider.GetRequiredService<ICompanyDA>().GetCompanycredits(CompanyId);
            foreach(var result in results)
            {
                list.Add(new CompanyCreditViewModel()
                {
                    Id = result.Id,
                    Companyid = result.Companyid,
                    CompanyName = result.Company.Name,
                    Credit = result.Credit,
                    Transactiontype = result.Transactiontype,
                    Referenceno = result.Referenceno,
                    Remarks = result.Remarks,
                    CreditDate = result.Createddate
                });
            }
            return list;
        }
        public bool AddCompanyCredit(CompanyCreditViewModel viewModel)
        {
            var record = _mapper.Map<Companycredit>(viewModel);
            record.Createdby = _executionContext.UserId;
            record.Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            _serviceProvider.GetRequiredService<ICompanyDA>().AddCompanyCredit(record);
            return true;
        }
        public int GetCompanyBalance(int companyId)
        {
            return _serviceProvider.GetRequiredService<ICompanyDA>().GetCompanyBalance(companyId);
        }
        public bool ReconcileCompanyCredit(int companyId)
        {
            _serviceProvider.GetRequiredService<ICompanyDA>().ReconcileCompanyCredit(companyId, true, false);
            return true;
        }
        public bool CheckCompanyBalance(int companyId)
        {
            int balance = GetCompanyBalance(companyId);
            if (balance > 0)
                return true;
            else
                return false;
        }
        public VerificationDetails GetReportData(int searchId, string clientname)
        {
            var verificationDetail = new VerificationDetails()
            {
                HrComments = new List<HRComments>()
            };

            var employeesearch = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeSearchById(searchId);
            var employeeapproval = _serviceProvider.GetRequiredService<IEmployeeDA>().GetLatestApproval(employeesearch.Employeeid.Value);            
            var employee = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeById(employeesearch.Employeeid.Value);
            verificationDetail = new VerificationDetails
            {
                Employer = employee.Customer.Name,
                CandidateName = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(employee.Name),
                Desigination = employee.Designation,
                DateOfJoining = employee.Fromdate.ToString("dd-MM-yyyy"),
                ReportingManagerName = employee.Reportingto,
                ReasonforLeaving = employee.Reasonforleaving,
                EmployeeCode = employee.Employeecode,
                Location = employee.Location,
                DateOfLeaving = employee.Todate.ToString("dd-MM-yyyy"),
                ReportingManagerDesigination = employee.Managerdesignation,
                LastSalary = employee.Lastdrawnsalary,
                HRName = ((employeeapproval != null && employeeapproval.ApprovedbyNavigation != null) ? employeeapproval.ApprovedbyNavigation.Displayname : "-"),
                HRDesigination = ((employeeapproval != null && employeeapproval.ApprovedbyNavigation != null) ? employeeapproval.ApprovedbyNavigation.Designation : "-"),
                HREmailId = ((employeeapproval != null && employeeapproval.ApprovedbyNavigation != null) ? employeeapproval.ApprovedbyNavigation.Emailid : "-"),
                VerificationFacilatedBy = "",
                ReportGeneratedOn = DateTime.Now.ToString("dd-MM-yyyy"),
                DateOfVerification = ((employeeapproval != null && employeeapproval.ApprovedbyNavigation != null) ? employeeapproval.Approveddate.Value.ToString("dd/MM/yyyy") : "-")
            };
            var questions = _serviceProvider.GetRequiredService<IEmployeeDA>().GetAllEmployeeQuestions(employee.Employeecode, employee.Customerid);
            if (questions != null || questions.Count > 0)
                verificationDetail.HrComments = questions.Select(x => new HRComments
                {
                    Questions = x.Question.Question,
                    Answers = x.Answer
                }).ToList();

            string fileName = $"Employment_Verification_Report_{verificationDetail.CandidateName}_{DateTime.Now.ToString("yyyy-dd-MM-HH-mm-ss")}.pdf";
            _serviceProvider.GetRequiredService<ICustomerDA>().ReconcileCustomerCredit(_executionContext.CustomerId, _executionContext.UserId);
            //save the report details to the respective tables
            _serviceProvider.GetRequiredService<IEmployeeDA>().UpdateReportLink(searchId, fileName, _executionContext.CompanyId, _executionContext.CustomerId, clientname);
            return verificationDetail;
        }
        public bool GenerateReportBySearchId(int searchid, string clientname)
        {
            
            var employeesearch = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeSearchById(searchid);
            int empId = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeIdByCode(employeesearch.Employeecode);

            return false;
        }
        public VerificationDetails GenerateReportByRequestId(int id)
        {
            var request = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmpverificationrequestById(id);
            //byte[] reportdata = null;
            var verificationDetail = new VerificationDetails()
            {
                HrComments = new List<HRComments>()
            };
            if (request.Requeststatus == "Approved")
            {                
                var employee = _serviceProvider.GetRequiredService<IEmployeeDA>().GetEmployeeById(request.Employeeid.Value);
                var employeeapproval = _serviceProvider.GetRequiredService<IEmployeeDA>().GetLatestApproval(request.Employeeid.Value);
                verificationDetail = new VerificationDetails
                {
                    Employer = employee.Customer.Name,
                    CandidateName = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(employee.Name),
                    Desigination = employee.Designation,
                    DateOfJoining = employee.Fromdate.ToString("dd-MM-yyyy"),
                    ReportingManagerName = employee.Reportingto,
                    ReasonforLeaving = employee.Reasonforleaving,
                    EmployeeCode = employee.Employeecode,
                    Location = employee.Location,
                    DateOfLeaving = employee.Todate.ToString("dd-MM-yyyy"),
                    ReportingManagerDesigination = employee.Managerdesignation,
                    LastSalary = employee.Lastdrawnsalary,
                    HRName = ((employeeapproval != null && employeeapproval.ApprovedbyNavigation != null) ? employeeapproval.ApprovedbyNavigation.Displayname : "-"),
                    HRDesigination = ((employeeapproval != null && employeeapproval.ApprovedbyNavigation != null) ? employeeapproval.ApprovedbyNavigation.Designation : "-"),
                    HREmailId = ((employeeapproval != null && employeeapproval.ApprovedbyNavigation != null) ? employeeapproval.ApprovedbyNavigation.Emailid : "-"),
                    VerificationFacilatedBy = "",
                    ReportGeneratedOn = DateTime.Now.ToString("dd-MM-yyyy"),
                    DateOfVerification = ((employeeapproval != null && employeeapproval.ApprovedbyNavigation != null) ? employeeapproval.Approveddate.Value.ToString("dd/MM/yyyy") : "-"),
                    Status = request.Requeststatus,
                };
                var questions = _serviceProvider.GetRequiredService<IEmployeeDA>().GetAllEmployeeQuestions(employee.Employeecode, employee.Customerid);
                if (questions != null || questions.Count > 0)
                    verificationDetail.HrComments = questions.Select(x => new HRComments
                    {
                        Questions = x.Question.Question,
                        Answers = x.Answer
                    }).ToList();
                //reportdata = _serviceProvider.GetRequiredService<FastReportService>().GeneratedEmployerReport(verificationDetail, hRComments);
            }
            else if (request.Requeststatus == "Rejected")
            {
                var invalidemployee = _serviceProvider.GetRequiredService<IEmployeeDA>().GetInvalidemployee(request.Invalidemployeeid.Value);
                verificationDetail = new VerificationDetails
                {
                    Employer = invalidemployee.Customer.Name,
                    CandidateName = invalidemployee.Name,
                    Desigination = invalidemployee.Designation,
                    DateOfJoining = invalidemployee.Fromdate.Value.ToString("dd-MM-yyyy"),
                    ReportingManagerName = invalidemployee.Reportingto,
                    ReasonforLeaving = invalidemployee.Reasonforleaving,
                    EmployeeCode = invalidemployee.Employeecode,
                    Location = invalidemployee.Location,
                    DateOfLeaving = invalidemployee.Todate.Value.ToString("dd-MM-yyyy"),
                    ReportingManagerDesigination = invalidemployee.Managerdesignation,
                    LastSalary = invalidemployee.Lastdrawnsalary,
                    Comments = invalidemployee.Comments,
                    HRName = invalidemployee.CreatedbyNavigation.Displayname,
                    HRDesigination = invalidemployee.CreatedbyNavigation.Designation,
                    HREmailId = invalidemployee.CreatedbyNavigation.Emailid,
                    VerificationFacilatedBy = "",
                    ReportGeneratedOn = DateTime.Now.ToString("dd-MM-yyyy"),
                    DateOfVerification = invalidemployee.Createddate.ToString("dd-MM-yyyy"),
                    Status = request.Requeststatus,
                };
                //reportdata = _serviceProvider.GetRequiredService<FastReportService>().GeneratedNegativeReport(nonverificationDetail);
            }
            string fileName = $"Employment_Verification_Report_{verificationDetail.CandidateName}_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}.pdf";
            
            _serviceProvider.GetRequiredService<IEmployeeDA>().UpdateEmpVerificationRequest(request.Id, 0, "Generated", fileName);
            return verificationDetail;
        }
        
        public bool GenerateStudentReport(int stuId, int searchId, string source)
        {            
            var studentsearch = _serviceProvider.GetRequiredService<IStudentDA>().GetStudentSearchById(searchId);
            if(stuId == 0 && studentsearch.Searchresult == "F")
            {
                stuId = studentsearch.Studentkey.Value;
            }
            var studentapproval = _serviceProvider.GetRequiredService<IStudentDA>().GetLatestApproval(stuId);
            var verificationDetail = new EducationDetail();
            var nonverificationDetail = new NonEducationDetail();
            byte[] reportdata;
            if (stuId > 0)
            {
                var student = _serviceProvider.GetRequiredService<IStudentDA>().GetStudentById(stuId);

                //validate point
                bool isvalidBalance = false;
                if (source == "company")
                    isvalidBalance = this.CheckCompanyBalance(_executionContext.CompanyId);
                else if (source == "customer")
                    isvalidBalance = (_serviceProvider.GetRequiredService<ICustomerService>().GetCustomerBalance(_executionContext.CustomerId) > 0);
                if (!isvalidBalance) return false;
                verificationDetail = new EducationDetail()
                {
                    StudentName = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(student.Studentname),
                    StudentId = student.Studentid,
                    InstituteName = student.Customer.Name,
                    University = student.University,
                    DegreeName = student.Degreetype,
                    MajorSubject = student.Majorsubject,
                    PeriodFrom = student.Periodfrom,
                    PeriodTo = student.Periodto,
                    PassYear = student.Passyear,
                    MarksObtained = student.Marksobtained,
                    StudyMode = student.Studymode,
                    AttainDegree = student.EligibleAttainDegree,
                    VerifierName = ((studentapproval != null && studentapproval.ApprovedbyNavigation != null) ? studentapproval.ApprovedbyNavigation.Displayname : "-"),
                    VerifierDesignation = ((studentapproval != null && studentapproval.ApprovedbyNavigation != null) ? studentapproval.ApprovedbyNavigation.Designation : "-"),
                    VerifierEmail = ((studentapproval != null && studentapproval.ApprovedbyNavigation != null) ? studentapproval.ApprovedbyNavigation.Emailid : "-"),
                    VerificationDate = ((studentapproval != null && studentapproval.ApprovedbyNavigation != null) ? studentapproval.Approveddate.Value.ToString("dd/MM/yyyy") : "-")
                };
                
                //reportdata = _serviceProvider.GetRequiredService<FastReportService>().GeneratedEducationReport(verificationDetail);
            }
            else
            {
                var request = _serviceProvider.GetRequiredService<IStudentDA>().GetStudentrequestBySearch(searchId);
                // Generate blank employee report (Employee not found for search criteria)
                nonverificationDetail = new NonEducationDetail()
                {
                    StudentId = request.Regno,
                    InstituteName = request.Customer.Name,
                    ReplyComments = request.Replycomments,
                    VerifierName = request.RepliedbyNavigation.Displayname,
                    VerifierDesignation = request.RepliedbyNavigation.Designation,
                    VerifierEmail = request.RepliedbyNavigation.Emailid
                };
                //reportdata = _serviceProvider.GetRequiredService<FastReportService>().GeneratedNegativeEducationReport(nonverificationDetail);
            }

            string fileName = $"{studentsearch.Searchrequestid}_{DateTime.Now.ToString("yyyy-dd-MM-HH-mm-ss")}.pdf";
            string reportpath = $"{_constants.Reports}{fileName}";
            //System.IO.File.WriteAllBytes(reportpath, reportdata);
            if (source == "company")
                _serviceProvider.GetRequiredService<ICompanyDA>().ReconcileCompanyCredit(_executionContext.CompanyId, false, true);
            else if (source == "customer")
                _serviceProvider.GetRequiredService<ICustomerDA>().ReconcileCustomerCredit(_executionContext.CustomerId, _executionContext.UserId);
            //save the file name to the respective tables
            _serviceProvider.GetRequiredService<IStudentDA>().UpdateReportLink(searchId, stuId, fileName, _executionContext.CompanyId, _executionContext.CustomerId);
            return true;
        }
    }
}
