using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using OrgCheck.DataAccess.Interfaces;
using OrgCheck.Middleware;
using OrgCheck.Models;
using OrgCheck.Services.Interfaces;
using OrgCheck.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;

namespace OrgCheck.Services
{
    public class ConsentService : IConsentService
    {
        private const int TokenValidityDays = 7;
        private readonly IServiceProvider _serviceProvider;
        private readonly ExecutionContext _executionContext;
        private readonly EmailService _emailService;
        public ConsentService(IServiceProvider serviceProvider, ExecutionContext executionContext, EmailService emailService)
        {
            _serviceProvider = serviceProvider;
            _executionContext = executionContext;
            _emailService = emailService;
        }

        public List<SelectListItem> GetConsentStatuses()
        {
            return _serviceProvider.GetRequiredService<IConsentDA>().GetConsentStatuses()
                .Select(_ => new SelectListItem { Value = _.Id.ToString(), Text = _.Name }).ToList();
        }

        public List<ConsentListViewModel> GetConsentRequests(string name, string empcode, string email, int statusId, DateTime? fromDate, DateTime? toDate)
        {
            var consentDA = _serviceProvider.GetRequiredService<IConsentDA>();
            ExpirePendingRequests(consentDA.GetConsentRequests(_executionContext.CustomerId, null, null, null, 1, null, null));

            var records = consentDA.GetConsentRequests(_executionContext.CustomerId, name, empcode, email, statusId, fromDate, toDate);
            return records.Select(_ => new ConsentListViewModel
            {
                Id = _.Id,
                Consentrequestid = _.Consentrequestid,
                Employeename = $"{_.Employeefirstname} {_.Employeelastname}",
                Employeecode = _.Employeecode,
                Employeeemail = _.Employeeemail,
                Requestdate = _.Createddate,
                Statusid = _.Statusid,
                Status = _.Status?.Name,
                Consentdate = _.Consentdate,
                Lastupdated = _.Modifieddate ?? _.Createddate
            }).ToList();
        }

        private void ExpirePendingRequests(List<Consentrequest> pendingRequests)
        {
            if (pendingRequests == null || pendingRequests.Count == 0)
                return;

            var consentDA = _serviceProvider.GetRequiredService<IConsentDA>();
            foreach (var request in pendingRequests.Where(_ => !_.Tokenconsumed && _.Tokenexpirydate < DateTime.UtcNow))
            {
                request.Statusid = 3; // Expired
                request.Modifieddate = DateTime.UtcNow;
                consentDA.UpdateConsentRequest(request);
                consentDA.AddAuditLog(new Consentauditlog
                {
                    Consentrequestid = request.Id,
                    Action = "Expired",
                    Oldstatusid = 1,
                    Newstatusid = 3,
                    Createddate = DateTime.UtcNow
                });
            }
        }

        public string SendConsentRequest(ConsentRequestViewModel model, string baseUrl)
        {
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true))
                throw new Exception(string.Join(" ", validationResults.Select(_ => _.ErrorMessage)));

            var consentDA = _serviceProvider.GetRequiredService<IConsentDA>();
            string token = GenerateSecureToken();
            var request = new Consentrequest
            {
                Consentrequestid = consentDA.GenerateConsentRequestId(),
                Customerid = _executionContext.CustomerId,
                Employeefirstname = model.Firstname.Trim(),
                Employeelastname = model.Lastname.Trim(),
                Employeecode = string.IsNullOrWhiteSpace(model.Employeecode) ? null : model.Employeecode.Trim(),
                Employeeemail = model.Employeeemail.Trim(),
                Statusid = 1,
                Token = token,
                Tokenconsumed = false,
                Tokenexpirydate = DateTime.UtcNow.AddDays(TokenValidityDays),
                Createdby = _executionContext.UserId,
                Createddate = DateTime.UtcNow
            };
            consentDA.AddConsentRequest(request);
            consentDA.AddAuditLog(new Consentauditlog
            {
                Consentrequestid = request.Id,
                Action = "Created",
                Newstatusid = 1,
                Performedby = _executionContext.UserId,
                Createddate = DateTime.UtcNow
            });

            var customer = _serviceProvider.GetRequiredService<ICustomerDA>().GetCustomer(_executionContext.CustomerId);
            var hrUser = _serviceProvider.GetRequiredService<IUserDA>().GetUser(_executionContext.UserId);
            string consentLink = $"{baseUrl.TrimEnd('/')}/Consent/Index?token={token}";
            _emailService.SendConsentRequestEmail(
                request.Employeeemail,
                $"{request.Employeefirstname} {request.Employeelastname}",
                customer?.Name ?? string.Empty,
                consentLink,
                request.Tokenexpirydate.ToString("dd-MMM-yyyy"),
                hrUser != null ? $"{hrUser.Displayname} ({hrUser.Emailid})" : string.Empty);

            consentDA.AddAuditLog(new Consentauditlog
            {
                Consentrequestid = request.Id,
                Action = "EmailSent",
                Performedby = _executionContext.UserId,
                Createddate = DateTime.UtcNow
            });

            return request.Consentrequestid;
        }

        public ConsentTokenViewModel ValidateToken(string token)
        {
            var result = new ConsentTokenViewModel { Token = token };
            if (string.IsNullOrWhiteSpace(token))
                return result;

            var consentDA = _serviceProvider.GetRequiredService<IConsentDA>();
            var request = consentDA.GetConsentRequestByToken(token);
            if (request == null)
                return result;

            if (!request.Tokenconsumed && request.Tokenexpirydate < DateTime.UtcNow && request.Statusid == 1)
            {
                request.Statusid = 3; // Expired
                request.Modifieddate = DateTime.UtcNow;
                consentDA.UpdateConsentRequest(request);
                consentDA.AddAuditLog(new Consentauditlog
                {
                    Consentrequestid = request.Id,
                    Action = "Expired",
                    Oldstatusid = 1,
                    Newstatusid = 3,
                    Createddate = DateTime.UtcNow
                });
            }

            if (request.Tokenconsumed)
            {
                result.Consumed = true;
                return result;
            }
            if (request.Tokenexpirydate < DateTime.UtcNow || request.Statusid == 3)
            {
                result.Expired = true;
                return result;
            }

            result.Valid = true;
            result.Employeename = $"{request.Employeefirstname} {request.Employeelastname}";
            result.Employeeemail = request.Employeeemail;
            result.Companyname = request.Customer?.Name;
            return result;
        }

        public bool SubmitConsent(ConsentSubmitViewModel model, string ipAddress, string userAgent, out string message)
        {
            message = string.Empty;
            var consentDA = _serviceProvider.GetRequiredService<IConsentDA>();
            var request = consentDA.GetConsentRequestByToken(model.Token);
            if (request == null)
            {
                message = "Invalid or unavailable consent request.";
                return false;
            }
            if (request.Tokenconsumed)
            {
                message = "This consent request has already been submitted.";
                return false;
            }
            if (request.Tokenexpirydate < DateTime.UtcNow || request.Statusid == 3)
            {
                message = "This consent request has expired. Please contact your HR representative.";
                return false;
            }
            if (!model.Consentaccepted)
            {
                message = "Please accept the consent statement before submitting.";
                return false;
            }
            if (!string.IsNullOrWhiteSpace(model.Optionalemail))
            {
                var emailValidator = new EmailAddressAttribute();
                if (!emailValidator.IsValid(model.Optionalemail))
                {
                    message = "Please enter a valid email address.";
                    return false;
                }
            }

            var (device, browser) = ParseUserAgent(userAgent);
            request.Statusid = 2; // Approved
            request.Consentdate = DateTime.UtcNow;
            request.Ipaddress = ipAddress;
            request.Device = device;
            request.Browser = browser;
            request.Optionalemail = string.IsNullOrWhiteSpace(model.Optionalemail) ? null : model.Optionalemail.Trim();
            request.Tokenconsumed = true;
            request.Modifieddate = DateTime.UtcNow;
            consentDA.UpdateConsentRequest(request);

            consentDA.AddAuditLog(new Consentauditlog
            {
                Consentrequestid = request.Id,
                Action = "Submitted",
                Oldstatusid = 1,
                Newstatusid = 2,
                Ipaddress = ipAddress,
                Useragent = userAgent,
                Createddate = DateTime.UtcNow
            });
            return true;
        }

        public List<ConsentAuditLogViewModel> GetAuditLogs(int consentRequestId)
        {
            return _serviceProvider.GetRequiredService<IConsentDA>().GetAuditLogs(consentRequestId, _executionContext.CustomerId)
                .Select(_ => new ConsentAuditLogViewModel
                {
                    Action = _.Action,
                    Oldstatus = _.OldstatusNavigation?.Name,
                    Newstatus = _.NewstatusNavigation?.Name,
                    Performedby = _.PerformedbyNavigation?.Displayname,
                    Ipaddress = _.Ipaddress,
                    Remarks = _.Remarks,
                    Createddate = _.Createddate
                }).ToList();
        }

        public bool CancelConsentRequest(int id)
        {
            var consentDA = _serviceProvider.GetRequiredService<IConsentDA>();
            var request = consentDA.GetConsentRequestById(id, _executionContext.CustomerId);
            if (request == null || request.Statusid != 1)
                return false;

            request.Statusid = 4; // Cancelled
            request.Modifiedby = _executionContext.UserId;
            request.Modifieddate = DateTime.UtcNow;
            consentDA.UpdateConsentRequest(request);
            consentDA.AddAuditLog(new Consentauditlog
            {
                Consentrequestid = request.Id,
                Action = "Cancelled",
                Oldstatusid = 1,
                Newstatusid = 4,
                Performedby = _executionContext.UserId,
                Createddate = DateTime.UtcNow
            });
            return true;
        }

        private string GenerateSecureToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        private (string device, string browser) ParseUserAgent(string userAgent)
        {
            string device = "Desktop";
            string browser = "Unknown";
            if (!string.IsNullOrEmpty(userAgent))
            {
                if (userAgent.Contains("iPad") || userAgent.Contains("Tablet"))
                    device = "Tablet";
                else if (userAgent.Contains("Mobi") || userAgent.Contains("Android") || userAgent.Contains("iPhone"))
                    device = "Mobile";

                if (userAgent.Contains("Edg/"))
                    browser = "Edge";
                else if (userAgent.Contains("Chrome/"))
                    browser = "Chrome";
                else if (userAgent.Contains("Firefox/"))
                    browser = "Firefox";
                else if (userAgent.Contains("Safari/"))
                    browser = "Safari";
            }
            return (device, browser);
        }
    }
}
