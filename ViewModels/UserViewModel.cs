using System;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class UserViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("loginname")]
        public string LoginName { get; set; }
        [JsonPropertyName("displayname")]
        public string DisplayName { get; set; }
        [JsonPropertyName("usertype")]
        public int UserType { get; set; }
        public string UserTypename { get; set; }
        [JsonPropertyName("customertypeid")]
        public int CustomerType { get; set; }
        [JsonPropertyName("customertypename")]
        public string CustomerTypename { get; set; }
        [JsonPropertyName("category")]
        public int Category { get; set; }        
        [JsonPropertyName("customerid")]
        public int CustomerId { get; set; }
        [JsonPropertyName("customername")]
        public string Customername { get; set; }
        [JsonPropertyName("emailid")]
        public string Emailid { get; set; }
        [JsonPropertyName("contactnumber")]
        public string Contactnumber { get; set; }
        [JsonPropertyName("designation")]
        public string Designation { get; set; }
        [JsonPropertyName("exempverification")]
        public bool IsEmployment { get; set; }
        [JsonPropertyName("empverification")]
        public bool IsEducation { get; set; }
        [JsonPropertyName("stuverification")]
        public bool IsBGV { get; set; }
        [JsonPropertyName("team")]
        public string Team { get; set; }
        [JsonPropertyName("function")]
        public string Function { get; set; }
        [JsonPropertyName("rptmgrname")]
        public string Reportingmgrname { get; set; }
        [JsonPropertyName("rptmgrdesignation")]
        public string Reportingmgrdesignation { get; set; }
    }

    public class UserProfileViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("displayname")]
        public string DisplayName { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("contactnumber")]
        public string Contactnumber { get; set; }
        [JsonPropertyName("designation")]
        public string Designation { get; set; }
        [JsonPropertyName("team")]
        public string Team { get; set; }
        [JsonPropertyName("function")]
        public string Function { get; set; }
        [JsonPropertyName("rptmgrname")]
        public string Reportingmgrname { get; set; }
        [JsonPropertyName("rptmgrdesignation")]
        public string Reportingmgrdesignation { get; set; }
    }
}
