using OrgCheck.Models;
using System;
using System.Collections.Generic;

namespace OrgCheck.DataAccess.Interfaces
{
    public interface IUserDA
    {
        List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetUserTypes();
        Login GetUser(string username, string password);
        Login GetUserByType(string username, string password, int userTypeId);
        Login GetUserByPrivilege(string username, string password, bool isEducation, bool isEmployment);
        Login GetUser(int userid, string password);

        List<Login> GetUsers(int usertypeId);
        List<Login> GetUsersByCustomer(int customerId);
        Login GetUser(int Id);
        Login GetUserByEmail(int Id, string email);
        int AddUser(Login user);
        void UpdateUser(Login user);
        void UpdatePassword(Login user);

        //void AddUserPrivilege(Loginprivilege user);
        //void UpdateUserPrivilege(Loginprivilege user);
        //Loginprivilege GetLoginprivilege(int userId);

        void UpdateProfile(Login login);
    }
}
