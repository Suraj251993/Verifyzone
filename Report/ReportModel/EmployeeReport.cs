using OrgCheck.ViewModels;
using System;
using System.Collections.Generic;

namespace OrgCheck.Report.ReportModel
{
    public class EmployeeReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Employeecode { get; set; }
        public string Designation { get; set; }
        public string Fromdate { get; set; }
        public string Todate { get; set; }
        public string Reasonforleaving { get; set; }
        public string Location { get; set; }
        public string Jobtype { get; set; }
        public string Lastdrawnsalary { get; set; }
        public string Reportingto { get; set; }
        public string Managerdesignation { get; set; }
        public string Comments { get; set; }
        public string Question1 { get; set; }
        public string Answer1 { get; set; }        
    }
}
