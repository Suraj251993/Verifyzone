using Microsoft.AspNetCore.Http;
using OrgCheck.ViewModels;
using System;
using System.Collections.Generic;

namespace OrgCheck.Services.Interfaces
{
    public interface IEmployeeService
    {
        HRDashboardViewModel GetDashboardData(int year, int customerId);
        CustomerDashboardCount GetDashboardCount(int month, int year, int userId);
        string AddEmployee(EmployeeViewModel viewModel, string inputFormat, bool isApproved = true);
        EmployeeViewModel ViewEmployee(string empCode, int customerId, string lastWorkingdate, bool addSearch);
        List<EmployeeViewModel> GetAllEmployees(int customerId);
        List<EmployeeViewModel> GetAllEmployeesWithQuestions(int customerId);
        List<EmployeeQuestionaireViewModel> ViewEmployeeQuestions(string empCode, int customerId);
        bool AddEmployeeApproval(int employeeId, int searchId, bool isEdit);
        bool UpdateClientEmployeeSearch(EmployeeSearchViewModel viewModel);
        bool UpdateEmployeeApproval(int id, int userId);
        List<EmployeeApprovalViewModel> GetEmployeeApprovals(int customerId);
        int GetApprovalGivenCount(int month, int year, int userId);
        List<EmployeeApprovalViewModel> GetApprovedData(DateTime fromDate, DateTime toDate, int userId);
        List<EmployeeApprovalViewModel> GetEditEmployeeApprovals(int customerId);
        List<EmployeeSearchViewModel> GetEmployeeSearchHistory(DateTime fromDate, DateTime toDate, string finalResult);
        List<EmployeeSearchViewModel> GetEmployeeSearchAttrition(DateTime fromDate, DateTime toDate, int customerId, string finalResult);
        EmployeeApprovalViewModel GetEmployeeApproval(int empId);

        string GenerateEmployeeDetails(int customerId);

        EmployeeSearchViewModel GetEmployeeSearch(int searchId);
        string Validate(IFormFile file, string strSixDigitNumber);
        UploadSummaryViewModel ParseFile(IFormFile file, string strSixDigitNumber, int customerId, int userId);
        int AddTempEmployee(RequestViewModel item, int userId);
        EmployeeViewModel ViewTempEmployee(string empCode, int customerId);
        bool ApproveFile(int fileId, int userId);
        bool RejectFile(int fileId);
        bool SaveEmployeeQuestionaries(string empCode, List<EmployeeQuestionaireViewModel> questions);

        string AddEmpVerificationRequest(RequestViewModel model, int userId);
        bool SendEmploymentVerificationApprovalReminder(int searchId);
        EmployeeRequestViewModel GetEmployeeVerificationRequest(int id);
        List<RequestViewModel> GetEmployeeVerificationRequests(string status, string ticketNumber, int customerId);

        List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetLookupVerificationResponses();        
        List<EmployeeRequestViewModel> GetOpenRequests(int customerId);        
        //List<EmployeeSearchViewModel> GetGeneratedRecords(int month, int year, int companyId);
        List<EmployeeSearchViewModel> GetGeneratedRecordsByCustomer(int month, int year, int customerId);
        AdminReportViewModel GetAdminDashboardData(int month, int year);
        bool ReconcileCustomerCredit(int customerId);

        string ApproveEmployee(EmployeeViewModel model, int userId);
        bool RejectEmployee(int Id, string Comments, int userId);

        List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetLookupDiscrepancyTypes();
        bool AddAbscondDetail(AbscondDetailViewModel viewModel);
        List<AbscondDetailViewModel> GetAbscondDetails(string name, string mobile, string email, string uan, string others);
    }
}
