using OrgCheck.Report.ReportModel;
using OrgCheck.ViewModels;
using System;
using System.Collections.Generic;

namespace OrgCheck.Services.Interfaces
{
    public interface ICompanyService
    {
        List<CompanyViewModel> GetCompanies();
        CompanyViewModel GetCompany(int id);
        string AddCompany(CompanyViewModel viewModel);
        string UpdateCompany(CompanyViewModel viewModel);
        List<CompanyCreditViewModel> GetCompanyCredits(int CompanyId);
        bool AddCompanyCredit(CompanyCreditViewModel viewModel);
        int GetCompanyBalance(int companyId);
        bool ReconcileCompanyCredit(int companyId);
        bool CheckCompanyBalance(int companyId);
        VerificationDetails GetReportData(int searchId, string clientname);
        public bool GenerateReportBySearchId(int searchid, string source);
        VerificationDetails GenerateReportByRequestId(int id);
        bool GenerateStudentReport(int stuId, int searchId, string source);
    }
}
