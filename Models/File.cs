using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class File
{
    public int Id { get; set; }

    public string Filename { get; set; }

    public int Filesize { get; set; }

    public int Uploadedby { get; set; }

    public DateTime? Uploadeddate { get; set; }

    public int Customerid { get; set; }

    public int Uploadedstatus { get; set; }

    public int Totalrecords { get; set; }

    public int Validrecords { get; set; }

    public int Invalidrecords { get; set; }

    public int Status { get; set; }

    public virtual Customer Customer { get; set; }

    public virtual ICollection<Tempemployee> Tempemployees { get; set; } = new List<Tempemployee>();

    public virtual ICollection<Tempstudent> Tempstudents { get; set; } = new List<Tempstudent>();

    public virtual Login UploadedbyNavigation { get; set; }
}
