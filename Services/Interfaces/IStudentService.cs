using Microsoft.AspNetCore.Http;
using OrgCheck.ViewModels;
using System;
using System.Collections.Generic;

namespace OrgCheck.Services.Interfaces
{
    public interface IStudentService
    {
        string AddStudent(StudentViewModel viewModel, bool isApproved = true);
        List<StudentViewModel> ViewStudents(string studentId, int customerId);
        StudentViewModel ViewStudent(int id, bool addSearch);
        StudentViewModel ViewStudentById(int id);
        int AddStudentSearch(StudentSearchViewModel viewModel);
        StudentSearchViewModel GetStudentSearch(int searchId);
        List<StudentSearchViewModel> GetStudentSearchHistory(DateTime fromDate, DateTime toDate, string finalResult);

        bool AddStudentApproval(int studentId, int searchId, bool isEdit);
        bool UpdateStudentApproval(int id, int userId);
        List<StudentApprovalViewModel> GetStudentApprovals(int customerId);
        //List<StudentSearchViewModel> GetGeneratedRecords(int month, int year, int companyId);

        string Validate(IFormFile file, string strSixDigitNumber);
        List<ErrorLogViewModel> ValidateStudent(StudentViewModel viewModel);
        UploadSummaryViewModel ParseFile(IFormFile file, string strSixDigitNumber, int customerId, int userId);
        List<StudentViewModel> GetTempstudents(int fileId);
        bool ApproveFile(int fileId, int userId);
        bool RejectFile(int fileId);

        CustomerDashboardCount GetDashboardCount(int month, int year);

        List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetLookupStuVerificationResponses();
        bool AddStudentRequest(StudentRequestViewModel viewModel);
        bool UpdateStudentRequest(StudentRequestViewModel viewModel);
        List<StudentRequestViewModel> GetOpenRequests(int customerId);
        StudentRequestViewModel GetRequestById(int id);
        //List<StudentRequestViewModel> GetStudentRequestByCompany(int companyId, bool isOpenOnly, bool isRepliedOnly);
        List<StudentRequestViewModel> GetStudentRequestByCustomer(int customerId, bool isOpenOnly, bool isRepliedOnly);
    }
}
