using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.DependencyInjection;
using OrgCheck.DataAccess;
using OrgCheck.DataAccess.Interfaces;
using OrgCheck.Models;
using OrgCheck.Services.Interfaces;
using OrgCheck.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrgCheck.Services
{
    public class UserService : IUserService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Constants _constants;
        private readonly EmailService _emailService;
        private static Random random = new Random();
        public UserService(IServiceProvider serviceProvider, Constants constants, EmailService emailService)
        {
            _serviceProvider = serviceProvider;
            _constants = constants;
            _emailService = emailService;
        }
        private string HashedPassword(string password)
        {
            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(_constants.SecretKey),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            return hashed;
        }
        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetUserTypes()
        {
            return _serviceProvider.GetRequiredService<IUserDA>().GetUserTypes();
        }
        public UserViewModel GetUser(LoginViewModel viewModel)
        {
            var hashPassword = HashedPassword(viewModel.Password);
            var login = _serviceProvider.GetRequiredService<IUserDA>().GetUser(viewModel.LoginName, hashPassword);
            var data = new UserViewModel();
            if (login != null)
            {
                //var privilege = _serviceProvider.GetRequiredService<IUserDA>().GetLoginprivilege(login.Id);
                data = new UserViewModel()
                {
                    Id = login.Id,
                    DisplayName = login.Displayname,
                    LoginName = login.Loginname,
                    UserType = login.Usertype.Id,
                    UserTypename = login.Usertype.Name,
                    CustomerType = login.Customertypeid.GetValueOrDefault(),
                    CustomerId = (login.Customerid.HasValue ? login.Customerid.Value : 0),
                    //IsEmpVerification = privilege.Emplverification,
                    //IsExEmpVerification = privilege.Exempverification,
                    //IsStudentVerification = privilege.Studentverification,
                };
            }
            return data;
        }
        public UserViewModel CheckUser(LoginViewModel viewModel)
        {
            var hashPassword = HashedPassword(viewModel.Password);
            var login = _serviceProvider.GetRequiredService<IUserDA>().GetUserByPrivilege(viewModel.LoginName, hashPassword, viewModel.isEducation, viewModel.isEmployment);
            var data = new UserViewModel();
            if (login.Id > 0)
            {
                data = new UserViewModel()
                {
                    Id = login.Id,
                    DisplayName = login.Displayname,
                    LoginName = login.Loginname,
                    UserType = login.Usertypeid,
                    UserTypename = login.Usertype.Name,
                    CustomerType = login.Customertypeid.GetValueOrDefault(),
                    CustomerId = (login.Customerid.HasValue ? login.Customerid.Value : 0),
                    IsBGV = login.Customer.Isbgv.Value,
                    IsEducation = login.Customer.Iseducation.Value,
                    IsEmployment = login.Customer.Isemployment.Value
                };                
            }
            return data;
        }
        public UserViewModel CheckUserByType(LoginViewModel viewModel, int userTypeId)
        {
            var hashPassword = HashedPassword(viewModel.Password);
            var login = _serviceProvider.GetRequiredService<IUserDA>().GetUserByType(viewModel.LoginName, hashPassword, userTypeId);
            var data = new UserViewModel();
            if (login != null)
            {
                data = new UserViewModel()
                {
                    Id = login.Id,
                    DisplayName = login.Displayname,
                    LoginName = login.Loginname,
                    UserType = login.Usertype.Id,
                    UserTypename = login.Usertype.Name,
                    CustomerType = (login.Customertypeid == null ? 0 : login.Customertypeid.GetValueOrDefault()),
                    CustomerId = (login.Customertypeid == null ? 0 : login.Customerid.GetValueOrDefault())
                };
            }
            return data;
        }

        public List<UserViewModel> GetAllUsers(int usertypeId)
        {
            var data = _serviceProvider.GetRequiredService<IUserDA>().GetUsers(usertypeId);
            return data.Select(_ => new UserViewModel()
            {
                Id = _.Id,
                LoginName = _.Loginname,
                DisplayName = _.Displayname,
                Emailid = _.Emailid,
                UserTypename = _.Usertype.Name,
                Customername = _.Customer?.Name ?? String.Empty,
                Contactnumber = _.Contactnumber,
                Designation = _.Designation
            }).ToList();
        }
        public UserViewModel GetUser(int id)
        {
            var data = _serviceProvider.GetRequiredService<IUserDA>().GetUser(id);
            var result = new UserViewModel()
            {
                Id = data.Id,
                LoginName = data.Loginname,
                DisplayName = data.Displayname,
                Emailid = data.Emailid,
                UserTypename = data.Usertype.Name,
                UserType = data.Usertypeid,
                CustomerId = data.Customerid.HasValue ? data.Customerid.Value : 0,
                CustomerType = data.Customertypeid.HasValue ? data.Customertypeid.Value : 0,
                Customername = data.Customer?.Name ?? String.Empty,
                Contactnumber = data.Contactnumber,
                Designation = data.Designation,
                Category = data.Category.HasValue ? data.Category.Value : 0
            };
            //var privilege = _serviceProvider.GetRequiredService<IUserDA>().GetLoginprivilege(id);
            //result.IsExEmpVerification = privilege.Exempverification;
            //result.IsEmpVerification = privilege.Emplverification;
            //result.IsStudentVerification = privilege.Studentverification;
            return result;
        }
        public string AddUser(UserViewModel viewModel)
        {
            var newPassword = GenerateRandomPassword();
            var data = new Login()
            {
                Loginname = viewModel.LoginName,
                Displayname = viewModel.DisplayName,
                Emailid = viewModel.Emailid,
                Customerid = (viewModel.CustomerId > 0 ? viewModel.CustomerId : null),
                Customertypeid = (viewModel.CustomerType > 0 ? viewModel.CustomerType : null),
                Usertypeid = viewModel.UserType,
                Password = HashedPassword(newPassword),
                Contactnumber = viewModel.Contactnumber,
                Designation = viewModel.Designation,
                Category = viewModel.Category,
                Status = 1
            };
            var exists = _serviceProvider.GetRequiredService<IUserDA>().GetUserByEmail(0, viewModel.Emailid);
            if (exists != null && exists.Id > 0)
                return "exists";
            int userId = _serviceProvider.GetRequiredService<IUserDA>().AddUser(data);

            //var privilege = new Loginprivilege()
            //{
            //    Loginid = userId,
            //    Exempverification = viewModel.IsExEmpVerification,
            //    Emplverification = viewModel.IsEmpVerification,
            //    Studentverification = viewModel.IsStudentVerification,
            //};
            //_serviceProvider.GetRequiredService<IUserDA>().AddUserPrivilege(privilege);

            // Email the temporary password to the user
            string emailBody = $"Dear {viewModel.DisplayName},<br><br>Greetings!! Welcome to VerifyZone.<br><br>Below are your credentials to login to the portal.<br><br>";
            emailBody += $"Username: {viewModel.Emailid}<br>Password: {newPassword}.<br>";
            emailBody += $"URL: https://app.verifyzone.in/";
            emailBody += "<br>Kindly change the password once you logged into the portal.<br><br>";
            emailBody += $"For any queries please reach out it.support@verifyzone.in<br><br>Thank you,<br>VerifyZone support team";
            
            string emailSubject = "VerifyZone - User credentials";
            string emailTo = viewModel.Emailid;
            _emailService.SendEmail(emailTo, string.Empty, string.Empty, emailSubject, emailBody);
            return "true";
        }
        public string UpdateUser(UserViewModel viewModel)
        {
            var data = new Login()
            {
                Id = viewModel.Id,
                Loginname = viewModel.LoginName,
                Displayname = viewModel.DisplayName,
                Emailid = viewModel.Emailid,
                Customerid = (viewModel.CustomerId > 0 ? viewModel.CustomerId : null),
                Customertypeid = (viewModel.CustomerType > 0 ? viewModel.CustomerType : null),
                Usertypeid = viewModel.UserType,
                Contactnumber = viewModel.Contactnumber,
                Designation = viewModel.Designation,
                Category = viewModel.Category,
                Status = 1
            };
            var exists = _serviceProvider.GetRequiredService<IUserDA>().GetUserByEmail(viewModel.Id, viewModel.Emailid);
            if (exists != null && exists.Id != viewModel.Id)
                return "exists";
            _serviceProvider.GetRequiredService<IUserDA>().UpdateUser(data);
                        
            //var privilege = new Loginprivilege()
            //{
            //    Loginid = viewModel.Id,
            //    Exempverification = viewModel.IsExEmpVerification,
            //    Emplverification = viewModel.IsEmpVerification,
            //    Studentverification = viewModel.IsStudentVerification,
            //};
            //_serviceProvider.GetRequiredService<IUserDA>().UpdateUserPrivilege(privilege);
            return "true";
        }
        public bool CheckOldPassword(int id, string password, int usertype)
        {
            var user = GetUser(id);
            var hashPassword = HashedPassword(password);
            var login = _serviceProvider.GetRequiredService<IUserDA>().GetUserByType(user.LoginName, hashPassword, usertype);
            if (login != null)
                return true;
            else
                return false;
        }
        public bool UpdatePassword(int id, string password)
        {
            var hashPassword = HashedPassword(password);
            var login = new Login()
            {
                Id = id,
                Password = hashPassword
            };
            _serviceProvider.GetRequiredService<IUserDA>().UpdatePassword(login);
            return true;
        }
        public bool ResetPassword(int id)
        {
            var user = _serviceProvider.GetRequiredService<IUserDA>().GetUser(id);
            var newPassword = GenerateRandomPassword();
            var hashPassword = HashedPassword(newPassword);
            var login = new Login()
            {
                Id = id,
                Password = hashPassword
            };
            _serviceProvider.GetRequiredService<IUserDA>().UpdatePassword(login);

            string emailBody = $"Hi {user.Displayname},<br><br>Your password has been reset and the new temporary password is {newPassword}.<br><br>Kindly use this to login into the VerifyZone portal and change the password";
            string emailSubject = "VerifyZone - Password reset request";
            string emailTo = user.Emailid;
            _emailService.SendEmail(emailTo, string.Empty, string.Empty, emailSubject, emailBody);
            return true;
        }
        public bool ForgotPassword(string emailId)
        {
            var user = _serviceProvider.GetRequiredService<IUserDA>().GetUserByEmail(0, emailId);
            if (user == null || user.Id == 0)
                return false;
            string emailBody = $"Hi team,<br><br>I had forgot my password. Kindly reset it and let me know.";
            emailBody += $"<br><br>My email id: {emailId} for your reference.<br><br>Thank you.";
            string emailSubject = "VerifyZone - Forgot password request";
            string emailTo = _constants.EmailFromId;
            _emailService.SendEmail(emailTo, string.Empty, string.Empty, emailSubject, emailBody);
            return true;
        }

        public UserProfileViewModel GetUserProfile(int id)
        {
            var user = _serviceProvider.GetRequiredService<IUserDA>().GetUser(id);
            return new UserProfileViewModel()
            {
                Id = user.Id,
                DisplayName = user.Displayname,
                Contactnumber = user.Contactnumber,
                Designation = user.Designation,
                Email = user.Emailid,
                Team = user.Team,
                Function = user.Function,
                Reportingmgrname = user.Reportingmgrname,
                Reportingmgrdesignation = user.Reportingmgrdesignation,
            };
        }
        public bool UpdateUserProfile(UserProfileViewModel model)
        {
            var login = new Login()
            {
                Id = model.Id,
                Displayname = model.DisplayName,
                Contactnumber = model.Contactnumber,
                Designation = model.Designation,
                Emailid = model.Email,
                Team = model.Team,
                Function = model.Function,
                Reportingmgrname = model.Reportingmgrname,
                Reportingmgrdesignation = model.Reportingmgrdesignation,
            };
            _serviceProvider.GetRequiredService<IUserDA>().UpdateProfile(login);
            return true;
        }
    }
}
