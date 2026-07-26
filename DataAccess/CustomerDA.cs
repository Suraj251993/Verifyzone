using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrgCheck.DataAccess.Interfaces;
using OrgCheck.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace OrgCheck.DataAccess
{
    public class CustomerDA : ICustomerDA
    {
        public PostgresContext orgCheckContext;
        public CustomerDA(PostgresContext _orgCheckContext)
        {
            orgCheckContext = _orgCheckContext;
        }
        public List<Customer> GetCustomers(bool isEducation, bool isEmployment)
        {
            var query = orgCheckContext.Customers.AsNoTracking().Where(_ => _.Status == 1);
            if (isEducation && !isEmployment)
                query = query.Where(_ => _.Iseducation.Value);
            if (!isEducation && isEmployment)
                query = query.Where(_ => _.Isemployment.Value);
            if (isEducation && isEmployment)
                query = query.Where(_ => _.Iseducation.Value || isEmployment);

            return query.OrderBy(_ => _.Name).ToList();
        }
        public List<Customer> GetCustomers(string search)
        {
            return orgCheckContext.Customers.AsNoTracking().Where(_ => _.Status == 1 && _.Name.ToLower().Contains(search)).OrderBy(_ => _.Name).ToList();
        }
        public Customer GetCustomer(int Id)
        {
            return orgCheckContext.Customers.AsNoTracking().Where(_ => _.Id == Id).FirstOrDefault();
        }
        public Customer AddCustomer(Customer customer)
        {
            orgCheckContext.Customers.Add(customer);
            orgCheckContext.SaveChanges();
            Customerwallet wallet = new Customerwallet()
            {
                Customerid = customer.Id,
                Totalcredit = 0,
                Creditpending = 0
            };
            orgCheckContext.Customerwallets.Add(wallet);
            return customer;
        }
        public void AddCustomerWallet(Customerwallettransaction transaction)
        {
            orgCheckContext.Customerwallettransactions.Add(transaction);
            var setting = orgCheckContext.Customersettings.AsNoTracking().FirstOrDefault(_ => _.Customerid == transaction.Customerid);
            var wallet = orgCheckContext.Customerwallets.AsNoTracking().FirstOrDefault(_ => _.Customerid == transaction.Customerid);
            wallet.Creditpending += transaction.Credits;
            if(wallet.Creditpending > setting.Pendingcreditthreshold.Value)
            {
                wallet.Totalcredit += Convert.ToInt32(wallet.Creditpending / 5);
                wallet.Creditpending = wallet.Creditpending % 5;
            }
            orgCheckContext.SaveChanges();
        }
        public List<SelectListItem> GetCustomerWalletTransactions(int customerId)
        {
            var data = orgCheckContext.Customerwallettransactions.Include(x => x.TransactiontypeNavigation).AsNoTracking().AsEnumerable()
                .Where(x => x.Customerid == customerId)
                .GroupBy(x => x.TransactiontypeNavigation.Name)
                .Select(x => new SelectListItem()
                {
                    Text = x.Select(s => s.TransactiontypeNavigation.Name).FirstOrDefault(),
                    Value = x.Sum(c => c.Credits).ToString()
                }).ToList();
            return data;
        }
        public void UpdateCustomer(Customer customer)
        {
            var existingEntity = orgCheckContext.Customers.FirstOrDefault(_ => _.Id == customer.Id);
            existingEntity.Industrytype = customer.Industrytype;
            existingEntity.Name = customer.Name;
            existingEntity.Address = customer.Address;
            existingEntity.Contactname = customer.Contactname;
            existingEntity.Contactnumber = customer.Contactnumber;
            existingEntity.Email = customer.Email;
            existingEntity.CommencementDate = customer.CommencementDate;
            existingEntity.ClosedDate = customer.ClosedDate;
            existingEntity.GstNumber = customer.GstNumber;
            existingEntity.TanNumber = customer.TanNumber;
            existingEntity.PanNumber = customer.PanNumber;
            existingEntity.Iseducation = customer.Iseducation;
            existingEntity.Isemployment = customer.Isemployment;
            existingEntity.Isbgv = customer.Isbgv;
            existingEntity.Modifiedby = customer.Modifiedby;
            existingEntity.Modifieddate = customer.Modifieddate;
            orgCheckContext.SaveChanges();
        }
        public bool IsDuplicateCustomer(int id, string name)
        {
            bool _result = false;
            var customer = new Customer();
            if (id > 0)
                customer = orgCheckContext.Customers.AsNoTracking().Where(_ => _.Name.ToUpper().Equals(name.ToUpper()) && _.Id != id && _.Status == 1).FirstOrDefault();
            else
                customer = orgCheckContext.Customers.AsNoTracking().Where(_ => _.Name.ToUpper().Equals(name.ToUpper()) && _.Status == 1).FirstOrDefault();
            if (customer != null && customer.Id > 0)
                _result = true;

            return _result;
        }
        public List<Customeremailsetting> GetCustomeremailsettings(int customerId)
        {
            return orgCheckContext.Customeremailsettings.Include(x => x.Customer).Where(_ => _.Createdcustomerid == customerId).ToList();
        }
        public Customeremailsetting GetCustomerEmailsetting(string customerId, int templateId, int custId)
        {
            var query = orgCheckContext.Customeremailsettings.AsNoTracking().Where(_ => _.Templateid == templateId && _.Createdcustomerid == custId);
            if (!string.IsNullOrEmpty(customerId))
                query = query.Where(_ => _.Customerid == Convert.ToInt32(customerId));
            return query.FirstOrDefault();
        }
        public void AddCustomerEmailSetting(Customeremailsetting setting)
        {
            orgCheckContext.Customeremailsettings.Add(setting);
            orgCheckContext.SaveChanges();
        }
        public void UpdateCustomerEmailSetting(Customeremailsetting setting)
        {
            var entity = orgCheckContext.Customeremailsettings.AsNoTracking().Where(_ => _.Id == setting.Id).FirstOrDefault();
            entity.Templatecontent = setting.Templatecontent;
            orgCheckContext.SaveChanges();
        }

        public List<SelectListItem> GetCustomertypes()
        {
            return orgCheckContext.LookupCustomertypes.AsNoTracking()
                .OrderBy(_ => _.Id)
                .Select(_ => new SelectListItem()
                {
                    Text = _.Name,
                    Value = _.Id.ToString(),
                    Selected = false
                }).ToList();
        }
        public void AddCustomerCredit(Customercredit customercredit)
        {
            orgCheckContext.Customercredits.Add(customercredit);
            var record = orgCheckContext.Customerwallets.FirstOrDefault(_ => _.Customerid == customercredit.Customerid);
            record.Totalcredit += customercredit.Credit;
            orgCheckContext.SaveChanges();
        }
        public List<Customercredit> GetCustomercredits(int customerId)
        {
            var _query = orgCheckContext.Customercredits.Include(x => x.Customer).AsNoTracking().Where(_ => _.Status == 1);
            if (customerId > 0)
                _query = _query.Where(_ => _.Customerid == customerId);
            return _query.OrderByDescending(_ => _.Id).Take(20).ToList();
        }
        public int GetCustomerBalance(int customerId)
        {
            int count = 0;
            if (customerId > 0)
            {
                var wallet = orgCheckContext.Customerwallets.Include(x => x.Customer).AsNoTracking().Where(_ => _.Customerid == customerId).FirstOrDefault();
                count = wallet.Totalcredit;
            }
            return count;
        }
        public void ReconcileCustomerCredit(int customerId, int userId)
        {
            var customer = orgCheckContext.Customers.FirstOrDefault(_ => _.Id == customerId);
            var wallet = orgCheckContext.Customerwallets.FirstOrDefault(_ => _.Customerid == customerId);
            wallet.Totalcredit -= 1;
            var transaction = new Customerwallettransaction()
            {
                Customerid = customerId,
                Transactiontype = 3,
                Credits = 1.0,
                Remarks = "Report generated",
                Createdby = userId,
                Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Status = 1
            };
            orgCheckContext.Customerwallettransactions.Add(transaction);
            orgCheckContext.SaveChanges();
        }
        public void AddAutoApprovalConfig(Autoapprovalconfig config)
        {
            orgCheckContext.Autoapprovalconfigs.Add(config);
            orgCheckContext.SaveChanges();
        }
        public List<Autoapprovalconfig> GetAutoapprovalconfigsByUser(int userId)
        {
            return orgCheckContext.Autoapprovalconfigs.AsNoTracking().Where(_ => _.Createdby == userId && _.Status == 1)
                .OrderBy(_ => _.Id).ToList();
        }
        public List<Autoapprovalconfig> GetAutoapprovalconfigsByCustomer(int customerId)
        {
            return orgCheckContext.Autoapprovalconfigs.Include(x => x.CreatedbyNavigation).AsNoTracking()
                .Where(_ => _.CreatedbyNavigation.Customerid.Value == customerId && _.Status == 1)
                .OrderBy(_ => _.Id).ToList();
        }
        public void DeleteAutoApprovalConfig(int id)
        {
            var record = orgCheckContext.Autoapprovalconfigs.AsNoTracking().FirstOrDefault(_ => _.Id == id);
            record.Status = 0;
            orgCheckContext.SaveChanges();
        }

        public string AddAutoApprovalExclusion(Autoapprovalexclusion autoapprovalexclusion)
        {
            var count = orgCheckContext.Autoapprovalexclusions.AsNoTracking()
                .Where(_ => _.Employeeid == autoapprovalexclusion.Employeeid && _.Customerid == autoapprovalexclusion.Customerid).Count();
            if (count > 0)
            {
                return "Exists";
            }
            orgCheckContext.Autoapprovalexclusions.Add(autoapprovalexclusion);
            orgCheckContext.SaveChanges();
            return "True";
        }
        public Autoapprovalexclusion GetAutoapprovalexclusion(int customerId, int employeeId)
        {
            return orgCheckContext.Autoapprovalexclusions.AsNoTracking()
                .FirstOrDefault(_ => _.Customerid.Value == customerId && _.Employeeid.Value == employeeId);
        }
        public void DeleteAutoApprovalExclusion(int id)
        {
            var record = orgCheckContext.Autoapprovalexclusions.AsNoTracking().FirstOrDefault(_ => _.Id == id);
            orgCheckContext.Autoapprovalexclusions.Remove(record);
            orgCheckContext.SaveChanges();
        }
        public List<Autoapprovalexclusion> GetAllExclusions(int customerId)
        {
            return orgCheckContext.Autoapprovalexclusions.Include(x => x.Employee).Include(x => x.CreatedbyNavigation)
                .AsNoTracking().Where(_ => _.Customerid.Value == customerId).ToList();
        }
    }
}
