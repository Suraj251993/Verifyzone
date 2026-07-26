using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Reportdownload
{
    public int Id { get; set; }

    public int Employeesearchid { get; set; }

    public int Downloadby { get; set; }

    public DateTime Downloaddate { get; set; }

    public virtual Employeesearch Employeesearch { get; set; }
}
