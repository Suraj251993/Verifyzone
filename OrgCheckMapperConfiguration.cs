using AutoMapper;
using OrgCheck.Models;
using OrgCheck.ViewModels;
using System;
using System.Linq;

namespace OrgCheck
{
    public class OrgCheckMapperConfiguration : Profile
    {
        public OrgCheckMapperConfiguration()
        {
            CreateMap<CustomerViewModel, Customer>()
                .ForMember(s => s.Email, d => d.MapFrom(m => string.IsNullOrEmpty(m.Email) ? string.Empty : m.Email))
                .ForMember(s => s.Name, d => d.MapFrom(m => string.IsNullOrEmpty(m.Name) ? string.Empty : m.Name))
                .ForMember(s => s.Address, d => d.MapFrom(m => string.IsNullOrEmpty(m.Address) ? string.Empty : m.Address))
                .ForMember(s => s.Contactname, d => d.MapFrom(m => string.IsNullOrEmpty(m.Contactname) ? string.Empty : m.Contactname))
                .ForMember(s => s.Contactnumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.Contactnumber) ? string.Empty : m.Contactnumber))
                .ForMember(s => s.GstNumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.GstNumber) ? string.Empty : m.GstNumber))
                .ForMember(s => s.TanNumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.TanNumber) ? string.Empty : m.TanNumber))
                .ForMember(s => s.PanNumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.PanNumber) ? string.Empty : m.PanNumber))
                .ForMember(s => s.CommencementDate, d => d.Ignore())
                .ForMember(s => s.ClosedDate, d => d.Ignore())
                .ForMember(s => s.Iseducation, d => d.Ignore())
                .ForMember(s => s.Isemployment, d => d.Ignore()); ;
            CreateMap<CompanyViewModel, Company>()
                .ForMember(s => s.Email, d => d.MapFrom(m => string.IsNullOrEmpty(m.Email) ? string.Empty : m.Email))
                .ForMember(s => s.Name, d => d.MapFrom(m => string.IsNullOrEmpty(m.Name) ? string.Empty : m.Name))
                .ForMember(s => s.Address, d => d.MapFrom(m => string.IsNullOrEmpty(m.Address) ? string.Empty : m.Address))
                .ForMember(s => s.Contactname, d => d.MapFrom(m => string.IsNullOrEmpty(m.Contactname) ? string.Empty : m.Contactname))
                .ForMember(s => s.Contactnumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.Contactnumber) ? string.Empty : m.Contactnumber))
                .ForMember(s => s.GstNumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.GstNumber) ? string.Empty : m.GstNumber))
                .ForMember(s => s.TanNumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.TanNumber) ? string.Empty : m.TanNumber))
                .ForMember(s => s.PanNumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.PanNumber) ? string.Empty : m.PanNumber));

            CreateMap<Customer, CustomerViewModel>()
                .ForMember(s => s.Email, d => d.MapFrom(m => string.IsNullOrEmpty(m.Email) ? string.Empty : m.Email))
                .ForMember(s => s.Name, d => d.MapFrom(m => string.IsNullOrEmpty(m.Name) ? string.Empty : m.Name))
                .ForMember(s => s.Address, d => d.MapFrom(m => string.IsNullOrEmpty(m.Address) ? string.Empty : m.Address))
                .ForMember(s => s.Contactname, d => d.MapFrom(m => string.IsNullOrEmpty(m.Contactname) ? string.Empty : m.Contactname))
                .ForMember(s => s.Contactnumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.Contactnumber) ? string.Empty : m.Contactnumber))
                .ForMember(s => s.GstNumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.GstNumber) ? string.Empty : m.GstNumber))
                .ForMember(s => s.TanNumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.TanNumber) ? string.Empty : m.TanNumber))
                .ForMember(s => s.PanNumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.PanNumber) ? string.Empty : m.PanNumber))
                .ForMember(s => s.CommencementDate, d => d.Ignore())
                .ForMember(s => s.Closeddate, d => d.Ignore())
                .ForMember(s => s.IsEducation, d => d.Ignore())
                .ForMember(s => s.IsEmployment, d => d.Ignore()); ;
            CreateMap<Company, CompanyViewModel>()
                .ForMember(s => s.Email, d => d.MapFrom(m => string.IsNullOrEmpty(m.Email) ? string.Empty : m.Email))
                .ForMember(s => s.Name, d => d.MapFrom(m => string.IsNullOrEmpty(m.Name) ? string.Empty : m.Name))
                .ForMember(s => s.Address, d => d.MapFrom(m => string.IsNullOrEmpty(m.Address) ? string.Empty : m.Address))
                .ForMember(s => s.Contactname, d => d.MapFrom(m => string.IsNullOrEmpty(m.Contactname) ? string.Empty : m.Contactname))
                .ForMember(s => s.Contactnumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.Contactnumber) ? string.Empty : m.Contactnumber))
                .ForMember(s => s.GstNumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.GstNumber) ? string.Empty : m.GstNumber))
                .ForMember(s => s.TanNumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.TanNumber) ? string.Empty : m.TanNumber))
                .ForMember(s => s.PanNumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.PanNumber) ? string.Empty : m.PanNumber));
            CreateMap<Employee, EmployeeViewModel>()
                .ForMember(s => s.Comments, d => d.MapFrom(m => string.IsNullOrEmpty(m.Comments) ? string.Empty : m.Comments))
                .ForMember(s => s.EmployeeQuestions, d => d.Ignore())
                .ForMember(s => s.Fromdate, d => d.Ignore())
                .ForMember(s => s.Todate, d => d.Ignore());
            CreateMap<EmployeeViewModel, Employee>()
               .ForMember(s => s.Comments, d => d.MapFrom(m => string.IsNullOrEmpty(m.Comments) ? string.Empty : m.Comments))
               .ForMember(s => s.Fromdate, d => d.Ignore())
               .ForMember(s => s.Todate, d => d.Ignore());
            CreateMap<EmployeeSearchViewModel, Employeesearch>()
               .ForMember(s => s.Searchrequestid, d => d.MapFrom(m => string.IsNullOrEmpty(m.Searchrequestid) ? string.Empty : m.Searchrequestid))
                .ForMember(s => s.Employeecode, d => d.MapFrom(m => string.IsNullOrEmpty(m.Employeecode) ? string.Empty : m.Employeecode))
                .ForMember(s => s.Name, d => d.MapFrom(m => string.IsNullOrEmpty(m.Name) ? string.Empty : m.Name))
                .ForMember(s => s.Reportlink, d => d.MapFrom(m => string.IsNullOrEmpty(m.Reportlink) ? string.Empty : m.Reportlink))
                .ForMember(s => s.Finalresult, d => d.MapFrom(m => string.IsNullOrEmpty(m.Finalresult) ? string.Empty : m.Finalresult))
                .ForMember(s => s.Searchresult, d => d.MapFrom(m => string.IsNullOrEmpty(m.Searchresult) ? string.Empty : m.Searchresult))
                .ForMember(s => s.Reportdownloads, d => d.Ignore())
                .ForMember(s => s.Customer, d => d.Ignore())
                .ForMember(s => s.Downloadreports, d => d.Ignore()).ReverseMap();
            CreateMap<CompanyCreditViewModel, Companycredit>()
               .ForMember(s => s.Remarks, d => d.MapFrom(m => string.IsNullOrEmpty(m.Remarks) ? string.Empty : m.Remarks));
            CreateMap<CustomerCreditViewModel, Customercredit>()
               .ForMember(s => s.Remarks, d => d.MapFrom(m => string.IsNullOrEmpty(m.Remarks) ? string.Empty : m.Remarks));
            CreateMap<StudentViewModel, Student>()
               .ForMember(s => s.Comments, d => d.MapFrom(m => string.IsNullOrEmpty(m.Comments) ? string.Empty : m.Comments));
            CreateMap<Student, StudentViewModel>()
               .ForMember(s => s.Comments, d => d.MapFrom(m => string.IsNullOrEmpty(m.Comments) ? string.Empty : m.Comments));
            CreateMap<AbscondDetailViewModel, Absconddetail>()
                .ForMember(s => s.Mobileno, d => d.MapFrom(m => string.IsNullOrEmpty(m.Mobileno) ? string.Empty : m.Mobileno))
                .ForMember(s => s.Linkedinurl, d => d.MapFrom(m => string.IsNullOrEmpty(m.Linkedinurl) ? string.Empty : m.Linkedinurl))
                .ForMember(s => s.Uannumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.Uannumber) ? string.Empty : m.Uannumber))
                .ForMember(s => s.Fathername, d => d.MapFrom(m => string.IsNullOrEmpty(m.Fathername) ? string.Empty : m.Fathername))
                .ForMember(s => s.Emailid, d => d.MapFrom(m => string.IsNullOrEmpty(m.Emailid) ? string.Empty : m.Emailid))
                .ForMember(s => s.Resume, d => d.Ignore());
            CreateMap<Absconddetail, AbscondDetailViewModel>()
                .ForMember(s => s.Name, d => d.Ignore())
                .ForMember(s => s.Joindate, d => d.Ignore())
                .ForMember(s => s.Lastworkingdate, d => d.Ignore())
                .ForMember(s => s.Mobileno, d => d.MapFrom(m => string.IsNullOrEmpty(m.Mobileno) ? string.Empty : m.Mobileno))
                .ForMember(s => s.Linkedinurl, d => d.MapFrom(m => string.IsNullOrEmpty(m.Linkedinurl) ? string.Empty : m.Linkedinurl))
                .ForMember(s => s.Uannumber, d => d.MapFrom(m => string.IsNullOrEmpty(m.Uannumber) ? string.Empty : m.Uannumber))
                .ForMember(s => s.Fathername, d => d.MapFrom(m => string.IsNullOrEmpty(m.Fathername) ? string.Empty : m.Fathername))
                .ForMember(s => s.Emailid, d => d.MapFrom(m => string.IsNullOrEmpty(m.Emailid) ? string.Empty : m.Emailid))
                .ForMember(s => s.Resume, d => d.Ignore());
        }
        
    }
}
