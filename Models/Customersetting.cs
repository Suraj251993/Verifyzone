using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Customersetting
{
    public int Id { get; set; }

    public int Customerid { get; set; }

    public int? Pendingcreditthreshold { get; set; }

    public virtual Customer Customer { get; set; }
}
