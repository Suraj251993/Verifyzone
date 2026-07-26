using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrgCheck.ViewModels;

namespace OrgCheck.Services.Interfaces
{
    public interface ICustomerService
    {
        List<CustomerViewModel> GetCustomers(bool isEducation, bool isEmployment);
        List<CustomerViewModel> GetCustomers(string search);

        CustomerViewModel GetCustomer(int id);
        string AddCustomer(CustomerViewModel viewModel);
        string UpdateCustomer(CustomerViewModel viewModel);
        List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCustomerTypes();
        List<CustomerCreditViewModel> GetCustomerCredits(int CustomerId);
        bool AddCustomerCredit(CustomerCreditViewModel viewModel);
        int GetCustomerBalance(int customerId);
        List<SelectListItem> GetWalletTransactions(int customerId);
        bool AddAutoApprovalConfig(AutoApprovalConfigViewModel viewModel);
        List<AutoApprovalConfigViewModel> GetAutoApprovalConfigs(int userId);
        bool DeleteAutoApprovalConfig(int id);
        string AddApprovalExclusion(ApprovalExclusionViewModel viewModel);
        bool DeleteApprovalExclusion(int id);
        List<ApprovalExclusionViewModel> GetApprovalExclusionsByCustomer(int customerId);
        List<CustomerEmailSettingViewModel> GetCustomerEmailSettings(int customerId);
        string GetEmailTemplate(string customerId, int templateId);
        string AddCustomerEmailSetting(CustomerEmailSettingViewModel viewModel);
    }
}
