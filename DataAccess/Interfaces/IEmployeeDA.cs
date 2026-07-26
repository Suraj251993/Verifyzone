using OrgCheck.Models;
using System;
using System.Collections.Generic;

namespace OrgCheck.DataAccess.Interfaces
{
    public interface IEmployeeDA
    {
        Employee AddEmployee(Employee employee, List<Employeequestionaire> questionaries);
        void AddBulkEmployee(Employee[] employees);
        void AddApproval(Employeeapproval approval);
        void UpdateApproval(Employeeapproval approval);
        Employee ViewEmployee(string empCode, int customerId, string lastworkingDate);
        List<Employee> GetAllEmployees(int customerId);
        Employeeapproval GetLatestApproval(int employeeId);
        List<Employeeapproval> GetApprovedData(DateTime fromDate, DateTime toDate, int userId);
        List<Employeeapproval> GetApprovedData(int month, int year, int customerId);
        List<Employeeapproval> GetEmployeeApprovals(int customerId, bool isEdit);
        List<Employeeapproval> GetApprovalGiven(int month, int year, int userId);
        int GetApprovalGivenCount(int month, int year, int userId);
        Employeeapproval GetEmployeeApprovalById(int id);
        Employeeapproval GetEmployeeApprovalByEmployeeId(int id);
        List<Employeequestionaire> GetEmployeeQuestions(string empCode, int customerId);
        List<Employeequestionaire> GetAllEmployeeQuestions(string empCode, int customerId);
        void SaveEmployeeQuestions(string empCode, int customerId, List<Employeequestionaire> questionaries);
        List<Employeequestionaire> GetEmployeeQuestions(int empId);
        Employee GetEmployeeById(int empId);
        int GetEmployeeIdByCode(string code);
        int AddEmployeeSearch(Employeesearch record);
        void UpdateClientEmployeeSearch(Employeesearch searchrecord);
        void UpdateEmployeeSearch(Employeesearch searchrecord);
        Employeesearch GetEmployeesearch(string empCode, int userId, string finalResult);
        Employeesearch GetEmployeeSearchById(int id);
        List<Employeesearch> GetEmployeeSearch(DateTime fromDate, DateTime toDate, int userId, string finalResult);
        List<Employeesearch> GetEmployeeSearchData(DateTime fromDate, DateTime toDate, int customerId, string finalResult);
        void UpdateReportLink(int id, string reportLink, int companyId, int customerId, string clientname);
        List<Employeesearch> GetGeneratedReportsByCustomerMonth(int month, int year, int customerId);
        int GetGeneratedReportsCountByCustomerMonth(int month, int year, int customerId);
        int GetMonthwiseGeneratedReportsCount(int month, int year);
        int GetSearchCountByCompanyMonth(int month, int year);
        //List<Employeesearch> GetGeneratedReportsByCompanyMonth(int month, int year, int companyId);
        string GenerateSearchRequestNumber();
        List<Employee> GetAllEmployeesWithDetails(int customerId);

        void AddTempEmployee(Tempemployee[] employee);
        Tempemployee AddTempEmployee(Tempemployee employee);
        void AddTempEmployeeQuestions(Tempemployeequestionaire[] employeeQuestions);
        List<Tempemployee> GetTempemployees(int fileId);
        Tempemployee ViewTempEmployee(string empCode, int customerId);
        Tempemployee GetTempemployeeById(int id);
        List<Employeequestionaire> GetAllTempEmployeeQuestions(string empCode, int customerId);
        void DeleteTempEmployees(int fileId);
        void DeleteTempEmployeeById(int id);

        Invalidemployee AddInvalidEmployee(Invalidemployee employee);
        void AddInvalidEmployeeQuestions(Invalidemployeequestionaire[] employeeQuestions);
        Invalidemployee GetInvalidemployee(int Id);

        string GenerateRequestNumber();
        List<Empverificationrequest> GetEmpverificationrequests(string status, string ticketNumber, int customerId);
        void AddEmpVerificationRequest(Empverificationrequest empverificationrequest);
        Empverificationrequest GetEmpverificationrequestByTempId(int id);
        Empverificationrequest GetEmpverificationrequestById(int id);
        void UpdateEmpVerificationRequest(int Id, int employeeId, string status, string reportname);
        List<LookupEmpverificationResponse> GetAllLookupVerifications();        
        List<Tempemployee> GetOpenRequestsByCustomer(int customerId);

        List<LookupDiscrepancytype> GetDiscrepancytypes();
        void AddabscondDetail(Absconddetail record);
        void DeleteAbscondDetail(int id);
        List<Absconddetail> GetAbsconddetails(string name, string mobile, string email, string uan, string others);
    }
}
