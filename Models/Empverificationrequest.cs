using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Empverificationrequest
{
    public int Id { get; set; }

    public int? Tempemployeeid { get; set; }

    public int? Employeeid { get; set; }

    public string Requestnumber { get; set; }

    public string Requeststatus { get; set; }

    public int Active { get; set; }

    public int Createdby { get; set; }

    public DateTime? Createddate { get; set; }

    public string Reportname { get; set; }

    public int? Invalidemployeeid { get; set; }

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual Employee Employee { get; set; }

    public virtual Invalidemployee Invalidemployee { get; set; }

    public virtual Tempemployee Tempemployee { get; set; }
}
