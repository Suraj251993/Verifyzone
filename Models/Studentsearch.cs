using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Studentsearch
{
    public int Id { get; set; }

    public string Searchrequestid { get; set; }

    public int Customerid { get; set; }

    public string Studentid { get; set; }

    public string Reportlink { get; set; }

    public string Searchresult { get; set; }

    public double Transactionamount { get; set; }

    public int Status { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public string Finalresult { get; set; }

    public DateTime? Reportdate { get; set; }

    public int? Studentkey { get; set; }

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual Customer Customer { get; set; }

    public virtual ICollection<Studentapproval> Studentapprovals { get; set; } = new List<Studentapproval>();

    public virtual Student StudentkeyNavigation { get; set; }

    public virtual ICollection<Studentrequest> Studentrequests { get; set; } = new List<Studentrequest>();
}
