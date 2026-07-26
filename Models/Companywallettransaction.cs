using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Companywallettransaction
{
    public int Id { get; set; }

    public int Companyid { get; set; }

    public int Transactiontype { get; set; }

    public double? Credits { get; set; }

    public int? Status { get; set; }

    public DateTime Createddate { get; set; }

    public string Remarks { get; set; }

    public int Createdby { get; set; }

    public virtual Company Company { get; set; }

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual LookupTransactiontype TransactiontypeNavigation { get; set; }
}
