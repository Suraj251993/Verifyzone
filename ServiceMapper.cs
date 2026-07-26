using Microsoft.Extensions.DependencyInjection;
using OrgCheck.DataAccess;
using OrgCheck.DataAccess.Interfaces;
using OrgCheck.Services;
using OrgCheck.Services.Interfaces;
using System;

namespace OrgCheck
{
    public static class ServiceMapper
    {
        public static void AddServices(this IServiceCollection _services)
        {
            _services.AddScoped<IAuthService, AuthService>();
            _services.AddScoped<IUserDA, UserDA>();
            _services.AddScoped<IUserService, UserService>();
            _services.AddScoped<ICustomerDA, CustomerDA>();
            _services.AddScoped<ICustomerService, CustomerService>();
            _services.AddScoped<ICompanyDA, CompanyDA>();
            _services.AddScoped<ICompanyService, CompanyService>();
            _services.AddScoped<IQuestionaireDA, QuestionaireDA>();
            _services.AddScoped<IQuestionaireService, QuestionaireService>();
            _services.AddScoped<IEmployeeDA, EmployeeDA>();
            _services.AddScoped<IEmployeeService, EmployeeService>();
            _services.AddTransient<EmailService, EmailService>();
            _services.AddTransient<CryptoService, CryptoService>();
            _services.AddScoped<IFileDA, FileDA>();
            _services.AddScoped<IFileService, FileService>();
            _services.AddTransient<LogService, LogService>();
            _services.AddScoped<IStudentDA, StudentDA>();
            _services.AddScoped<IStudentService, StudentService>();
            _services.AddScoped<IConsentDA, ConsentDA>();
            _services.AddScoped<IConsentService, ConsentService>();
        }
    }
}
