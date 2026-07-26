using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Questionaire
{
    public int Id { get; set; }

    public string Question { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Companyquestion> Companyquestions { get; set; } = new List<Companyquestion>();

    public virtual ICollection<Employeequestionaire> Employeequestionaires { get; set; } = new List<Employeequestionaire>();

    public virtual ICollection<Invalidemployeequestionaire> Invalidemployeequestionaires { get; set; } = new List<Invalidemployeequestionaire>();

    public virtual ICollection<Tempemployeequestionaire> Tempemployeequestionaires { get; set; } = new List<Tempemployeequestionaire>();
}
