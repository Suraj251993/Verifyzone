using OrgCheck.Models;
using System;
using System.Collections.Generic;

namespace OrgCheck.DataAccess.Interfaces
{
    public interface IConsentDA
    {
        string GenerateConsentRequestId();
        Consentrequest AddConsentRequest(Consentrequest request);
        List<Consentrequest> GetConsentRequests(int customerId, string name, string empcode, string email, int statusId, DateTime? fromDate, DateTime? toDate);
        Consentrequest GetConsentRequestByToken(string token);
        Consentrequest GetConsentRequestById(int id, int customerId);
        void UpdateConsentRequest(Consentrequest request);
        void AddAuditLog(Consentauditlog log);
        List<Consentauditlog> GetAuditLogs(int consentRequestId, int customerId);
        List<LookupConsentstatus> GetConsentStatuses();
    }
}
