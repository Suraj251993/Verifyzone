using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Customer
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

    public DateTime? CommencementDate { get; set; }

    public DateTime? ClosedDate { get; set; }

    public string GstNumber { get; set; }

    public string TanNumber { get; set; }

    public string PanNumber { get; set; }

    public double Charges { get; set; }

    public string Parentname { get; set; }

    public bool? Isemployment { get; set; }

    public bool? Iseducation { get; set; }

    public int? Industrytype { get; set; }

    public bool? Isbgv { get; set; }

    public virtual ICollection<Autoapprovalexclusion> Autoapprovalexclusions { get; set; } = new List<Autoapprovalexclusion>();

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual ICollection<Customercredit> Customercredits { get; set; } = new List<Customercredit>();

    public virtual ICollection<Customeremailsetting> CustomeremailsettingCreatedcustomers { get; set; } = new List<Customeremailsetting>();

    public virtual ICollection<Customeremailsetting> CustomeremailsettingCustomers { get; set; } = new List<Customeremailsetting>();

    public virtual ICollection<Customersetting> Customersettings { get; set; } = new List<Customersetting>();

    public virtual ICollection<Customerwallet> Customerwallets { get; set; } = new List<Customerwallet>();

    public virtual ICollection<Customerwallettransaction> Customerwallettransactions { get; set; } = new List<Customerwallettransaction>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Employeesearch> Employeesearches { get; set; } = new List<Employeesearch>();

    public virtual ICollection<File> Files { get; set; } = new List<File>();

    public virtual ICollection<Invalidemployee> Invalidemployees { get; set; } = new List<Invalidemployee>();

    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();

    public virtual Login ModifiedbyNavigation { get; set; }

    public virtual ICollection<Studentrequest> Studentrequests { get; set; } = new List<Studentrequest>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Studentsearch> Studentsearches { get; set; } = new List<Studentsearch>();

    public virtual ICollection<Tempemployee> Tempemployees { get; set; } = new List<Tempemployee>();
}
