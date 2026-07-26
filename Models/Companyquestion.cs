using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Companyquestion
{
    public int Id { get; set; }

    public int Companyid { get; set; }

    public int Questionid { get; set; }

    public virtual Company Company { get; set; }

    public virtual Questionaire Question { get; set; }
}
