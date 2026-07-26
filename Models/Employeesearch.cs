using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Employeesearch
{
    public int Id { get; set; }

    public string Searchrequestid { get; set; }

    public int Customerid { get; set; }

    public string Employeecode { get; set; }

    public string Name { get; set; }

    public string Reportlink { get; set; }

    public string Searchresult { get; set; }

    public double Transactionamount { get; set; }

    public int Status { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public string Finalresult { get; set; }

    public DateTime? Reportdate { get; set; }

    public string Clientname { get; set; }

    public int? Employeeid { get; set; }

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual Customer Customer { get; set; }

    public virtual ICollection<Downloadreport> Downloadreports { get; set; } = new List<Downloadreport>();

    public virtual Employee Employee { get; set; }

    public virtual ICollection<Employeeapproval> Employeeapprovals { get; set; } = new List<Employeeapproval>();

    public virtual ICollection<Reportdownload> Reportdownloads { get; set; } = new List<Reportdownload>();
}
