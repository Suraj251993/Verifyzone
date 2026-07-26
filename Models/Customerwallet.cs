using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Customerwallet
{
    public int Id { get; set; }

    public int Customerid { get; set; }

    public int Totalcredit { get; set; }

    public int Status { get; set; }

    public double? Creditpending { get; set; }

    public virtual Customer Customer { get; set; }
}
