using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Companywallet
{
    public int Id { get; set; }

    public int Companyid { get; set; }

    public int Totalcredit { get; set; }

    public int Status { get; set; }

    public virtual Company Company { get; set; }
}
