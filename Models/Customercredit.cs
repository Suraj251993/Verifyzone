using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Customercredit
{
    public int Id { get; set; }

    public int Customerid { get; set; }

    public int Credit { get; set; }

    public string Transactiontype { get; set; }

    public string Referenceno { get; set; }

    public string Remarks { get; set; }

    public int Status { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual Customer Customer { get; set; }
}
