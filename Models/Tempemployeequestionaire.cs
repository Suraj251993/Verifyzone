using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Tempemployeequestionaire
{
    public int Id { get; set; }

    public int Tempemployeeid { get; set; }

    public int Questionid { get; set; }

    public string Answer { get; set; }

    public int Status { get; set; }

    public virtual Questionaire Question { get; set; }

    public virtual Tempemployee Tempemployee { get; set; }
}
