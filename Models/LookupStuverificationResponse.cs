using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class LookupStuverificationResponse
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Studentrequest> Studentrequests { get; set; } = new List<Studentrequest>();
}
