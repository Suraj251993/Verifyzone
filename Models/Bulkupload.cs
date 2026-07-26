using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Bulkupload
{
    public int Id { get; set; }

    public string Filename { get; set; }

    public int Uploadedby { get; set; }

    public DateTime Uploadeddate { get; set; }
}
