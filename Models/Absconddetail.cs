using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Absconddetail
{
    public int Id { get; set; }

    public int? Status { get; set; }

    public int Createdby { get; set; }

    public DateTime? Createddate { get; set; }

    public string Mobileno { get; set; }

    public string Linkedinurl { get; set; }

    public string Uannumber { get; set; }

    public string Fathername { get; set; }

    public string Resume { get; set; }

    public string Emailid { get; set; }

    public bool? Isprocessed { get; set; }

    public int? Discrepancetype { get; set; }

    public int? Employeeid { get; set; }

    public virtual ICollection<Absconddocumentdatum> Absconddocumentdata { get; set; } = new List<Absconddocumentdatum>();

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual LookupDiscrepancytype DiscrepancetypeNavigation { get; set; }

    public virtual Employee Employee { get; set; }
}
