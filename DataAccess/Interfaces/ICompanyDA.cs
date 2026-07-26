using System;
using System.Collections.Generic;
using OrgCheck.Models;

namespace OrgCheck.DataAccess.Interfaces
{
    public interface ICompanyDA
    {
        List<Company> GetCompanies();
        Company GetCompany(int Id);
        Company AddCompany(Company company);
        void AddCompanyWallet(Companywallet companywallet);
        void UpdateCompany(Company company);
        bool IsDuplicateCompany(int id, string name);
        void AddCompanyCredit(Companycredit companycredit);
        List<Companycredit> GetCompanycredits(int companyId);
        int GetCompanyBalance(int companyId);
        void ReconcileCompanyCredit(int companyId, bool isEmp, bool isEdu);
    }
}
