using OrgCheck.Models;
using System;
using System.Collections.Generic;

namespace OrgCheck.DataAccess.Interfaces
{
    public interface IStudentDA
    {
        Student AddStudent(Student student);
        void AddBulkStudent(Student[] students);
        void AddApproval(Studentapproval approval);
        void UpdateApproval(Studentapproval approval);
        List<Studentapproval> GetStudentApprovals(int customerId, bool isEdit);
        Studentapproval GetStudentApprovalById(int id);

        List<Student> ViewStudent(string studentId, int customerId);
        Student ViewStudentById(int id);
        List<Student> GetAllStudents(int customerId);
        Studentapproval GetLatestApproval(int studentId);

        List<Studentapproval> GetApprovedData(DateTime fromDate, DateTime toDate, int userId);
        List<Studentapproval> GetApprovedData(int month, int year, int customerId);

        string GenerateSearchRequestNumber();
        int AddStudentSearch(Studentsearch record);
        void UpdateStudentSearch(Studentsearch searchrecord);
        Student GetStudentById(int stuId);

        Studentsearch GetStudentSearchById(int id);
        List<Studentsearch> GetStudentSearch(DateTime fromDate, DateTime toDate, int userId, string finalResult);
        void UpdateReportLink(int id, int studentId, string reportLink, int companyId, int customerId);
        List<Studentsearch> GetGeneratedReportsByCustomerMonth(int month, int year, int customerId);
        //List<Studentsearch> GetGeneratedReportsByCompanyMonth(int month, int year, int companyId);

        void AddTempStudents(Tempstudent[] students);
        List<Tempstudent> GetTempstudents(int fileId);
        void DeleteTempStudents(int fileId);

        List<LookupStuverificationResponse> GetAllStudentVerificationResponses();
        void AddStudentRequest(Studentrequest request);
        void UpdateStudentRequest(Studentrequest request);
        List<Studentrequest> GetOpenRequestsByCustomer(int customerId);
        Studentrequest GetStudentrequestById(int id);
        //List<Studentrequest> GetStudentrequestsByCompany(int companyId, bool isOpenOnly, bool isRepliedOnly);
        Studentrequest GetStudentrequestBySearch(int searchId);
        List<Studentrequest> GetStudentrequestsByCustomer(int customerId, bool isOpenOnly, bool isRepliedOnly);
    }
}
