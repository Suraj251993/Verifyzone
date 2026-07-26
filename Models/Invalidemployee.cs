using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Invalidemployee
{
    public int Id { get; set; }

    public int? Customerid { get; set; }

    public string Name { get; set; }

    public string Employeecode { get; set; }

    public string Designation { get; set; }

    public DateTime? Fromdate { get; set; }

    public DateTime? Todate { get; set; }

    public string Reasonforleaving { get; set; }

    public string Location { get; set; }

    public string Jobtype { get; set; }

    public string Lastdrawnsalary { get; set; }

    public string Reportingto { get; set; }

    public string Managerdesignation { get; set; }

    public string Comments { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public string Exittype { get; set; }

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual Customer Customer { get; set; }

    public virtual ICollection<Empverificationrequest> Empverificationrequests { get; set; } = new List<Empverificationrequest>();

    public virtual ICollection<Invalidemployeequestionaire> Invalidemployeequestionaires { get; set; } = new List<Invalidemployeequestionaire>();
}
