using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Student
{
    public int Id { get; set; }

    public int Customerid { get; set; }

    public string University { get; set; }

    public string Studentid { get; set; }

    public string Studentname { get; set; }

    public string Degreetype { get; set; }

    public string Majorsubject { get; set; }

    public string Periodfrom { get; set; }

    public string Periodto { get; set; }

    public string Passyear { get; set; }

    public string Marksobtained { get; set; }

    public string Comments { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public bool Isapproved { get; set; }

    public string Studymode { get; set; }

    public string EligibleAttainDegree { get; set; }

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual Customer Customer { get; set; }

    public virtual ICollection<Studentapproval> Studentapprovals { get; set; } = new List<Studentapproval>();

    public virtual ICollection<Studentsearch> Studentsearches { get; set; } = new List<Studentsearch>();
}
