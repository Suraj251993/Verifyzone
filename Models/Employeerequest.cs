using System;
using System.Collections.Generic;

#nullable disable

namespace OrgCheck.Models
{
    public partial class Employeerequest
    {
        public int Id { get; set; }
        public int Searchid { get; set; }
        public string Empcode { get; set; }
        public int Customerid { get; set; }
        public string Requestcomments { get; set; }
        public int Raisedby { get; set; }
        public DateTime Raiseddate { get; set; }
        public string Hrcomments { get; set; }
        public int? Repliedby { get; set; }
        public DateTime? Replieddate { get; set; }
        public int Status { get; set; }
        public int? Responsetype { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Login RaisedbyNavigation { get; set; }
        public virtual Login RepliedbyNavigation { get; set; }
        public virtual LookupEmpverificationResponse ResponsetypeNavigation { get; set; }
        public virtual Employeesearch Search { get; set; }
    }
}
