using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class LookupDiscrepancytype
{
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Absconddetail> Absconddetails { get; set; } = new List<Absconddetail>();
}
