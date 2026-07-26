using System;

namespace OrgCheck.Models;

public partial class Consentauditlog
{
    public int Id { get; set; }

    public int Consentrequestid { get; set; }

    public string Action { get; set; }

    public int? Oldstatusid { get; set; }

    public int? Newstatusid { get; set; }

    public int? Performedby { get; set; }

    public string Ipaddress { get; set; }

    public string Useragent { get; set; }

    public string Remarks { get; set; }

    public DateTime Createddate { get; set; }

    public virtual Consentrequest Consentrequest { get; set; }

    public virtual Login PerformedbyNavigation { get; set; }

    public virtual LookupConsentstatus OldstatusNavigation { get; set; }

    public virtual LookupConsentstatus NewstatusNavigation { get; set; }
}
