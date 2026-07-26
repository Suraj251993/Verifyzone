using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrgCheck.DataAccess.Interfaces;
using OrgCheck.Models;

namespace OrgCheck.DataAccess
{
    public class CompanyDA : ICompanyDA
    {
        public PostgresContext orgCheckContext;
        public CompanyDA(PostgresContext _orgCheckContext)
        {
            orgCheckContext = _orgCheckContext;
        }
        public List<Company> GetCompanies()
        {
            return orgCheckContext.Companies.AsNoTracking().Where(_ => _.Status == 1).OrderBy(_ => _.Name).ToList();
        }
        public Company GetCompany(int Id)
        {
            return orgCheckContext.Companies.AsNoTracking().Where(_ => _.Id == Id).FirstOrDefault();
        }
        public Company AddCompany(Company company)
        {
            orgCheckContext.Companies.Add(company);
            orgCheckContext.SaveChanges();
            return company;
        }
        public void AddCompanyWallet(Companywallet companywallet)
        {
            orgCheckContext.Companywallets.Add(companywallet);
            orgCheckContext.SaveChanges();
        }
        public void UpdateCompany(Company company)
        {
            var existingEntity = orgCheckContext.Companies.FirstOrDefault(_ => _.Id == company.Id);
            if (existingEntity != null)
            {
                existingEntity.Name = company.Name;
                existingEntity.Address = company.Address;
                existingEntity.Contactname = company.Contactname;
                existingEntity.Contactnumber = company.Contactnumber;
                existingEntity.Email = company.Email;
                existingEntity.GstNumber = company.GstNumber;
                existingEntity.TanNumber = company.TanNumber;
                existingEntity.PanNumber = company.PanNumber;
                orgCheckContext.SaveChanges();
            }
        }
        public bool IsDuplicateCompany(int id, string name)
        {
            bool _result = false;
            var company = new Company();
            if (id > 0)
                company = orgCheckContext.Companies.AsNoTracking().Where(_ => _.Name.ToUpper().Equals(name.ToUpper()) && _.Id != id && _.Status == 1).FirstOrDefault();
            else
                company = orgCheckContext.Companies.AsNoTracking().Where(_ => _.Name.ToUpper().Equals(name.ToUpper()) && _.Status == 1).FirstOrDefault();
            if (company != null && company.Id > 0)
                _result = true;

            return _result;
        }
        public void AddCompanyCredit(Companycredit companycredit)
        {
            orgCheckContext.Companycredits.Add(companycredit);
            var record = orgCheckContext.Companywallets.FirstOrDefault(_ => _.Companyid == companycredit.Companyid);
            if (record != null)
            {
                record.Totalcredit += companycredit.Credit;
                orgCheckContext.SaveChanges();
            }
        }
        public List<Companycredit> GetCompanycredits(int companyId)
        {
            var _query = orgCheckContext.Companycredits.Include(x => x.Company).AsNoTracking().Where(_ => _.Status == 1);
            if (companyId > 0)
                _query = _query.Where(_ => _.Companyid == companyId);
            return _query.OrderByDescending(_ => _.Id).Take(20).ToList();
        }
        public int GetCompanyBalance(int companyId)
        {
            int count = 0;
            if (companyId > 0)
            {
                var wallet = orgCheckContext.Companywallets.Include(x => x.Company).AsNoTracking().Where(_ => _.Companyid == companyId).FirstOrDefault();
                count = wallet.Totalcredit;
            }
            return count;
        }
        public void ReconcileCompanyCredit(int companyId, bool isEmp, bool isEdu)
        {
            var company = orgCheckContext.Companies.FirstOrDefault(_ => _.Id == companyId);
            var wallet = orgCheckContext.Companywallets.FirstOrDefault(_ => _.Companyid == companyId);
            if (wallet != null)
            {
                if (isEmp)
                    wallet.Totalcredit -= 1;
                else if (isEdu)
                    wallet.Totalcredit -= 1;
            }
            orgCheckContext.SaveChanges();
        }
    }
}
