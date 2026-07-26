using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Login
{
    public int Id { get; set; }

    public string Loginname { get; set; }

    public string Password { get; set; }

    public int Usertypeid { get; set; }

    public int? Customerid { get; set; }

    public string Emailid { get; set; }

    public string Contactnumber { get; set; }

    public int Status { get; set; }

    public string Displayname { get; set; }

    public string Designation { get; set; }

    public int? Customertypeid { get; set; }

    public string Team { get; set; }

    public string Function { get; set; }

    public string Reportingmgrname { get; set; }

    public string Reportingmgrdesignation { get; set; }

    public int? Category { get; set; }

    public virtual ICollection<Absconddetail> Absconddetails { get; set; } = new List<Absconddetail>();

    public virtual ICollection<Autoapprovalconfig> AutoapprovalconfigCreatedbyNavigations { get; set; } = new List<Autoapprovalconfig>();

    public virtual ICollection<Autoapprovalconfig> AutoapprovalconfigUpdatedbyNavigations { get; set; } = new List<Autoapprovalconfig>();

    public virtual ICollection<Autoapprovalexclusion> Autoapprovalexclusions { get; set; } = new List<Autoapprovalexclusion>();

    public virtual ICollection<Company> CompanyCreatedbyNavigations { get; set; } = new List<Company>();

    public virtual ICollection<Company> CompanyModifiedbyNavigations { get; set; } = new List<Company>();

    public virtual ICollection<Companycredit> Companycredits { get; set; } = new List<Companycredit>();

    public virtual ICollection<Companywallettransaction> Companywallettransactions { get; set; } = new List<Companywallettransaction>();

    public virtual Customer Customer { get; set; }

    public virtual ICollection<Customer> CustomerCreatedbyNavigations { get; set; } = new List<Customer>();

    public virtual ICollection<Customer> CustomerModifiedbyNavigations { get; set; } = new List<Customer>();

    public virtual ICollection<Customercredit> Customercredits { get; set; } = new List<Customercredit>();

    public virtual ICollection<Customeremailsetting> Customeremailsettings { get; set; } = new List<Customeremailsetting>();

    public virtual LookupCustomertype Customertype { get; set; }

    public virtual ICollection<Customerwallettransaction> Customerwallettransactions { get; set; } = new List<Customerwallettransaction>();

    public virtual ICollection<Downloadreport> Downloadreports { get; set; } = new List<Downloadreport>();

    public virtual ICollection<Employeeapproval> EmployeeapprovalApprovedbyNavigations { get; set; } = new List<Employeeapproval>();

    public virtual ICollection<Employeeapproval> EmployeeapprovalRequestedbyNavigations { get; set; } = new List<Employeeapproval>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Employeesearch> Employeesearches { get; set; } = new List<Employeesearch>();

    public virtual ICollection<Empverificationrequest> Empverificationrequests { get; set; } = new List<Empverificationrequest>();

    public virtual ICollection<File> Files { get; set; } = new List<File>();

    public virtual ICollection<Invalidemployee> Invalidemployees { get; set; } = new List<Invalidemployee>();

    public virtual ICollection<Studentapproval> StudentapprovalApprovedbyNavigations { get; set; } = new List<Studentapproval>();

    public virtual ICollection<Studentapproval> StudentapprovalRequestedbyNavigations { get; set; } = new List<Studentapproval>();

    public virtual ICollection<Studentrequest> StudentrequestRaisedbyNavigations { get; set; } = new List<Studentrequest>();

    public virtual ICollection<Studentrequest> StudentrequestRepliedbyNavigations { get; set; } = new List<Studentrequest>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Studentsearch> Studentsearches { get; set; } = new List<Studentsearch>();

    public virtual ICollection<Tempemployee> Tempemployees { get; set; } = new List<Tempemployee>();

    public virtual ICollection<Tempstudent> Tempstudents { get; set; } = new List<Tempstudent>();

    public virtual LookupUsertype Usertype { get; set; }
}
