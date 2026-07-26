using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Autoapprovalconfig
{
    public int Id { get; set; }

    public DateTime Startdate { get; set; }

    public DateTime Enddate { get; set; }

    public int Status { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public int? Updatedby { get; set; }

    public DateTime? Updateddate { get; set; }

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual Login UpdatedbyNavigation { get; set; }
}
