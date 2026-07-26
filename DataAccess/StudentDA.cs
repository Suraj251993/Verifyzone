using Microsoft.EntityFrameworkCore;
using OrgCheck.DataAccess.Interfaces;
using OrgCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrgCheck.DataAccess
{
    public class StudentDA : IStudentDA
    {
        public PostgresContext orgCheckContext;
        public StudentDA(PostgresContext _orgCheckContext)
        {
            orgCheckContext = _orgCheckContext;
        }
        public Student AddStudent(Student student)
        {
            orgCheckContext.Students.Add(student);
            orgCheckContext.SaveChanges();

            if (student.Isapproved)
            {
                // Add default entry
                var record = new Studentapproval()
                {
                    Studentid = student.Id,
                    Approvedby = student.Createdby,
                    Approveddate = student.Createddate
                };
                orgCheckContext.Studentapprovals.Add(record);
            }
            orgCheckContext.SaveChanges();
            return student;
        }
        public void AddBulkStudent(Student[] students)
        {
            foreach(var student in students)
            {
                orgCheckContext.Students.Add(student);
                orgCheckContext.SaveChanges();
                if(student.Isapproved)
                {
                    var record = new Studentapproval()
                    {
                        Studentid = student.Id,
                        Isedit = false,
                        Approvedby = student.Createdby,
                        Approveddate = student.Createddate
                    };
                    AddApproval(record);
                }
            }
            
        }
        public void AddApproval(Studentapproval approval)
        {
            orgCheckContext.Studentapprovals.Add(approval);
            orgCheckContext.SaveChanges();
        }

        public void UpdateApproval(Studentapproval approval)
        {
            var record = orgCheckContext.Studentapprovals.FirstOrDefault(_ => _.Id == approval.Id);
            record.Approvedby = approval.Approvedby;
            record.Approveddate = approval.Approveddate;
            orgCheckContext.SaveChanges();
        }
        public List<Studentapproval> GetStudentApprovals(int customerId, bool isEdit)
        {
            return orgCheckContext.Studentapprovals.Include(x => x.Student).Include(x => x.RequestedbyNavigation)
                .Include(x => x.RequestedbyNavigation.Customer).AsNoTracking()
                .Where(x => x.Student.Customerid == customerId && x.Requestedby != null && x.Approvedby == null && x.Isedit == isEdit)
                .OrderByDescending(x => x.Id)
                .ToList();
        }
        public Studentapproval GetStudentApprovalById(int id)
        {
            return orgCheckContext.Studentapprovals.AsNoTracking().Where(x => x.Id == id).FirstOrDefault();
        }

        public List<Student> ViewStudent(string studentId, int customerId)
        {
            return orgCheckContext.Students.AsNoTracking().Where(_ => _.Studentid == studentId && _.Customerid == customerId).ToList();
        }
        public Student ViewStudentById(int id)
        {
            return orgCheckContext.Students.FirstOrDefault(_ => _.Id == id);
        }
        public List<Student> GetAllStudents(int customerId)
        {
            return orgCheckContext.Students.AsNoTracking().Where(_ => _.Customerid == customerId).ToList();
        }
        public Studentapproval GetLatestApproval(int studentId)
        {
            return orgCheckContext.Studentapprovals
                .Include(x => x.ApprovedbyNavigation).AsNoTracking()
                .Where(_ => _.Studentid == studentId && _.Approveddate != null).OrderByDescending(_ => _.Id).Take(1).FirstOrDefault();
        }

        public List<Studentapproval> GetApprovedData(DateTime fromDate, DateTime toDate, int userId)
        {
            return orgCheckContext.Studentapprovals.Include(x => x.Student).AsNoTracking()
                .Where(_ => _.Approveddate.Value >= DateTime.SpecifyKind(fromDate, DateTimeKind.Utc) && _.Approveddate.Value < DateTime.SpecifyKind(toDate, DateTimeKind.Utc).AddDays(1)
                && _.Approveddate.HasValue && _.Approvedby.GetValueOrDefault() == userId).ToList();
        }
        public List<Studentapproval> GetApprovedData(int month, int year, int customerId)
        {
            var _qry = orgCheckContext.Studentapprovals.Include(x => x.Student).AsNoTracking()
                .Where(_ => _.Student.Customerid == customerId && _.Approveddate.HasValue && _.Approveddate.Value.Year == year);
            if (month > 0)
                _qry = _qry.Where(_ => _.Approveddate.Value.Month == month);
            return _qry.ToList();
        }

        public string GenerateSearchRequestNumber()
        {
            // SR11042023000001
            string SRTemplate = $"SR{DateTime.Now.ToString("ddMMyyyy")}";
            string maxSRId = orgCheckContext.Studentsearches.AsNoTracking()
                .Where(_ => _.Searchrequestid.StartsWith(SRTemplate)).OrderByDescending(_ => _.Id).Take(1)
                .Select(_ => _.Searchrequestid).FirstOrDefault();
            int currentFlowNumber = 0;
            if (!string.IsNullOrEmpty(maxSRId))
                currentFlowNumber = Convert.ToInt32(maxSRId.Substring(10));

            return SRTemplate + ((currentFlowNumber + 1).ToString().PadLeft(6, '0'));
        }
        public int AddStudentSearch(Studentsearch record)
        {
            orgCheckContext.Studentsearches.Add(record);
            orgCheckContext.SaveChanges();
            return record.Id;
        }
        public void UpdateStudentSearch(Studentsearch searchrecord)
        {
            var record = orgCheckContext.Studentsearches.FirstOrDefault(_ => _.Id == searchrecord.Id);
            record.Finalresult = searchrecord.Finalresult;
            orgCheckContext.SaveChanges();
        }
        public Student GetStudentById(int stuId)
        {
            return orgCheckContext.Students.Include(x => x.Customer).AsNoTracking().Where(_ => _.Id == stuId).FirstOrDefault();
        }

        public Studentsearch GetStudentSearchById(int id)
        {
            return orgCheckContext.Studentsearches.Include(x => x.CreatedbyNavigation).AsNoTracking()
                .Include(x => x.Customer).Where(_ => _.Id == id).FirstOrDefault();
        }
        public List<Studentsearch> GetStudentSearch(DateTime fromDate, DateTime toDate, int userId, string finalResult)
        {
            var query = orgCheckContext.Studentsearches.Include(x => x.Customer).Include(x => x.Studentapprovals).Include(x => x.CreatedbyNavigation).AsNoTracking()
                .Where(_ => _.Createdby == userId);
            query = query.Where(_ => _.Createddate >= DateTime.SpecifyKind(fromDate, DateTimeKind.Utc) && _.Createddate < DateTime.SpecifyKind(toDate, DateTimeKind.Utc).AddDays(1));
            if (!string.IsNullOrEmpty(finalResult))
                query = query.Where(_ => _.Finalresult.ToUpper().Equals(finalResult.ToUpper()));

            return query.OrderByDescending(_ => _.Createddate).ToList();
        }
        public void UpdateReportLink(int id, int studentId, string reportLink, int companyId, int customerId)
        {
            var record = orgCheckContext.Studentsearches.FirstOrDefault(_ => _.Id == id);
            var company = orgCheckContext.Companies.FirstOrDefault(_ => _.Id == companyId);
            var customer = orgCheckContext.Customers.FirstOrDefault(_ => _.Id == customerId);
            
            if (companyId > 0)
                record.Transactionamount = company.Educharges;
            else if (customerId > 0)
                record.Transactionamount = customer.Charges;

            record.Finalresult = "Generated";
            if (studentId > 0)
                record.Studentkey = studentId;
            record.Reportlink = reportLink;
            record.Reportdate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            orgCheckContext.SaveChanges();
        }
        public List<Studentsearch> GetGeneratedReportsByCustomerMonth(int month, int year, int customerId)
        {
            var _qry = orgCheckContext.Studentsearches.AsNoTracking()
                .Where(_ => _.Customerid == customerId && _.Reportdate.Value.Year == year && _.Status == 1);
            if (month > 0)
                _qry = _qry.Where(_ => _.Reportdate.Value.Month == month);
            return _qry.ToList();
        }
        //public List<Studentsearch> GetGeneratedReportsByCompanyMonth(int month, int year, int companyId)
        //{
        //    var _qry = orgCheckContext.Studentsearches.Include(x => x.Customer).Include(x => x.CreatedbyNavigation).AsNoTracking()
        //        .Where(_ => _.CreatedbyNavigation.Companyid.Value == companyId && _.Reportdate.Value.Year == year && _.Status == 1);
        //    if (month > 0)
        //        _qry = _qry.Where(_ => _.Reportdate.Value.Month == month);
        //    return _qry.ToList();
        //}

        public void AddTempStudents(Tempstudent[] students)
        {
            orgCheckContext.Tempstudents.AddRange(students);
            orgCheckContext.SaveChanges();
        }
        public List<Tempstudent> GetTempstudents(int fileId)
        {
            return orgCheckContext.Tempstudents.AsNoTracking()
                .Where(_ => _.Fileid == fileId).OrderBy(_ => _.Id).ToList();
        }
        public void DeleteTempStudents(int fileId)
        {
            var lists = orgCheckContext.Tempstudents.AsQueryable().Where(_ => _.Fileid == fileId).ToList();
            orgCheckContext.Tempstudents.RemoveRange(lists);
            orgCheckContext.SaveChanges();
        }

        public List<LookupStuverificationResponse> GetAllStudentVerificationResponses()
        {
            return orgCheckContext.LookupStuverificationResponses.AsNoTracking().Where(_ => _.Status == 1).OrderBy(_ => _.Id).ToList();
        }
        public void AddStudentRequest(Studentrequest request)
        {
            orgCheckContext.Studentrequests.Add(request);
            orgCheckContext.SaveChanges();
        }
        public void UpdateStudentRequest(Studentrequest request)
        {
            var existingentity = orgCheckContext.Studentrequests.FirstOrDefault(_ => _.Id == request.Id);
            existingentity.Responsetype = request.Responsetype;
            existingentity.Replycomments = request.Replycomments;
            existingentity.Repliedby = request.Repliedby;
            existingentity.Replieddate = DateTime.Now;
            orgCheckContext.SaveChanges();
        }
        public List<Studentrequest> GetOpenRequestsByCustomer(int customerId)
        {
            return orgCheckContext.Studentrequests.Include(x => x.Customer).Include(x => x.RaisedbyNavigation)
                .Include(x => x.RaisedbyNavigation.Customer).AsNoTracking()
                .Where(_ => _.Repliedby == null && _.Status == 1 && _.Customerid == customerId).OrderBy(x => x.Id).ToList();
        }
        public Studentrequest GetStudentrequestById(int id)
        {
            return orgCheckContext.Studentrequests.Include(x => x.Customer).Include(x => x.RaisedbyNavigation)
                .AsNoTracking().Where(_ => _.Id == id).FirstOrDefault();
        }
        //public List<Studentrequest> GetStudentrequestsByCompany(int companyId, bool isOpenOnly, bool isRepliedOnly)
        //{
        //    var query = orgCheckContext.Studentrequests.Include(x => x.Customer).Include(x => x.RepliedbyNavigation).Include(x => x.RaisedbyNavigation)
        //        .AsNoTracking().Where(_ => _.Status == 1);
        //    if (isOpenOnly)
        //        query = query.Where(_ => _.Repliedby == null);
        //    if (isRepliedOnly)
        //        query = query.Where(_ => _.Repliedby != null && _.Replieddate.Value >= (DateTime.Now.AddDays(-15)));
        //    return query.OrderByDescending(x => x.Id).ToList();
        //}
        public Studentrequest GetStudentrequestBySearch(int searchId)
        {
            return orgCheckContext.Studentrequests.Include(x => x.Customer).Include(x => x.RepliedbyNavigation).Include(x => x.RaisedbyNavigation)
                .AsNoTracking().Where(_ => _.Status == 1 && _.Searchid == searchId).FirstOrDefault();
        }
        public List<Studentrequest> GetStudentrequestsByCustomer(int customerId, bool isOpenOnly, bool isRepliedOnly)
        {
            var query = orgCheckContext.Studentrequests.Include(x => x.Customer).Include(x => x.RepliedbyNavigation).Include(x => x.RaisedbyNavigation)
                .AsNoTracking().Where(_ => _.Status == 1 && _.RaisedbyNavigation.Customerid == customerId);
            if (isOpenOnly)
                query = query.Where(_ => _.Repliedby == null);
            if (isRepliedOnly)
                query = query.Where(_ => _.Repliedby != null && _.Replieddate.Value >= (DateTime.Now.AddDays(-15)));
            return query.OrderByDescending(x => x.Id).ToList();
        }
    }
}
