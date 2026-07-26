using System;
using System.Collections.Generic;

namespace OrgCheck.Models;

public partial class Tempstudent
{
    public int Id { get; set; }

    public string Institutionname { get; set; }

    public string Studentid { get; set; }

    public string Studentname { get; set; }

    public string Degreetype { get; set; }

    public string Majorsubject { get; set; }

    public string Periodfrom { get; set; }

    public string Periodto { get; set; }

    public string Passyear { get; set; }

    public string Marksobtained { get; set; }

    public string Comments { get; set; }

    public int Fileid { get; set; }

    public int Createdby { get; set; }

    public DateTime Createddate { get; set; }

    public string University { get; set; }

    public int? Customerid { get; set; }

    public string Studymode { get; set; }

    public string EligibleAttainDegree { get; set; }

    public virtual Login CreatedbyNavigation { get; set; }

    public virtual File File { get; set; }
}
