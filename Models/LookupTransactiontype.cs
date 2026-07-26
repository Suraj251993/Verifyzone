using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class LookupTransactiontype
{
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Companywallettransaction> Companywallettransactions { get; set; } = new List<Companywallettransaction>();

    public virtual ICollection<Customerwallettransaction> Customerwallettransactions { get; set; } = new List<Customerwallettransaction>();
}
