using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Employeeapproval
{
    public int Id { get; set; }

    public int Employeeid { get; set; }

    public int? Requestedby { get; set; }

    public DateTime? Requesteddate { get; set; }

    public int? Approvedby { get; set; }

    public DateTime? Approveddate { get; set; }

    public int? Employeesearchid { get; set; }

    public bool Isedit { get; set; }

    public virtual Login ApprovedbyNavigation { get; set; }

    public virtual Employee Employee { get; set; }

    public virtual Employeesearch Employeesearch { get; set; }

    public virtual Login RequestedbyNavigation { get; set; }
}
