using System;
using System.Collections.Generic;
using System.Linq;
using OrgCheck.Models;
using OrgCheck.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using JetBrains.Annotations;
namespace OrgCheck.DataAccess
{
    public class UserDA : IUserDA
    {
        public PostgresContext orgCheckContext;
        public UserDA(PostgresContext _orgCheckContext)
        {
            orgCheckContext = _orgCheckContext;
        }
        public List<SelectListItem> GetUserTypes()
        {
            return orgCheckContext.LookupUsertypes.AsNoTracking().Where(_ => _.Status == 1)
                .OrderBy(_ => _.Id)
                .Select(_ => new SelectListItem()
                {
                    Text = _.Name,
                    Value = _.Id.ToString(),
                    Selected = false
                }).ToList();
        }
        public Login GetUser(string username, string password)
        {
            return orgCheckContext.Logins.Include(x => x.Usertype).AsNoTracking()
                .FirstOrDefault(_ => _.Loginname == username && _.Password == password && _.Status > 0);
        }
        public Login GetUserByType(string username, string password, int userTypeId)
        {
            return orgCheckContext.Logins.Include(x => x.Usertype).FirstOrDefault(_ => _.Loginname == username && _.Password == password && _.Usertypeid == userTypeId);
        }
        public Login GetUserByPrivilege(string username, string password, bool isEducation, bool isEmployment)
        {
            var result = new Login();
            var query = orgCheckContext.Logins.Include(x => x.Usertype).Include(x => x.Customer).AsNoTracking()
                .Where(_ => _.Loginname == username && _.Password == password && _.Status > 0).AsQueryable();
            if (isEducation)
            {
                result = query.Where(x => x.Customer.Iseducation == isEducation).FirstOrDefault();
                if (result != null)
                    return result;
                else
                    return new Login();
            }
            else if (isEmployment)
            {
                result = query.Where(x => x.Customer.Isemployment == isEmployment).FirstOrDefault();
                if (result != null)
                    return result;
                else
                    return new Login();
            }
            return result;
        }
        public Login GetUser(int userid, string password)
        {
            return orgCheckContext.Logins.Include(x => x.Usertype).AsNoTracking()
                .Where(_ => _.Id == userid && _.Password == password && _.Status > 0).FirstOrDefault();

        }

        public List<Login> GetUsers(int usertypeId)
        {
            var query = orgCheckContext.Logins.Include(x => x.Customer).Include(x => x.Usertype).AsNoTracking();
            if (usertypeId > 0)
                query = query.Where(_ => _.Usertypeid == usertypeId);
            return query.Where(_ => _.Status == 1).OrderBy(_ => _.Loginname).ToList();
        }
        public List<Login> GetUsersByCustomer(int customerId)
        {
            return orgCheckContext.Logins.Include(x => x.Customer).AsNoTracking()
                .Where(_ => _.Customerid == customerId && _.Status == 1).ToList();
        }
        public Login GetUser(int Id)
        {
            return orgCheckContext.Logins.Include(x => x.Usertype).Include(x => x.Customertype).Include(x => x.Customer)
                .AsNoTracking().Where(_ => _.Id == Id && _.Status > 0).FirstOrDefault();
        }
        public Login GetUserByEmail(int Id, string email)
        {
            var user = new Login();
            if (Id == 0)
                user = orgCheckContext.Logins.Include(x => x.Customer).Include(x => x.Usertype)
                    .AsNoTracking().Where(_ => _.Emailid == email && _.Status > 0).FirstOrDefault();
            else
                user = orgCheckContext.Logins.Include(x => x.Customer).Include(x => x.Usertype)
                    .Where(_ => _.Id != Id && _.Emailid == email && _.Status > 0).FirstOrDefault();
            return user;
        }
        public int AddUser(Login user)
        {
            orgCheckContext.Logins.Add(user);
            orgCheckContext.SaveChanges();
            return user.Id;
        }
        public void UpdateUser(Login user)
        {
            var existingEntity = orgCheckContext.Logins.FirstOrDefault(_ => _.Id == user.Id);
            existingEntity.Loginname = user.Loginname;
            existingEntity.Displayname = user.Displayname;
            existingEntity.Customerid = user.Customerid;
            existingEntity.Customertypeid = user.Customertypeid;
            existingEntity.Contactnumber = user.Contactnumber;
            existingEntity.Emailid = user.Emailid;
            existingEntity.Designation = user.Designation;
            existingEntity.Category = user.Category;
            orgCheckContext.SaveChanges();
        }
        public void UpdatePassword(Login user)
        {
            var existingEntity = orgCheckContext.Logins.FirstOrDefault(_ => _.Id == user.Id);
            existingEntity.Password = user.Password;
            orgCheckContext.SaveChanges();
        }

        //public void AddUserPrivilege(Loginprivilege user)
        //{
        //    orgCheckContext.Loginprivileges.Add(user);
        //    orgCheckContext.SaveChanges();
        //}
        //public void UpdateUserPrivilege(Loginprivilege user)
        //{
        //    var existingEntiry = orgCheckContext.Loginprivileges.FirstOrDefault(_ => _.Loginid == user.Loginid);
        //    existingEntiry.Exempverification = user.Exempverification;
        //    existingEntiry.Emplverification = user.Emplverification;
        //    existingEntiry.Studentverification = user.Studentverification;
        //    orgCheckContext.SaveChanges();
        //}
        //public Loginprivilege GetLoginprivilege(int userId)
        //{
        //    return orgCheckContext.Loginprivileges.FirstOrDefault(_ => _.Loginid == userId);
        //}

        public void UpdateProfile(Login login)
        {
            var existingEntity = orgCheckContext.Logins.FirstOrDefault(_ => _.Id == login.Id);
            existingEntity.Displayname = login.Displayname;
            existingEntity.Designation = login.Designation;
            existingEntity.Contactnumber = login.Contactnumber;
            existingEntity.Emailid = login.Emailid;
            existingEntity.Team = login.Team;
            existingEntity.Function = login.Function;
            existingEntity.Reportingmgrname = login.Reportingmgrname;
            existingEntity.Reportingmgrdesignation = login.Reportingmgrdesignation;
            orgCheckContext.SaveChanges();
        }
    }
}
