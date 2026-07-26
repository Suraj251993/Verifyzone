using Microsoft.AspNetCore.Mvc.Rendering;
using OrgCheck.ViewModels;
using System;
using System.Collections.Generic;

namespace OrgCheck.Services.Interfaces
{
    public interface IConsentService
    {
        List<SelectListItem> GetConsentStatuses();
        List<ConsentListViewModel> GetConsentRequests(string name, string empcode, string email, int statusId, DateTime? fromDate, DateTime? toDate);
        string SendConsentRequest(ConsentRequestViewModel model, string baseUrl);
        ConsentTokenViewModel ValidateToken(string token);
        bool SubmitConsent(ConsentSubmitViewModel model, string ipAddress, string userAgent, out string message);
        List<ConsentAuditLogViewModel> GetAuditLogs(int consentRequestId);
        bool CancelConsentRequest(int id);
    }
}
