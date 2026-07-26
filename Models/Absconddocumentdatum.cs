using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Absconddocumentdatum
{
    public int Id { get; set; }

    public int Abscondid { get; set; }

    public string Extracttext { get; set; }

    public virtual Absconddetail Abscond { get; set; }
}
