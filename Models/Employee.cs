using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Employee
{
    public int Id { get; set; }

    public int Customerid { get; set; }

    public string Name { get; set; }

    public string Employeecode { get; set; }

    public string Designation { get; set; }

    public DateTime Fromdate { get; set; }

    public DateTime Todate { get; set; }

    public string Reasonforleaving { get; set; }

    public string Location { get; set; }

    public string Jobtype { get; set; }

    public string Lastdrawnsalary { get; set; }

    public string Reportingto { get; set; }

    public string Managerdesignation { get; set; }

    public string Comments { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public bool Isapproved { get; set; }

    public bool Isedit { get; set; }

    public string Exittype { get; set; }

    public virtual ICollection<Absconddetail> Absconddetails { get; set; } = new List<Absconddetail>();

    public virtual ICollection<Autoapprovalexclusion> Autoapprovalexclusions { get; set; } = new List<Autoapprovalexclusion>();

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual Customer Customer { get; set; }

    public virtual ICollection<Employeeapproval> Employeeapprovals { get; set; } = new List<Employeeapproval>();

    public virtual ICollection<Employeequestionaire> Employeequestionaires { get; set; } = new List<Employeequestionaire>();

    public virtual ICollection<Employeesearch> Employeesearches { get; set; } = new List<Employeesearch>();

    public virtual ICollection<Empverificationrequest> Empverificationrequests { get; set; } = new List<Empverificationrequest>();
}
