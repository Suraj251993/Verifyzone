using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class EmployeeViewModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("customerid")]
        public int Customerid { get; set; }

        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Name Required")]
        public string Name { get; set; }

        [JsonPropertyName("employeecode")]
        [Required(ErrorMessage = "Employee Code Required")]
        public string Employeecode { get; set; }

        [Required(ErrorMessage = "Designation Required")]
        [JsonPropertyName("designation")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "From Date Required")]
        [JsonPropertyName("fromdate")]
        public string Fromdate { get; set; }

        [Required(ErrorMessage = "Todate Required")]
        [JsonPropertyName("todate")]
        public string Todate { get; set; }

        [JsonPropertyName("reasonforleaving")]
        [Required(ErrorMessage = "Reason for leaving Required")]
        public string Reasonforleaving { get; set; }

        [JsonPropertyName("exittype")]
        public string ExitType { get; set; }

        [Required(ErrorMessage = "Location Required")]
        [JsonPropertyName("location")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Jobtype Required")]
        [JsonPropertyName("jobtype")]
        public string Jobtype { get; set; }

        [JsonPropertyName("jobtypename")]
        public string JobtypeName { get; set; }

        [JsonPropertyName("lastdrawnsalary")]
        [Required(ErrorMessage = "Last drawn salary Required")]
        public string Lastdrawnsalary { get; set; }

        [Required(ErrorMessage = "Reporting To Required")]
        [JsonPropertyName("reportingto")]
        public string Reportingto { get; set; }

        [Required(ErrorMessage = "Manager Desigation Required")]
        [JsonPropertyName("managerdesignation")]
        public string Managerdesignation { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }
        [JsonPropertyName("authorizedby")]
        public string AuthorizedBy { get; set; }
        [JsonPropertyName("authorizeddate")]
        public string AuthorizedDate { get; set; }

        [JsonPropertyName("emailid")]
        public string Emailid { get; set; }
        [JsonPropertyName("mobileno")]
        public string Mobileno { get; set; }
        [JsonPropertyName("uannumber")]
        public string Uannumber { get; set; }
        [JsonPropertyName("fathername")]
        public string Fathername { get; set; }
        [JsonPropertyName("resume")]
        public IFormFile Resume { get; set; }
        [JsonPropertyName("resumename")]
        public string Resumename { get; set; }
        [JsonPropertyName("discrepancytype")]
        public int DiscrepancyType { get; set; }
        [JsonPropertyName("linkedinurl")]
        public string Linkedinurl { get; set; }
        [JsonPropertyName("highlight")]
        public string Highlight { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("searchid")]
        public int SearchId { get; set; }
        [JsonPropertyName("isedit")]
        public bool IsEdit { get; set; }

        [Required(ErrorMessage = "Employee Questions Required")]
        [JsonPropertyName("employeeQuestions")]
        public List<EmployeeQuestionaireViewModel> EmployeeQuestions { get; set; }  

    }
    public class FileApprove
    {
        public string Mode { get; set; }

        public int FileId { get; set; }
    }
}
