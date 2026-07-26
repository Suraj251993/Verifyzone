using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Autoapprovalexclusion
{
    public int Id { get; set; }

    public int? Customerid { get; set; }

    public int? Employeeid { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual Customer Customer { get; set; }

    public virtual Employee Employee { get; set; }
}
