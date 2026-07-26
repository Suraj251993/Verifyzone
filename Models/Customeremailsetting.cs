using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Customeremailsetting
{
    public int Id { get; set; }

    public int? Customerid { get; set; }

    public int Templateid { get; set; }

    public string Templatecontent { get; set; }

    public int Createdby { get; set; }

    public int Createdcustomerid { get; set; }

    public DateTime Createddate { get; set; }

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual Customer Createdcustomer { get; set; }

    public virtual Customer Customer { get; set; }
}
