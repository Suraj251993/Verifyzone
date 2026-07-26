using OrgCheck.DataAccess.Interfaces;
using OrgCheck.Models;
using OrgCheck.Services.Interfaces;
using OrgCheck.ViewModels;
using System.Collections.Generic;
using System.Globalization;
using System;
using OrgCheck.Middleware;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrgCheck.DataAccess;

namespace OrgCheck.Services
{
    public class StudentService : IStudentService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ExecutionContext _executionContext;
        private readonly EmailService _emailService;
        private readonly IMapper _mapper;
        private readonly Constants _constants;
        public StudentService(IServiceProvider serviceProvider, ExecutionContext executionContext, IMapper mapper,
            EmailService emailService, Constants constants)
        {
            _serviceProvider = serviceProvider;
            _executionContext = executionContext;
            _mapper = mapper;
            _emailService = emailService;
            _constants = constants;
        }

        public string AddStudent(StudentViewModel viewModel, bool isApproved = true)
        {
            viewModel.Id = "0";
            viewModel.Customerid = _executionContext.CustomerId;

            var record = _mapper.Map<Student>(viewModel);
            record.Studentname = _serviceProvider.GetRequiredService<CryptoService>().Encrypt(viewModel.Studentname);
            record.Isapproved = isApproved;
            record.Createdby = _executionContext.UserId;
            record.Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            
            _serviceProvider.GetRequiredService<IStudentDA>().AddStudent(record);
            return "true";
        }
        public List<StudentViewModel> ViewStudents(string studentId, int customerId)
        {
            var record = _serviceProvider.GetRequiredService<IStudentDA>().ViewStudent(studentId, customerId);
            var result = new List<StudentViewModel>();
            if (record != null && record.Count > 0)
            {
                foreach(var student in record)
                {
                    result.Add(new StudentViewModel()
                    {
                        Id = student.Id.ToString(),
                        Studentname = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(student.Studentname),
                        Studentid=student.Studentid,
                        Degreetype=student.Degreetype,
                        Majorsubject = student.Majorsubject,
                        Marksobtained = student.Marksobtained,
                        Periodfrom = student.Periodfrom,
                        Periodto = student.Periodto,
                        EducationPeriod = $"{student.Periodfrom} - {student.Periodto}",
                        Passyear = student.Passyear,
                        University = student.University,
                    });
                }
            }
            
            return result;
        }
        public StudentViewModel ViewStudent(int id, bool addSearch)
        {
            var result = _serviceProvider.GetRequiredService<IStudentDA>().ViewStudentById(id);
            var viewmodel = _mapper.Map<StudentViewModel>(result);
            viewmodel.Studentname = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(result.Studentname);
            var search = new Studentsearch();
            if (addSearch)
                search = new Studentsearch()
                {
                    Searchrequestid = _serviceProvider.GetRequiredService<IStudentDA>().GenerateSearchRequestNumber(),
                    Studentid = result.Studentid,
                    Customerid = result.Customerid,
                    Createdby = _executionContext.UserId,
                    Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                    Finalresult = "Searched",
                    Transactionamount = 0.0,
                    Reportlink = "",
                    Studentkey = id,
                    Status = 1
                };
            if (result != null)
            {
                var approval = _serviceProvider.GetRequiredService<IStudentDA>().GetLatestApproval(result.Id);
                if (approval != null)
                {
                    viewmodel.AuthorizedBy = approval.ApprovedbyNavigation?.Displayname + " (" + approval.ApprovedbyNavigation?.Emailid + ")";
                    viewmodel.AuthorizedDate = approval.Approveddate?.ToString("dd/MM/yyyy");
                }
                //search.Name = record.Name;
                search.Searchresult = "F";                
            }
            else
            {
                viewmodel = new StudentViewModel();
                search.Searchresult = "N";
                //var customer = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomer(customerId);
                //string emailBody = $"{Environment.NewLine}{Environment.NewLine}The employee record {empCode} in {customer.Name} was not found";
                //string emailSubject = "VerifyZone - No record found";
                //string emailTo = _constants.NoRecordNotificationEmail;
                //_emailService.SendEmail(emailTo, string.Empty, string.Empty, emailSubject, emailBody);
            }

            if (addSearch)
                viewmodel.SearchId = _serviceProvider.GetRequiredService<IStudentDA>().AddStudentSearch(search);
            return viewmodel;
        }
        public StudentViewModel ViewStudentById(int id)
        {
            var result = _serviceProvider.GetRequiredService<IStudentDA>().ViewStudentById(id);
            var approval = _serviceProvider.GetRequiredService<IStudentDA>().GetLatestApproval(id);
            var viewmodel = _mapper.Map<StudentViewModel>(result);
            viewmodel.Studentname = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(result.Studentname);
            viewmodel.AuthorizedDate = approval.Approveddate.Value.ToString("dd/MM/yyyy");
            viewmodel.AuthorizedBy = approval.ApprovedbyNavigation.Displayname;
            return viewmodel;
        }
        public int AddStudentSearch(StudentSearchViewModel viewModel)
        {
            var search = new Studentsearch()
            {
                Searchrequestid = _serviceProvider.GetRequiredService<IStudentDA>().GenerateSearchRequestNumber(),
                Studentid = viewModel.StudentId,
                Customerid = viewModel.Customerid,
                Createdby = _executionContext.UserId,
                Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Searchresult = viewModel.Searchresult,
                Finalresult = "Searched",
                Transactionamount = 0.0,
                Reportlink = "",
                Status = 1
            };
            return _serviceProvider.GetRequiredService<IStudentDA>().AddStudentSearch(search);
        }
        public StudentSearchViewModel GetStudentSearch(int searchId)
        {
            var data = _serviceProvider.GetRequiredService<IStudentDA>().GetStudentSearchById(searchId);

            var result = new StudentSearchViewModel()
            {
                Searchrequestid = data.Searchrequestid,
                Customerid = data.Customerid,
                CustomerName = data.Customer.Name,
                Finalresult = data.Finalresult,
                StudentId = data.Studentid,
                Reportlink = data.Reportlink,
                Reportdate = data.Reportdate,
                Transactionamount = data.Transactionamount
            };
            return result;
        }
        public List<StudentSearchViewModel> GetStudentSearchHistory(DateTime fromDate, DateTime toDate, string finalResult)
        {
            var list = _serviceProvider.GetRequiredService<IStudentDA>().GetStudentSearch(fromDate, toDate, _executionContext.UserId, finalResult);
            var result = new List<StudentSearchViewModel>();
            foreach (var record in list)
            {
                var entry = new StudentSearchViewModel()
                {
                    Searchrequestid = record.Searchrequestid,
                    Customerid = record.Customerid,
                    //Name = (!string.IsNullOrEmpty(record.Name) ? _serviceProvider.GetRequiredService<CryptoService>().Decrypt(record.Name) : ""),
                    StudentId = record.Studentid,
                    Searchresult = (record.Searchresult == "F" ? "Record found" : "Record not found"),
                    Id = record.Id,
                    Finalresult = record.Finalresult,
                    Reportdate = record.Reportdate,
                    Reportlink = record.Reportlink,
                    CustomerName = (record.Customer != null ? record.Customer.Name : ""),
                    Transactionamount = record.Transactionamount,
                    CreatedbyName = record.CreatedbyNavigation.Displayname,
                    Createddate = record.Createddate.ToString("dd/MM/yyyy"),
                    ApprovedDate = ""
                };
                switch (entry.Finalresult)
                {
                    case "Searched":
                    case "Approved":
                        entry.ActionStatus = "2";
                        break;
                    case "Generated":
                        entry.ActionStatus = "1";
                        break;
                    default:
                        entry.ActionStatus = "0";
                        break;
                }
                if (record.Searchresult == "F")
                {
                    var maxApprovedEntry = _serviceProvider.GetRequiredService<IStudentDA>().GetLatestApproval(record.Studentkey.Value);
                    if (maxApprovedEntry != null && maxApprovedEntry.Approveddate.HasValue)
                        entry.ApprovedDate = maxApprovedEntry.Approveddate.Value.ToString("dd/MM/yyyy");
                }
                result.Add(entry);
            }
            return result;
        }

        public bool AddStudentApproval(int studentId, int searchId, bool isEdit)
        {
            var record = new Studentapproval()
            {
                Studentid = studentId,
                Requestedby = _executionContext.UserId,
                Requesteddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Studentsearchid = searchId,
                Isedit = isEdit
            };
            _serviceProvider.GetRequiredService<IStudentDA>().AddApproval(record);

            // Check for auto approval
            var custId = _serviceProvider.GetRequiredService<IStudentDA>().GetStudentById(studentId).Customerid;
            //var users = _serviceProvider.GetRequiredService<IUserDA>().GetUsersByCustomer(custId);
            var approvalsList = _serviceProvider.GetRequiredService<ICustomerDA>().GetAutoapprovalconfigsByCustomer(custId);
            var leastapproval = approvalsList.Where(_ => _.Enddate >= DateTime.Now.Date && _.Status == 1).OrderBy(_ => _.Id).Take(1).SingleOrDefault();
            if (leastapproval != null)
            {
                UpdateStudentApproval(record.Id, leastapproval.Createdby);
                return true;
            }

            // Update the corresponding status on employee search table
            var searchrecord = _serviceProvider.GetRequiredService<IStudentDA>().GetStudentSearchById(searchId);
            searchrecord.Finalresult = "Sent for approval";
            _serviceProvider.GetRequiredService<IStudentDA>().UpdateStudentSearch(searchrecord);
            if (record.Approvedby == null)
            {
                // Send email notification to the respective HRs (if multiple)                
                var hr_users = _serviceProvider.GetRequiredService<IUserDA>().GetUsersByCustomer(custId);
                foreach (var user in hr_users)
                {
                    string emailBody = $"Dear {user.Displayname},<br><br>You received a request for re-verify in VerifyZone portal";
                    emailBody += $"<br>Kindly login into the VerifyZone portal and re-verify.";
                    emailBody += $"<br><br>Thank you,<br>VerifyZone support team";
                    string emailSubject = "VerifyZone - Student re-verify";
                    string emailTo = user.Emailid;
                    _emailService.SendEmail(emailTo, string.Empty, string.Empty, emailSubject, emailBody);
                }
            }
            return true;
        }
        public bool UpdateStudentApproval(int id, int userId)
        {
            var record = new Studentapproval()
            {
                Id = id,
                Approvedby = userId,
                Approveddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
            };
            _serviceProvider.GetRequiredService<IStudentDA>().UpdateApproval(record);

            var approval = _serviceProvider.GetRequiredService<IStudentDA>().GetStudentApprovalById(id);

            // Update the corresponding status on employee search table
            var searchrecord = _serviceProvider.GetRequiredService<IStudentDA>().GetStudentSearchById(approval.Studentsearchid.Value);
            searchrecord.Finalresult = "Approved";
            _serviceProvider.GetRequiredService<IStudentDA>().UpdateStudentSearch(searchrecord);

            // Send email notification to the respective BGV user

            var user = _serviceProvider.GetRequiredService<IUserDA>().GetUser(approval.Requestedby.Value);
            string emailBody = $"Dear {user.Displayname},<br><br>Your request for student re-verify in VerifyZone portal was given.";
            emailBody += $"<br>Kindly login into the VerifyZone portal and proceed.";
            emailBody += $"<br><br>Thank you,<br>VerifyZone support team";
            string emailSubject = "VerifyZone - Student re-verify request given";
            string emailTo = user.Emailid;
            _emailService.SendEmail(emailTo, string.Empty, string.Empty, emailSubject, emailBody);
            return true;
        }
        public List<StudentApprovalViewModel> GetStudentApprovals(int customerId)
        {
            var list = _serviceProvider.GetRequiredService<IStudentDA>().GetStudentApprovals(customerId, false);
            return list.Select(_ => new StudentApprovalViewModel()
            {
                Id = _.Id.ToString(),
                StudentId = _.Studentid.ToString(),
                RegNo = _.Student.Studentid,
                StudentName = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(_.Student.Studentname),
                DegreeType = _.Student.Degreetype,
                MajorSubject = _.Student.Majorsubject,
                RequestedBy = _.RequestedbyNavigation.Displayname,
                RequestedOrganisation = _.RequestedbyNavigation.Customer.Name,
                RequestedDate = _.Requesteddate.Value.ToString("dd/MM/yyyy")
            }).ToList();
        }
        //public List<StudentSearchViewModel> GetGeneratedRecords(int month, int year, int companyId)
        //{
        //    var list = _serviceProvider.GetRequiredService<IStudentDA>().GetGeneratedReportsByCompanyMonth(month, year, companyId);
        //    var result = list.Select(_ => new StudentSearchViewModel()
        //    {
        //        Searchrequestid = _.Searchrequestid,
        //        CustomerName = _.Customer.Name,
        //        StudentId = _.Studentid,
        //        Reportdate = _.Reportdate.Value,
        //        CreatedbyName = _.CreatedbyNavigation.Loginname
        //    }).OrderBy(x => x.Reportdate).ToList();

        //    return result;
        //}

        public string Validate(IFormFile file, string strSixDigitNumber)
        {
            string _result = string.Empty;
            if (file.Length == 0)
                _result = "Corrupted file";
            else
            {
                var info = new FileInfo(file.FileName);
                if (info.Extension != ".csv" && !file.ContentType.Contains("excel"))
                    _result = "Invalid file. Only csv file is allowed to upload";
                else
                {
                    var filePath = $"{_constants.UploadPath}{Path.GetFileNameWithoutExtension(file.FileName)}_{strSixDigitNumber}.csv";
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
        public List<ErrorLogViewModel> ValidateStudent(StudentViewModel viewModel)
        {
            var result = new List<ErrorLogViewModel>();
            if (string.IsNullOrEmpty(viewModel.Studentid))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Studentname,
                    errordescription = "StudentId is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Studentname))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Studentid,
                    errordescription = "Student name is empty"
                });
            else if (viewModel.Studentname.Trim().Length <= 2)
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Studentid + ", " + viewModel.Studentname,
                    errordescription = "Student name is invalid"
                });
            if (string.IsNullOrEmpty(viewModel.Degreetype))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Studentid + ", " + viewModel.Studentname,
                    errordescription = "Degreetype is empty"
                });
            if (string.IsNullOrEmpty(viewModel.University))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Studentid + ", " + viewModel.Studentname,
                    errordescription = "University is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Majorsubject))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Studentid + ", " + viewModel.Studentname,
                    errordescription = "Major subject is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Periodfrom))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Studentid + ", " + viewModel.Studentname,
                    errordescription = "'Period from' is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Periodto))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Studentid + ", " + viewModel.Studentname,
                    errordescription = "'Period to' is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Passyear))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Studentid + ", " + viewModel.Studentname,
                    errordescription = "Pass year is empty"
                });
            if (string.IsNullOrEmpty(viewModel.Marksobtained))
                result.Add(new ErrorLogViewModel()
                {
                    errorcode = viewModel.Studentid + ", " + viewModel.Studentname,
                    errordescription = "Marks obtained drawn is empty"
                });
            
            return result;
        }
        public UploadSummaryViewModel ParseFile(IFormFile file, string strSixDigitNumber, int customerId, int userId)
        {
            var resultModel = new UploadSummaryViewModel();
            resultModel.errors = new List<ErrorLogViewModel>();
            var validdatalist = new List<StudentViewModel>();
            string filePath = $"{_constants.UploadPath}{Path.GetFileNameWithoutExtension(file.FileName)}_{strSixDigitNumber}.csv";
            //var existingEmployeeLists = _serviceProvider.GetRequiredService<IEmployeeDA>().GetAllEmployees(customerId);

            var Lines = System.IO.File.ReadLines(filePath).ToList();
            int lineCount = 1;
            var currentRow = Lines[lineCount];
            while (currentRow != "")
            {
                var values = currentRow.Split(',');
                var dr = new StudentViewModel()
                {
                    Studentname = values[0].Trim(),
                    Studentid = values[1].Trim(),
                    University = values[2].Trim(),
                    Degreetype = values[3].Trim(),
                    Majorsubject = values[4].Trim(),
                    Periodfrom = values[5].Trim(),
                    Periodto = values[6].Trim(),
                    Passyear = values[7].Trim(),
                    Marksobtained = values[8].Trim(),
                    StudyMode = values[9].Trim(),
                    EligibleAttainDegree = values[10].Trim(),
                    Comments = values[11].Trim()
                };

                var errors = ValidateStudent(dr);
                if (errors.Count == 0)
                {
                    resultModel.validrecords++;
                    validdatalist.Add(dr);
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

            var customer = _serviceProvider.GetRequiredService<ICustomerService>().GetCustomer(customerId);
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
            var studentlists = new Tempstudent[validdatalist.Count];
            int listindex = 0;
            foreach (var item in validdatalist)
            {
                var record = new Tempstudent()
                {
                    Studentname = _serviceProvider.GetRequiredService<CryptoService>().Encrypt(item.Studentname),
                    Studentid = item.Studentid,
                    Institutionname = customer.Name,
                    University = item.University,
                    Customerid = customerId,
                    Degreetype = item.Degreetype,
                    Majorsubject = item.Majorsubject,
                    Periodfrom = item.Periodfrom,
                    Periodto = item.Periodto,
                    Passyear = item.Passyear,
                    Marksobtained = item.Marksobtained,
                    Comments = item.Comments ?? "",
                    Studymode = item.StudyMode,
                    EligibleAttainDegree = item.EligibleAttainDegree,
                    Fileid = newfile.Id,
                    Createdby = userId,
                    Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                };
                studentlists[listindex] = record;
                listindex++;
            }
            _serviceProvider.GetRequiredService<IStudentDA>().AddTempStudents(studentlists);
            return resultModel;
        }
        public List<StudentViewModel> GetTempstudents(int fileId)
        {
            var results = new List<StudentViewModel>();
            var list = _serviceProvider.GetRequiredService<IStudentDA>().GetTempstudents(fileId);
            foreach (var item in list)
            {
                var empViewModel = new StudentViewModel()
                {
                    Id = item.Id.ToString(),
                    Studentname = item.Studentname,
                    Studentid = item.Studentid,
                    University = item.University,
                    Degreetype = item.Degreetype,
                    Majorsubject = item.Majorsubject,
                    Periodfrom = item.Periodfrom,
                    Periodto = item.Periodto,
                    Passyear = item.Passyear,
                    Marksobtained = item.Marksobtained,
                    Comments = item.Comments,
                    StudyMode = item.Studymode,
                    EligibleAttainDegree = item.EligibleAttainDegree,
                    Customerid = item.Customerid.GetValueOrDefault()
                };
                
                results.Add(empViewModel);
            }
            return results;
        }
        public bool ApproveFile(int fileId, int userId)
        {
            var records = GetTempstudents(fileId);
            if (userId == 0)
            {
                var record = _serviceProvider.GetRequiredService<IStudentDA>().GetTempstudents(fileId).FirstOrDefault();
                userId = record.Createdby;
            }
            var emplists = new Student[records.Count];
            int index = 0;
            var file = new OrgCheck.Models.File()
            {
                Id = fileId,
                Uploadedstatus = 2,
            };
            foreach (var record in records)
            {
                var student = new Student()
                {
                    Studentname = record.Studentname,
                    Customerid = record.Customerid,
                    Studentid = record.Studentid,
                    University = record.University,
                    Degreetype = record.Degreetype,
                    Majorsubject = record.Majorsubject,
                    Periodfrom = record.Periodfrom,
                    Periodto = record.Periodto,
                    Passyear = record.Passyear,
                    Marksobtained = record.Marksobtained,
                    Studymode = record.StudyMode,
                    EligibleAttainDegree = record.EligibleAttainDegree,
                    Comments = record.Comments,
                    Isapproved = true,
                    Createdby = userId,
                    Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                };

                emplists[index] = student;
                index++;
            }
            _serviceProvider.GetRequiredService<IStudentDA>().AddBulkStudent(emplists);
            _serviceProvider.GetRequiredService<IFileDA>().UpdateFile(file);
            _serviceProvider.GetRequiredService<IStudentDA>().DeleteTempStudents(fileId);
            return true;
        }
        public bool RejectFile(int fileId)
        {
            var records = GetTempstudents(fileId);
            var file = new OrgCheck.Models.File()
            {
                Id = fileId,
                Uploadedstatus = 3
            };
            _serviceProvider.GetRequiredService<IFileDA>().UpdateFile(file);
            _serviceProvider.GetRequiredService<IStudentDA>().DeleteTempStudents(fileId);
            return true;
        }

        public CustomerDashboardCount GetDashboardCount(int month, int year)
        {
            var dashboardCount = new CustomerDashboardCount();
            dashboardCount.CompletedCount = _serviceProvider.GetRequiredService<IStudentDA>().GetApprovedData(month, year, _executionContext.CustomerId).Count;
            dashboardCount.ApprovalCount = _serviceProvider.GetRequiredService<IStudentDA>().GetStudentApprovals(_executionContext.CustomerId, false).Count;
            dashboardCount.DownloadCount = _serviceProvider.GetRequiredService<IStudentDA>().GetGeneratedReportsByCustomerMonth(month, year, _executionContext.CustomerId).Count;
            dashboardCount.RequestCount = GetOpenRequests(_executionContext.CustomerId).Count;
            return dashboardCount;
        }

        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetLookupStuVerificationResponses()
        {
            return _serviceProvider.GetRequiredService<IStudentDA>().GetAllStudentVerificationResponses().Select(_ => new SelectListItem()
            {
                Text = _.Name,
                Value = _.Id.ToString(),
                Selected = false
            }).ToList();
        }
        public bool AddStudentRequest(StudentRequestViewModel viewModel)
        {
            if(viewModel.Searchid == 0)
            {
                var search = new Studentsearch()
                {
                    Searchrequestid = _serviceProvider.GetRequiredService<IStudentDA>().GenerateSearchRequestNumber(),
                    Studentid = viewModel.Regno,
                    Customerid = viewModel.Customerid,
                    Createdby = _executionContext.UserId,
                    Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                    Searchresult = "N",
                    Finalresult = "Searched",
                    Transactionamount = 0.0,
                    Reportlink = "",
                    Status = 1
                };
                viewModel.Searchid = _serviceProvider.GetRequiredService<IStudentDA>().AddStudentSearch(search);
            }
            var record = new Studentrequest()
            {
                Customerid = viewModel.Customerid,
                Searchid = viewModel.Searchid,
                Regno = viewModel.Regno,
                Requestcomments = viewModel.Requestcomments,
                Raisedby = _executionContext.UserId,
                Raiseddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Status = 1
            };
            _serviceProvider.GetRequiredService<IStudentDA>().AddStudentRequest(record);

            return true;
        }
        public bool UpdateStudentRequest(StudentRequestViewModel viewModel)
        {
            var record = new Studentrequest()
            {
                Id = viewModel.Id,
                Responsetype = Convert.ToInt32(viewModel.ResponseType),
                Replycomments = viewModel.Replycomments,
                Repliedby = _executionContext.UserId,
                Replieddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
            };
            _serviceProvider.GetRequiredService<IStudentDA>().UpdateStudentRequest(record);
            return true;
        }
        public List<StudentRequestViewModel> GetOpenRequests(int customerId)
        {
            var returnlist = new List<StudentRequestViewModel>();
            var list = _serviceProvider.GetRequiredService<IStudentDA>().GetOpenRequestsByCustomer(customerId);
            foreach (var item in list)
            {
                returnlist.Add(new StudentRequestViewModel()
                {
                    Id = item.Id,
                    Customerid = item.Customerid,
                    Customername = item.Customer.Name,
                    Regno = item.Regno,
                    Requestcomments = item.Requestcomments,
                    Raisedby = item.Raisedby,
                    RaisedByName = item.RaisedbyNavigation.Displayname + " (" + item.RaisedbyNavigation.Customer.Name + ")",
                    Raiseddate = item.Raiseddate
                });
            }
            return returnlist;
        }
        public StudentRequestViewModel GetRequestById(int id)
        {
            var result = _serviceProvider.GetRequiredService<IStudentDA>().GetStudentrequestById(id);
            return new StudentRequestViewModel()
            {
                Id = result.Id,
                Regno = result.Regno,
                Requestcomments = result.Requestcomments,
                Raisedby = result.Raisedby,
                Raiseddate = result.Raiseddate
            };
        }
        //public List<StudentRequestViewModel> GetStudentRequestByCompany(int companyId, bool isOpenOnly, bool isRepliedOnly)
        //{
        //    var returnlist = new List<StudentRequestViewModel>();
        //    var list = _serviceProvider.GetRequiredService<IStudentDA>().GetStudentrequestsByCompany(companyId, isOpenOnly, isRepliedOnly);
        //    foreach (var item in list)
        //    {
        //        returnlist.Add(new StudentRequestViewModel()
        //        {
        //            //Id = item.Id,
        //            //Customerid = item.Customerid,
        //            Customername = item.Customer.Name,
        //            Regno = item.Regno,
        //            Requestcomments = item.Requestcomments,
        //            Replycomments = string.IsNullOrEmpty(item.Replycomments) ? "" : item.Replycomments,
        //            //Raisedby = item.Raisedby,
        //            RaisedByName = item.RaisedbyNavigation.Displayname,
        //            Raiseddate = item.Raiseddate,
        //            //Repliedby = item.Repliedby.Value,
        //            ReplierName = (item.Repliedby == null ? "" : item.RepliedbyNavigation.Displayname + " (" + item.RepliedbyNavigation.Designation + ")"),
        //            Replieddate = item.Replieddate,
        //        });
        //    }
        //    return returnlist;
        //}
        public List<StudentRequestViewModel> GetStudentRequestByCustomer(int customerId, bool isOpenOnly, bool isRepliedOnly)
        {
            var returnlist = new List<StudentRequestViewModel>();
            var list = _serviceProvider.GetRequiredService<IStudentDA>().GetStudentrequestsByCustomer(customerId, isOpenOnly, isRepliedOnly);
            foreach (var item in list)
            {
                returnlist.Add(new StudentRequestViewModel()
                {
                    //Id = item.Id,
                    //Customerid = item.Customerid,
                    Customername = item.Customer.Name,
                    Regno = item.Regno,
                    Requestcomments = item.Requestcomments,
                    Replycomments = string.IsNullOrEmpty(item.Replycomments) ? "" : item.Replycomments,
                    //Raisedby = item.Raisedby,
                    RaisedByName = item.RaisedbyNavigation.Displayname,
                    Raiseddate = item.Raiseddate,
                    //Repliedby = item.Repliedby.Value,
                    ReplierName = (item.Repliedby == null ? "" : item.RepliedbyNavigation.Displayname + " (" + item.RepliedbyNavigation.Designation + ")"),
                    Replieddate = item.Replieddate,
                });
            }
            return returnlist;
        }
    }
}
