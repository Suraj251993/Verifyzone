using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class AdminReportViewModel
    {
        public List<SelectListItem> data { get; set; }
        public List<ReportCountViewModel> rcviewmodels { get; set; }
        public List<CompanyCountViewModel> cmpviewmodels { get; set; }
        public List<ReportCountViewModel> yearwisecount { get; set; }
    }

    public class ReportCountViewModel
    {
        [JsonPropertyName("customername")]
        public string CustomerName { get; set; }
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
    public class CompanyCountViewModel
    {
        [JsonPropertyName("companyname")]
        public string CompanyName { get; set; }
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
