using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Company
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    public string Contactname { get; set; }

    public string Email { get; set; }

    public string Contactnumber { get; set; }

    public int Status { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public int? Modifiedby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public double Charges { get; set; }

    public string GstNumber { get; set; }

    public string TanNumber { get; set; }

    public string PanNumber { get; set; }

    public double Educharges { get; set; }

    public virtual ICollection<Companycredit> Companycredits { get; set; } = new List<Companycredit>();

    public virtual ICollection<Companyquestion> Companyquestions { get; set; } = new List<Companyquestion>();

    public virtual ICollection<Companywallet> Companywallets { get; set; } = new List<Companywallet>();

    public virtual ICollection<Companywallettransaction> Companywallettransactions { get; set; } = new List<Companywallettransaction>();

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual Login ModifiedbyNavigation { get; set; }
}
