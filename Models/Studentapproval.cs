using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Studentapproval
{
    public int Id { get; set; }

    public int Studentid { get; set; }

    public int? Requestedby { get; set; }

    public DateTime? Requesteddate { get; set; }

    public int? Approvedby { get; set; }

    public DateTime? Approveddate { get; set; }

    public int? Studentsearchid { get; set; }

    public bool Isedit { get; set; }

    public virtual Login ApprovedbyNavigation { get; set; }

    public virtual Login RequestedbyNavigation { get; set; }

    public virtual Student Student { get; set; }

    public virtual Studentsearch Studentsearch { get; set; }
}
