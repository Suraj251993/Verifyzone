using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrgCheck.Models;

namespace OrgCheck.DataAccess.Interfaces
{
    public interface ICustomerDA
    {
        List<Customer> GetCustomers(bool isEducation, bool isEmployment);
        List<Customer> GetCustomers(string search);

        Customer GetCustomer(int Id);
        Customer AddCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        bool IsDuplicateCustomer(int id, string name);
        List<Customeremailsetting> GetCustomeremailsettings(int customerId);
        Customeremailsetting GetCustomerEmailsetting(string customerId, int templateId, int custId);
        void AddCustomerEmailSetting(Customeremailsetting setting);
        void UpdateCustomerEmailSetting(Customeremailsetting setting);
        List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCustomertypes();
        void AddCustomerWallet(Customerwallettransaction transaction);
        List<SelectListItem> GetCustomerWalletTransactions(int customerId);
        void AddCustomerCredit(Customercredit customercredit);
        List<Customercredit> GetCustomercredits(int customerId);
        int GetCustomerBalance(int customerId);
        void ReconcileCustomerCredit(int customerId, int userId);
        void AddAutoApprovalConfig(Autoapprovalconfig config);
        List<Autoapprovalconfig> GetAutoapprovalconfigsByUser(int userId);
        List<Autoapprovalconfig> GetAutoapprovalconfigsByCustomer(int customerId);
        void DeleteAutoApprovalConfig(int id);
        string AddAutoApprovalExclusion(Autoapprovalexclusion autoapprovalexclusion);
        Autoapprovalexclusion GetAutoapprovalexclusion(int customerId, int employeeId);
        void DeleteAutoApprovalExclusion(int id);
        List<Autoapprovalexclusion> GetAllExclusions(int customerId);
    }
}
