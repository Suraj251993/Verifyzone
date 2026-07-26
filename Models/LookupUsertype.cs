using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class LookupUsertype
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();
}
