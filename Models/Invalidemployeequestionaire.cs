using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Invalidemployeequestionaire
{
    public int Id { get; set; }

    public int Invalidemployeeid { get; set; }

    public int Questionid { get; set; }

    public string Answer { get; set; }

    public int Status { get; set; }

    public virtual Invalidemployee Invalidemployee { get; set; }

    public virtual Questionaire Question { get; set; }
}
