using Microsoft.EntityFrameworkCore;
using OrgCheck.DataAccess.Interfaces;
using OrgCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrgCheck.DataAccess
{
    public class ConsentDA : IConsentDA
    {
        public PostgresContext orgCheckContext;
        public ConsentDA(PostgresContext _orgCheckContext)
        {
            orgCheckContext = _orgCheckContext;
        }

        public string GenerateConsentRequestId()
        {
            // CR20260726-0001
            string template = $"CR{DateTime.Now:yyyyMMdd}-";
            string maxId = orgCheckContext.Consentrequests.AsNoTracking()
                .Where(_ => _.Consentrequestid.StartsWith(template)).OrderByDescending(_ => _.Id).Take(1)
                .Select(_ => _.Consentrequestid).FirstOrDefault();
            int currentSequence = 0;
            if (!string.IsNullOrEmpty(maxId))
                currentSequence = Convert.ToInt32(maxId.Substring(template.Length));

            return template + (currentSequence + 1).ToString().PadLeft(4, '0');
        }

        public Consentrequest AddConsentRequest(Consentrequest request)
        {
            orgCheckContext.Consentrequests.Add(request);
            orgCheckContext.SaveChanges();
            return request;
        }

        public List<Consentrequest> GetConsentRequests(int customerId, string name, string empcode, string email, int statusId, DateTime? fromDate, DateTime? toDate)
        {
            var query = orgCheckContext.Consentrequests.Include(x => x.Status).Include(x => x.CreatedbyNavigation)
                .AsNoTracking().Where(_ => _.Customerid == customerId).AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(_ => (_.Employeefirstname + " " + _.Employeelastname).ToLower().Contains(name.ToLower()));
            if (!string.IsNullOrEmpty(empcode))
                query = query.Where(_ => _.Employeecode != null && _.Employeecode.ToLower().Contains(empcode.ToLower()));
            if (!string.IsNullOrEmpty(email))
                query = query.Where(_ => _.Employeeemail.ToLower().Contains(email.ToLower()));
            if (statusId > 0)
                query = query.Where(_ => _.Statusid == statusId);
            if (fromDate.HasValue)
                query = query.Where(_ => _.Createddate.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(_ => _.Createddate.Date <= toDate.Value.Date);

            return query.OrderByDescending(_ => _.Createddate).ToList();
        }

        public Consentrequest GetConsentRequestByToken(string token)
        {
            return orgCheckContext.Consentrequests.Include(x => x.Customer).Include(x => x.Status)
                .FirstOrDefault(_ => _.Token == token);
        }

        public Consentrequest GetConsentRequestById(int id, int customerId)
        {
            return orgCheckContext.Consentrequests.Include(x => x.Status)
                .FirstOrDefault(_ => _.Id == id && _.Customerid == customerId);
        }

        public void UpdateConsentRequest(Consentrequest request)
        {
            orgCheckContext.Consentrequests.Update(request);
            orgCheckContext.SaveChanges();
        }

        public void AddAuditLog(Consentauditlog log)
        {
            orgCheckContext.Consentauditlogs.Add(log);
            orgCheckContext.SaveChanges();
        }

        public List<Consentauditlog> GetAuditLogs(int consentRequestId, int customerId)
        {
            return orgCheckContext.Consentauditlogs.Include(x => x.OldstatusNavigation).Include(x => x.NewstatusNavigation)
                .Include(x => x.PerformedbyNavigation).AsNoTracking()
                .Where(_ => _.Consentrequestid == consentRequestId && _.Consentrequest.Customerid == customerId)
                .OrderByDescending(_ => _.Createddate).ToList();
        }

        public List<LookupConsentstatus> GetConsentStatuses()
        {
            return orgCheckContext.LookupConsentstatuses.AsNoTracking().OrderBy(_ => _.Id).ToList();
        }
    }
}
