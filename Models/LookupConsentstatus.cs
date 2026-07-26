using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class LookupConsentstatus
{
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Consentrequest> Consentrequests { get; set; } = new List<Consentrequest>();
}
