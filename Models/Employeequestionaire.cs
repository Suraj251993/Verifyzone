using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Employeequestionaire
{
    public int Id { get; set; }

    public int Employeeid { get; set; }

    public int Questionid { get; set; }

    public string Answer { get; set; }

    public int Status { get; set; }

    public virtual Employee Employee { get; set; }

    public virtual Questionaire Question { get; set; }
}
