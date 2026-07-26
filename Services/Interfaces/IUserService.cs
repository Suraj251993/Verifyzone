using OrgCheck.ViewModels;
using System;
using System.Collections.Generic;
namespace OrgCheck.Services.Interfaces
{
    public interface IUserService
    {
        List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetUserTypes();
        UserViewModel GetUser(LoginViewModel viewModel);
        UserViewModel CheckUser(LoginViewModel viewModel);
        UserViewModel CheckUserByType(LoginViewModel viewModel, int userTypeId);
        List<UserViewModel> GetAllUsers(int usertypeId);
        UserViewModel GetUser(int id);
        string AddUser(UserViewModel viewModel);
        string UpdateUser(UserViewModel viewModel);
        bool CheckOldPassword(int id, string password, int usertype);
        bool UpdatePassword(int id, string password);
        bool ResetPassword(int id);
        bool ForgotPassword(string emailId);

        UserProfileViewModel GetUserProfile(int id);
        bool UpdateUserProfile(UserProfileViewModel model);
    }
}
