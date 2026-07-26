using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Consentrequest
{
    public int Id { get; set; }

    public string Consentrequestid { get; set; }

    public int Customerid { get; set; }

    public string Employeefirstname { get; set; }

    public string Employeelastname { get; set; }

    public string Employeecode { get; set; }

    public string Employeeemail { get; set; }

    public string Optionalemail { get; set; }

    public int Statusid { get; set; }

    public string Token { get; set; }

    public bool Tokenconsumed { get; set; }

    public DateTime Tokenexpirydate { get; set; }

    public DateTime? Consentdate { get; set; }

    public string Ipaddress { get; set; }

    public string Device { get; set; }

    public string Browser { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public int? Modifiedby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public virtual Customer Customer { get; set; }

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual Login ModifiedbyNavigation { get; set; }

    public virtual LookupConsentstatus Status { get; set; }

    public virtual ICollection<Consentauditlog> Consentauditlogs { get; set; } = new List<Consentauditlog>();
}
