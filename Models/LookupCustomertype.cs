using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class LookupCustomertype
{
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();
}
