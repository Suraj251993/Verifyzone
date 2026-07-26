using System;
using Microsoft.Extensions.DependencyInjection;
using OrgCheck.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Linq;
using OrgCheck.ViewModels;
using AutoMapper;
using OrgCheck.Services.Interfaces;
using OrgCheck.Models;
using OrgCheck.Middleware;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrgCheck.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ExecutionContext _executionContext;
        private readonly IMapper _mapper;
        public CustomerService(IServiceProvider serviceProvider, ExecutionContext executionContext, IMapper mapper)
        {
            _serviceProvider = serviceProvider;
            _executionContext = executionContext;
            _mapper = mapper;
        }
        public List<CustomerViewModel> GetCustomers(bool isEducation, bool isEmployment)
        {
            var record = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomers(isEducation, isEmployment);
            return record.Select(_ => new CustomerViewModel()
            {
                Id = _.Id,
                Name = _.Name,
                Address = _.Address,
                Contactname = _.Contactname,
                Contactnumber = _.Contactnumber,
                Email = _.Email
            }).ToList();
        }
        public List<CustomerViewModel> GetCustomers(string search)
        {
            var record = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomers(search);
            return record.Select(_ => new CustomerViewModel()
            {
                Id = _.Id,
                Name = _.Name,
                Address = _.Address,
                Contactname = _.Contactname,
                Contactnumber = _.Contactnumber,
                Email = _.Email
            }).ToList();
        }
        public CustomerViewModel GetCustomer(int id)
        {
            var record = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomer(id);
            var result = _mapper.Map<CustomerViewModel>(record);
            result.Industry = record.Industrytype.Value;
            if (record.CommencementDate.HasValue)
                result.CommencementDate = record.CommencementDate.Value.ToString("dd-MM-yyyy");
            if (record.ClosedDate.HasValue)
                result.Closeddate = record.ClosedDate.Value.ToString("dd-MM-yyyy");
            result.IsEducation = record.Iseducation.Value;
            result.IsEmployment = record.Isemployment.Value;
            return result;
        }
        public string AddCustomer(CustomerViewModel viewModel)
        {
            var record = _mapper.Map<Customer>(viewModel);
            record.Industrytype = viewModel.Industry;
            if (!string.IsNullOrEmpty(viewModel.CommencementDate))
                record.CommencementDate = DateTime.ParseExact(viewModel.CommencementDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(viewModel.Closeddate))
                record.ClosedDate = DateTime.ParseExact(viewModel.Closeddate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            record.Iseducation = viewModel.IsEducation;
            record.Isemployment = viewModel.IsEmployment;
            record.Isbgv = viewModel.IsBGV;
            record.Createdby = _executionContext.UserId;
            record.Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            record.Charges = 50.0;
            if (_serviceProvider.GetRequiredService<ICustomerDA>().IsDuplicateCustomer(0, viewModel.Name))
                return "exists";
            _serviceProvider.GetRequiredService<ICustomerDA>().AddCustomer(record);            
            return "true";
        }
        public string UpdateCustomer(CustomerViewModel viewModel)
        {
            var record = _mapper.Map<Customer>(viewModel);
            record.Industrytype = viewModel.Industry;
            if (!string.IsNullOrEmpty(viewModel.CommencementDate))
                record.CommencementDate = DateTime.ParseExact(viewModel.CommencementDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(viewModel.Closeddate))
                record.ClosedDate = DateTime.ParseExact(viewModel.Closeddate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            record.Iseducation = viewModel.IsEducation;
            record.Isemployment = viewModel.IsEmployment;
            record.Isbgv = viewModel.IsBGV;
            record.Modifiedby = _executionContext.UserId;
            record.Modifieddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            if (_serviceProvider.GetRequiredService<ICustomerDA>().IsDuplicateCustomer(viewModel.Id, viewModel.Name))
                return "exists";
            _serviceProvider.GetRequiredService<ICustomerDA>().UpdateCustomer(record);
            return "true";
        }
        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCustomerTypes()
        {
            return _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomertypes();
        }
        public List<CustomerCreditViewModel> GetCustomerCredits(int CustomerId)
        {
            var list = new List<CustomerCreditViewModel>();
            var results = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomercredits(CustomerId);
            foreach (var result in results)
            {
                list.Add(new CustomerCreditViewModel()
                {
                    Id = result.Id,
                    Customerid = result.Customerid,
                    CustomerName = result.Customer.Name,
                    Credit = result.Credit,
                    Transactiontype = result.Transactiontype,
                    Referenceno = result.Referenceno,
                    Remarks = result.Remarks,
                    CreditDate = result.Createddate
                });
            }
            return list;
        }
        public bool AddCustomerCredit(CustomerCreditViewModel viewModel)
        {
            var record = _mapper.Map<Customercredit>(viewModel);
            record.Createdby = _executionContext.UserId;
            record.Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            _serviceProvider.GetRequiredService<ICustomerDA>().AddCustomerCredit(record);
            return true;
        }
        public int GetCustomerBalance(int customerId)
        {
            return _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomerBalance(customerId);
        }
        public List<SelectListItem> GetWalletTransactions(int customerId)
        {
            return _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomerWalletTransactions(customerId);
        }
        public bool AddAutoApprovalConfig(AutoApprovalConfigViewModel viewModel)
        {
            Autoapprovalconfig config = new()
            {
                Startdate = DateTime.ParseExact(viewModel.startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToUniversalTime(),
                Enddate = DateTime.ParseExact(viewModel.endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToUniversalTime(),
                Createdby = _executionContext.UserId,
                Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
            };
            _serviceProvider.GetRequiredService<ICustomerDA>().AddAutoApprovalConfig(config);
            return true;
        }
        public List<AutoApprovalConfigViewModel> GetAutoApprovalConfigs(int userId)
        {
            return _serviceProvider.GetRequiredService<ICustomerDA>().GetAutoapprovalconfigsByUser(userId)
                .Select(_ => new AutoApprovalConfigViewModel()
                {
                    id = _.Id,
                    startDate = _.Startdate.ToString("dd-MM-yyyy"),
                    endDate = _.Enddate.ToString("dd-MM-yyyy")
                }).ToList();
        }
        public bool DeleteAutoApprovalConfig(int id)
        {
            _serviceProvider.GetRequiredService<ICustomerDA>().DeleteAutoApprovalConfig(id);
            return true;
        }
        public string AddApprovalExclusion(ApprovalExclusionViewModel viewModel)
        {
            Autoapprovalexclusion exclusion = new Autoapprovalexclusion()
            {
                Customerid = viewModel.customerId,
                Employeeid = viewModel.employeeId,
                Createdby = Convert.ToInt32(viewModel.excludedBy),
                Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
            };
            return _serviceProvider.GetRequiredService<ICustomerDA>().AddAutoApprovalExclusion(exclusion);
        }
        public bool DeleteApprovalExclusion(int id)
        {
            _serviceProvider.GetRequiredService<ICustomerDA>().DeleteAutoApprovalExclusion(id);
            return true;
        }
        public List<ApprovalExclusionViewModel> GetApprovalExclusionsByCustomer(int customerId)
        {
            var data = new List<ApprovalExclusionViewModel>();
            var list = _serviceProvider.GetRequiredService<ICustomerDA>().GetAllExclusions(customerId);
            foreach (var exclusion in list)
            {
                data.Add(new ApprovalExclusionViewModel()
                {
                    id = exclusion.Id,
                    employeeId = exclusion.Employee.Id,
                    empCode = exclusion.Employee.Employeecode,
                    name = _serviceProvider.GetRequiredService<CryptoService>().Decrypt(exclusion.Employee.Name),
                    excludedBy = exclusion.CreatedbyNavigation.Displayname,
                    excludedDate = exclusion.Createddate.ToString("dd-MM-yyyy")
                });
            }
            return data;
        }
        public string GetEmailTemplate(string customerId, int templateId)
        {
            string emailTemplate = "";
            var setting = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomerEmailsetting(customerId, templateId, _executionContext.CustomerId);
            if (setting == null || string.IsNullOrEmpty(setting.Templatecontent))
            {
                emailTemplate = $"Dear user,\n\nGreetings from VerifyZone!!!\nWe request you to verify the details claimed by an ex-employee who worked in your esteemed organization.\n";
                emailTemplate += $"Kindly login into the VerifyZone portal and approve accordingly.\n\nThanks and Regards,\r\nVerifyZone IT Support team";
            }
            else
                emailTemplate = setting.Templatecontent;
            return emailTemplate;
        }
        public List<CustomerEmailSettingViewModel> GetCustomerEmailSettings(int customerId)
        {
            var list = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomeremailsettings(customerId)
                .Select(_ => new CustomerEmailSettingViewModel()
                {
                    Customerid = _.Customerid != null ? _.Customerid.Value.ToString() : "",
                    Customername = _.Customer != null ? _.Customer.Name : "-",
                    Templateid = _.Templateid.ToString(),
                    Templatecontent = _.Templatecontent
                }).ToList();
            return list;
        }
        public string AddCustomerEmailSetting(CustomerEmailSettingViewModel viewModel)
        {
            Customeremailsetting setting = new Customeremailsetting()
            {
                Customerid = (viewModel.Customerid == "" ? null : Convert.ToInt32(viewModel.Customerid)),
                Templateid = Convert.ToInt32(viewModel.Templateid),
                Templatecontent = viewModel.Templatecontent,
                Createdby = _executionContext.UserId,
                Createdcustomerid = _executionContext.CustomerId,
                Createddate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
            };
            var existingentity = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomerEmailsetting(viewModel.Customerid, Convert.ToInt32(viewModel.Templateid), _executionContext.CustomerId);
            if (existingentity != null) { return "exists"; }
            _serviceProvider.GetRequiredService<ICustomerDA>().AddCustomerEmailSetting(setting);
            return "true";
        }
    }
}
