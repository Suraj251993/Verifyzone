using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrgCheck.DataAccess.Interfaces;
using OrgCheck.Models;
using OrgCheck.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OrgCheck.DataAccess
{
    public class EmployeeDA : IEmployeeDA
    {
        private readonly IServiceProvider _serviceProvider;
        public PostgresContext orgCheckContext;
        public EmployeeDA(PostgresContext _orgCheckContext, IServiceProvider serviceProvider)
        {
            orgCheckContext = _orgCheckContext;
            _serviceProvider = serviceProvider;
        }
        public Employee AddEmployee(Employee employee, List<Employeequestionaire> questionaries)
        {
            orgCheckContext.Employees.Add(employee);
            orgCheckContext.SaveChanges();
            foreach(var data in questionaries)
            {
                var record = new Employeequestionaire()
                {
                    Employeeid = employee.Id,
                    Questionid = data.Questionid,
                    Answer = data.Answer,
                    Status = 1
                };
                orgCheckContext.Employeequestionaires.Add(record);
            }
            if (employee.Isapproved)
            {
                // Add default entry
                var record = new Employeeapproval()
                {
                    Employeeid = employee.Id,
                    Approvedby = employee.Createdby,
                    Approveddate = employee.Createddate,
                    Isedit = employee.Isedit
                };
                orgCheckContext.Employeeapprovals.Add(record);
            }
            orgCheckContext.SaveChanges();
            return employee;
        }
        public void AddBulkEmployee(Employee[] employees)
        {
            orgCheckContext.Employees.AddRange(employees);
            orgCheckContext.SaveChanges();
        }
        public void AddApproval(Employeeapproval approval)
        {
            orgCheckContext.Employeeapprovals.Add(approval);
            orgCheckContext.SaveChanges();
        }

        public void UpdateApproval(Employeeapproval approval)
        {
            var record = orgCheckContext.Employeeapprovals.FirstOrDefault(_ => _.Id == approval.Id);
            if (record != null)
            {
                record.Approvedby = approval.Approvedby;
                record.Approveddate = approval.Approveddate;
                orgCheckContext.SaveChanges();
            }
        }
        public List<Employeeapproval> GetEmployeeApprovals(int customerId, bool isEdit)
        {
            return orgCheckContext.Employeeapprovals.Include(x => x.Employee).Include(x => x.RequestedbyNavigation)
                .AsNoTracking()
                .Where(x => x.Employee.Customerid == customerId && x.Requestedby != null && x.Approvedby == null && x.Isedit == isEdit)
                .OrderByDescending(x => x.Id)
                .ToList();
        }
        public List<Employeeapproval> GetApprovalGiven(int month, int year, int userId)
        {
            var query = orgCheckContext.Employeeapprovals.Include(x => x.Employee).Include(x => x.RequestedbyNavigation)
                .AsNoTracking()
                .Where(_ => _.Approvedby.Value == userId
                && _.Approveddate.Value.Year == year);
            if (month > 0)
                query = query.Where(_ => _.Approveddate.Value.Month == month);
            if (userId > 0)
                query = query.Where(_ => _.Approvedby.Value == userId);
            return query.ToList();
        }
        public int GetApprovalGivenCount(int month, int year, int userId)
        {
            var query = orgCheckContext.Employeeapprovals.AsNoTracking()
                .Where(_ => _.Approvedby.Value == userId
                && _.Approveddate.Value.Year == year);
            if (month > 0)
                query = query.Where(_ => _.Approveddate.Value.Month == month);
            if (userId > 0)
                query = query.Where(_ => _.Approvedby.Value == userId);
            return query.ToList().Count;
        }
        public Employeeapproval GetEmployeeApprovalById(int id)
        {
            return orgCheckContext.Employeeapprovals.AsNoTracking().Where(x => x.Id == id).FirstOrDefault();
        }
        public Employeeapproval GetEmployeeApprovalByEmployeeId(int id)
        {
            return orgCheckContext.Employeeapprovals.AsNoTracking().Where(x => x.Employeeid == id && x.Approvedby == null).FirstOrDefault();
        }
        public Employee ViewEmployee(string empCode, int customerId, string lastworkingDate)
        {
            var query = orgCheckContext.Employees.AsNoTracking().Where(_ => _.Employeecode == empCode && _.Customerid == customerId);
            if (!string.IsNullOrEmpty(lastworkingDate))
            {
                DateTime dtLastWorkingdate= DateTime.ParseExact(lastworkingDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                query = query.Where(_ => _.Todate == DateTime.SpecifyKind(dtLastWorkingdate, DateTimeKind.Unspecified));
            }
            return query.FirstOrDefault();
        }
        public List<Employee> GetAllEmployees(int customerId)
        {
            return orgCheckContext.Employees.AsNoTracking().Where(_ => _.Customerid == customerId).ToList();
        }
        public Employeeapproval GetLatestApproval(int employeeId)
        {
            return orgCheckContext.Employeeapprovals
                .Include(x => x.ApprovedbyNavigation).AsNoTracking()
                .Where(_ => _.Employeeid == employeeId && _.Approveddate != null).OrderByDescending(_ => _.Id).Take(1).FirstOrDefault();
        }
        public List<Employeeapproval> GetApprovedData(DateTime fromDate, DateTime toDate, int userId)
        {
            return orgCheckContext.Employeeapprovals.Include(x => x.Employee).AsNoTracking()
                .Where(_ => _.Approveddate.Value >= DateTime.SpecifyKind(fromDate, DateTimeKind.Utc) && _.Approveddate.Value < DateTime.SpecifyKind(toDate, DateTimeKind.Utc).AddDays(1) 
                && _.Approveddate.HasValue && _.Approvedby.GetValueOrDefault() == userId).ToList();
        }
        public List<Employeeapproval> GetApprovedData(int month, int year, int customerId)
        {
            var _qry = orgCheckContext.Employeeapprovals.Include(x => x.Employee).AsNoTracking()
                .Where(_ => _.Employee.Customerid == customerId && _.Approveddate.HasValue && _.Approveddate.Value.Year == year);
            if (month > 0)
                _qry = _qry.Where(_ => _.Approveddate.Value.Month == month);
            return _qry.ToList();
        }
        public List<Employeequestionaire> GetEmployeeQuestions(string empCode, int customerId)
        {
            var empId = orgCheckContext.Employees.AsNoTracking().Where(_ => _.Employeecode == empCode && _.Customerid == customerId).FirstOrDefault()?.Id;
            return orgCheckContext.Employeequestionaires.Include(x => x.Question).AsNoTracking()
                .Where(_ => _.Employeeid == empId).ToList();
        }
        public List<Employeequestionaire> GetEmployeeQuestions(int empId)
        {

            return orgCheckContext.Employeequestionaires.Include(x => x.Question).AsNoTracking()
                .Where(_ => _.Employeeid == empId).ToList();
        }
        public List<Employeequestionaire> GetAllEmployeeQuestions(string empCode, int customerId)
        {
            var empId = orgCheckContext.Employees.AsNoTracking().Where(_ => _.Employeecode == empCode && _.Customerid == customerId).FirstOrDefault()?.Id;
            var questions = orgCheckContext.Questionaires.AsNoTracking().ToList();
            var answers= orgCheckContext.Employeequestionaires.Include(x => x.Question).AsNoTracking()
                .Where(_ => _.Employeeid == empId).ToList();
            var data = questions.Select(_ => new Employeequestionaire()
            {
                Questionid = _.Id,
                Question = _,
                Employeeid = empId.Value,
                Answer = (answers.Find(q => q.Questionid == _.Id) != null ? answers.Find(q => q.Questionid == _.Id).Answer : "-")
            }).ToList();

            return data;
        }
        public void SaveEmployeeQuestions(string empCode, int customerId, List<Employeequestionaire> questionaries)
        {
            var empId = orgCheckContext.Employees.FirstOrDefault(_ => _.Employeecode == empCode && _.Customerid == customerId).Id;
            foreach (var data in questionaries)
            {
                var record = new Employeequestionaire()
                {
                    Employeeid = empId,
                    Questionid = data.Questionid,
                    Answer = data.Answer,
                    Status = 1
                };
                orgCheckContext.Employeequestionaires.Add(record);
            }
            orgCheckContext.SaveChanges();
        }
        
        public string GenerateSearchRequestNumber()
        {
            // SR11042023000001
            string SRTemplate = $"SR{DateTime.Now.ToString("ddMMyyyy")}";
            string maxSRId = orgCheckContext.Employeesearches.AsNoTracking()
                .Where(_ => _.Searchrequestid.StartsWith(SRTemplate)).OrderByDescending(_ => _.Id).Take(1)
                .Select(_ => _.Searchrequestid).FirstOrDefault();
            int currentFlowNumber = 0;
            if (!string.IsNullOrEmpty(maxSRId))
                currentFlowNumber = Convert.ToInt32(maxSRId.Substring(10));

            return SRTemplate + ((currentFlowNumber + 1).ToString().PadLeft(6, '0'));
        }
        public int AddEmployeeSearch(Employeesearch record)
        {
            orgCheckContext.Employeesearches.Add(record);
            orgCheckContext.SaveChanges();
            return record.Id;
        }
        public void UpdateClientEmployeeSearch(Employeesearch searchrecord)
        {
            var record = orgCheckContext.Employeesearches.FirstOrDefault(_ => _.Id == searchrecord.Id);
            record.Clientname = searchrecord.Clientname;
            orgCheckContext.SaveChanges();
        }
        public void UpdateEmployeeSearch(Employeesearch searchrecord)
        {
            var record = orgCheckContext.Employeesearches.FirstOrDefault(_ => _.Id == searchrecord.Id);
            record.Finalresult = searchrecord.Finalresult;
            orgCheckContext.SaveChanges();
        }
        public Employee GetEmployeeById(int empId)
        {
            return orgCheckContext.Employees.Include(x => x.Customer).AsNoTracking().Where(_ => _.Id == empId).FirstOrDefault();
        }
        public int GetEmployeeIdByCode(string code)
        {
            return orgCheckContext.Employees.AsNoTracking().FirstOrDefault(_ => _.Employeecode == code)?.Id ?? default;
        }
        public Employeesearch GetEmployeesearch(string empCode, int userId, string finalResult)
        {
            var record = orgCheckContext.Employeesearches.AsNoTracking()
                .Where(_ => _.Employeecode == empCode && _.Createdby == (userId == 0 ? _.Createdby : userId));
            if (!string.IsNullOrEmpty(finalResult))
                record = record.Where(_ => _.Finalresult == finalResult);
            return record.OrderByDescending(_ => _.Id).Take(1).FirstOrDefault();
        }
        public Employeesearch GetEmployeeSearchById(int id)
        {
            return orgCheckContext.Employeesearches.Include(x => x.CreatedbyNavigation).AsNoTracking()
                .Include(x => x.Customer).Where(_ => _.Id == id).FirstOrDefault();
        }
        public List<Employeesearch> GetEmployeeSearch(DateTime fromDate, DateTime toDate, int userId, string finalResult)
        {
            var query = orgCheckContext.Employeesearches.Include(x => x.CreatedbyNavigation).Include(x => x.CreatedbyNavigation.Customer)
                .AsNoTracking()
                .Where(_ => _.Createddate >= DateTime.SpecifyKind(fromDate, DateTimeKind.Utc) && _.Createddate < DateTime.SpecifyKind(toDate, DateTimeKind.Utc).AddDays(1) 
                    && _.Createdby == userId && _.Status == 1);
            if (!string.IsNullOrEmpty(finalResult))
                query = query.Where(_ => _.Finalresult.ToUpper().Equals(finalResult.ToUpper()));
            return query.OrderByDescending(_ => _.Createddate).ToList();
        }
        public List<Employeesearch> GetEmployeeSearchData(DateTime fromDate, DateTime toDate, int customerId, string finalResult)
        {
            var query = orgCheckContext.Employeesearches.Include(x => x.Employee).Include(x => x.Employee.Customer)
                .Include(x => x.CreatedbyNavigation).Include(x => x.CreatedbyNavigation.Customer)
                .AsNoTracking()
                .Where(_ => _.Createddate >= DateTime.SpecifyKind(fromDate, DateTimeKind.Utc) && _.Createddate < DateTime.SpecifyKind(toDate, DateTimeKind.Utc).AddDays(1) 
                    && _.Searchresult == "F" && _.Customerid == customerId && _.Status == 1);
            if (!string.IsNullOrEmpty(finalResult))
                query = query.Where(_ => _.Finalresult.ToUpper().Equals(finalResult.ToUpper()));
            return query.OrderByDescending(_ => _.Createddate).ToList();
        }
        public void UpdateReportLink(int id, string reportLink, int companyId, int customerId, string clientname)
        {
            var record = orgCheckContext.Employeesearches.FirstOrDefault(_ => _.Id == id);
            var company = orgCheckContext.Companies.FirstOrDefault(_ => _.Id == companyId);
            var customer = orgCheckContext.Customers.FirstOrDefault(_ => _.Id == customerId);
            record.Finalresult = "Generated";
            if (companyId > 0)
                record.Transactionamount = company.Charges;
            else if (customerId > 0)
                record.Transactionamount = customer.Charges;
            record.Clientname = clientname;
            record.Reportlink = reportLink;
            record.Reportdate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            orgCheckContext.SaveChanges();
        }
        public List<Employeesearch> GetGeneratedReportsByCustomerMonth(int month, int year, int customerId)
        {
            var _qry = orgCheckContext.Employeesearches.Include(x => x.CreatedbyNavigation)
                .Include(x => x.CreatedbyNavigation.Customer).AsNoTracking()
                .Where(_ => _.Customerid == customerId && _.Reportdate.Value.Year == year && _.Status == 1);
            if (month > 0)
                _qry = _qry.Where(_ => _.Reportdate.Value.Month == month);
            return _qry.ToList();
        }
        public int GetGeneratedReportsCountByCustomerMonth(int month, int year, int customerId)
        {
            return orgCheckContext.Employeesearches.AsNoTracking()
                .Where(_ => _.Customerid == customerId && _.Reportdate.Value.Month == month && _.Reportdate.Value.Year == year && _.Status == 1)
                .Count();
        }
        public int GetMonthwiseGeneratedReportsCount(int month, int year)
        {
            return orgCheckContext.Employeesearches.AsNoTracking()
                .Where(_ => _.Reportdate.Value.Month == month && _.Reportdate.Value.Year == year && _.Status == 1)
                .Count();
        }
        public int GetSearchCountByCompanyMonth(int month, int year)
        {
            return orgCheckContext.Employeesearches.Include(x => x.CreatedbyNavigation).AsNoTracking()
                .Where(_ => _.Createddate.Month == month && _.Createddate.Year == year && _.Status == 1)
                .Count();
        }
        
        public void AddTempEmployee(Tempemployee[] employee)
        {
            orgCheckContext.Tempemployees.AddRange(employee);
            orgCheckContext.SaveChanges();
        }
        public Tempemployee AddTempEmployee(Tempemployee employee)
        {
            orgCheckContext.Tempemployees.Add(employee);
            orgCheckContext.SaveChanges();
            return employee;
        }
        public void AddTempEmployeeQuestions(Tempemployeequestionaire[] employeeQuestions)
        {
            orgCheckContext.Tempemployeequestionaires.AddRange(employeeQuestions);
            orgCheckContext.SaveChanges();
        }
        public List<Tempemployee> GetTempemployees(int fileId)
        {
            return orgCheckContext.Tempemployees.Include(x => x.Tempemployeequestionaires).AsNoTracking()
                .Where(_ => _.Fileid == fileId).OrderBy(_ => _.Id).ToList();
        }
        public Tempemployee GetTempemployeeById(int id)
        {
            return orgCheckContext.Tempemployees.Include(x => x.Tempemployeequestionaires).AsNoTracking()
                .FirstOrDefault(_ => _.Id == id);
        }
        public Tempemployee ViewTempEmployee(string empCode, int customerId)
        {
            return orgCheckContext.Tempemployees.AsNoTracking().Where(_ => _.Employeecode == empCode && _.Customerid == customerId).FirstOrDefault();
        }
        public List<Employeequestionaire> GetAllTempEmployeeQuestions(string empCode, int customerId)
        {
            var empId = orgCheckContext.Tempemployees.AsNoTracking().Where(_ => _.Employeecode == empCode && _.Customerid == customerId).FirstOrDefault()?.Id;
            var questions = orgCheckContext.Questionaires.AsNoTracking().ToList();
            var answers = orgCheckContext.Tempemployeequestionaires.Include(x => x.Question).AsNoTracking()
                .Where(_ => _.Tempemployeeid == empId).ToList();
            var data = questions.Select(_ => new Employeequestionaire()
            {
                Questionid = _.Id,
                Question = _,
                Employeeid = empId.Value,
                Answer = (answers.Find(q => q.Questionid == _.Id) != null ? answers.Find(q => q.Questionid == _.Id).Answer : "-")
            }).ToList();

            return data;
        }
        public void DeleteTempEmployees(int fileId)
        {
            var questions = orgCheckContext.Tempemployeequestionaires.Include(x => x.Tempemployee)
                .Where(_ => _.Tempemployee.Fileid == fileId).ToList();
            orgCheckContext.Tempemployeequestionaires.RemoveRange(questions);

            var lists = orgCheckContext.Tempemployees.AsNoTracking().Where(_ => _.Fileid == fileId).ToList();
            orgCheckContext.Tempemployees.RemoveRange(lists);
            orgCheckContext.SaveChanges();
        }
        public void DeleteTempEmployeeById(int id)
        {
            var questions = orgCheckContext.Tempemployeequestionaires.Include(x => x.Tempemployee)
                .Where(_ => _.Tempemployee.Id == id).ToList();
            orgCheckContext.Tempemployeequestionaires.RemoveRange(questions);

            var emp = orgCheckContext.Tempemployees.FirstOrDefault(_ => _.Id == id);
            orgCheckContext.Tempemployees.Remove(emp);
            orgCheckContext.SaveChanges();
        }

        public List<Employee> GetAllEmployeesWithDetails(int customerId)
        {
            return orgCheckContext.Employees.Include(x => x.Employeequestionaires).ThenInclude(x => x.Question).AsNoTracking()
                .Where(x => x.Customerid == customerId).ToList();
        }

        public Invalidemployee AddInvalidEmployee(Invalidemployee employee)
        {
            orgCheckContext.Invalidemployees.Add(employee);
            orgCheckContext.SaveChanges();
            return employee;
        }
        public void AddInvalidEmployeeQuestions(Invalidemployeequestionaire[] employeeQuestions)
        {
            orgCheckContext.Invalidemployeequestionaires.AddRange(employeeQuestions);
            orgCheckContext.SaveChanges();
        }
        public Invalidemployee GetInvalidemployee(int Id)
        {
            return orgCheckContext.Invalidemployees.Include(x => x.Customer).Include(x => x.CreatedbyNavigation).FirstOrDefault(_ => _.Id == Id);
        }

        public List<LookupEmpverificationResponse> GetAllLookupVerifications() 
        {
            return orgCheckContext.LookupEmpverificationResponses.AsNoTracking().Where(_ => _.Status == 1).OrderBy(_ => _.Id).ToList();
        }
        
        public List<Tempemployee> GetOpenRequestsByCustomer(int customerId)
        {
            return orgCheckContext.Empverificationrequests.Include(x => x.CreatedbyNavigation)
                .Include(x => x.CreatedbyNavigation.Customer)
                .Include(x => x.Tempemployee).Include(x => x.Tempemployee.Customer).AsNoTracking()
                .Where(_ => _.Tempemployee.Customerid == customerId && _.Requeststatus == "Open").OrderBy(x => x.Id).Select(_ => _.Tempemployee).ToList();
        }
        
        public List<LookupDiscrepancytype> GetDiscrepancytypes()
        {
            return orgCheckContext.LookupDiscrepancytypes.AsNoTracking().ToList();
        }
        public void AddabscondDetail(Absconddetail record)
        {
            orgCheckContext.Absconddetails.Add(record);
            orgCheckContext.SaveChanges();
        }
        public void DeleteAbscondDetail(int id)
        {
            var existingentity = orgCheckContext.Absconddetails.AsNoTracking().Where(_ => _.Id == id).FirstOrDefault();
            existingentity.Status = 0;
            orgCheckContext.SaveChanges();
        }
        public List<Absconddetail> GetAbsconddetails(string name, string mobile, string email, string uan, string others)
        {
            if (string.IsNullOrEmpty(others))
            {
                var query = orgCheckContext.Absconddetails.Include(x => x.Employee).Include(x => x.CreatedbyNavigation.Customer).AsNoTracking();
                query = query.Where(_ => (_.Status == 1));
                if (!string.IsNullOrEmpty(mobile))
                    query = query.Where(_ => _.Mobileno == mobile);
                if (!string.IsNullOrEmpty(name))
                    query = query.Where(_ => _serviceProvider.GetRequiredService<CryptoService>().Decrypt(_.Employee.Name).ToLower().Contains(name.ToLower()));
                if (!string.IsNullOrEmpty(email))
                    query = query.Where(_ => _.Emailid == email);
                if (!string.IsNullOrEmpty(uan))
                    query = query.Where(_ => _.Uannumber == uan);
                return query.OrderBy(_ => _.Employee.Fromdate).ToList();
            }
            else
            {
                var query = orgCheckContext.Absconddocumentdata.Include(x => x.Abscond).Include(x => x.Abscond.Employee).Include(x => x.Abscond.CreatedbyNavigation.Customer).AsNoTracking();
                query = query.Where(_ => (_.Abscond.Status == 1));
                if (!string.IsNullOrEmpty(mobile))
                    query = query.Where(_ => _.Abscond.Mobileno == mobile);
                if (!string.IsNullOrEmpty(name))
                    query = query.Where(_ => _.Abscond.Employee.Name.ToLower().Contains(name.ToLower()));
                if (!string.IsNullOrEmpty(email))
                    query = query.Where(_ => _.Abscond.Emailid == email);
                if (!string.IsNullOrEmpty(uan))
                    query = query.Where(_ => _.Abscond.Uannumber == uan);
                return query.OrderBy(_ => _.Abscond.Employee.Fromdate).Select(x => x.Abscond).Distinct().ToList();
            }
        }

        public string GenerateRequestNumber()
        {
            int count = orgCheckContext.Empverificationrequests.AsNoTracking()
                .Where(_ => _.Active == 1 && _.Createddate.Value.Date == DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc).Date).ToList().Count + 1;
            string VRNumber = $"#VZ-{DateTime.Now.ToString("yyyyMMdd")}{count.ToString("D4")}";
            return VRNumber;
        }
        public void AddEmpVerificationRequest(Empverificationrequest empverificationrequest)
        {
            orgCheckContext.Empverificationrequests.Add(empverificationrequest);
            orgCheckContext.SaveChanges();
        }
        public Empverificationrequest GetEmpverificationrequestByTempId(int tempId)
        {
            return orgCheckContext.Empverificationrequests.FirstOrDefault(_ => _.Tempemployeeid == tempId);
        }
        public Empverificationrequest GetEmpverificationrequestById(int id)
        {
            return orgCheckContext.Empverificationrequests.Include(x => x.Employee).Include(x => x.Tempemployee).Include(x => x.Invalidemployee)
                .FirstOrDefault(_ => _.Id == id);
        }
        public void UpdateEmpVerificationRequest(int Id, int employeeId, string status, string reportname)
        {
            var request = orgCheckContext.Empverificationrequests.FirstOrDefault(x => x.Id == Id);            
            if (status == "Approved")
            {
                request.Employeeid = employeeId;
                request.Tempemployeeid = null;
                request.Invalidemployeeid = null;
            }
            else if(status == "Rejected")
            {
                request.Employeeid = null;
                request.Tempemployeeid = null;
                request.Invalidemployeeid = employeeId;
            }
            else if (status == "Generated")
                request.Reportname = reportname;
            request.Requeststatus = status;
            orgCheckContext.SaveChanges();
        }
        public List<Empverificationrequest> GetEmpverificationrequests(string status, string ticketNumber, int customerId)
        {
            var query = orgCheckContext.Empverificationrequests.Include(x => x.Tempemployee).Include(x => x.Employee).Include(x => x.Invalidemployee)
                .Include(x => x.Tempemployee.Customer).Include(x => x.Employee.Customer).Include(x => x.Invalidemployee.Customer)
                .Include(x => x.CreatedbyNavigation)
                .Include(x => x.CreatedbyNavigation.Customer).AsQueryable();
            if (customerId > 0)
                query = query.Where(x => x.CreatedbyNavigation.Customerid.Value == customerId);            
            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.Requeststatus == status);
            if (!string.IsNullOrEmpty(ticketNumber))
                query = query.Where(x => x.Requestnumber == status);
            return query.ToList();
        }
    }
}
