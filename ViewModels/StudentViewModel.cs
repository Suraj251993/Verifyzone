using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class StudentViewModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("customerid")]
        public int Customerid { get; set; }
        [JsonPropertyName("university")]
        public string University { get; set; }
        [JsonPropertyName("studentid")]
        public string Studentid { get; set; }
        [JsonPropertyName("studentname")]
        public string Studentname { get; set; }
        [JsonPropertyName("degreetype")]
        public string Degreetype { get; set; }
        [JsonPropertyName("majorsubject")]
        public string Majorsubject { get; set; }
        [JsonPropertyName("periodfrom")]
        public string Periodfrom { get; set; }
        [JsonPropertyName("periodto")]
        public string Periodto { get; set; }
        [JsonPropertyName("passyear")]
        public string Passyear { get; set; }
        [JsonPropertyName("marksobtained")]
        public string Marksobtained { get; set; }
        [JsonPropertyName("comments")]
        public string Comments { get; set; }
        [JsonPropertyName("educationperiod")]
        public string EducationPeriod { get; set; }
        [JsonPropertyName("studymode")]
        public string StudyMode { get; set; }
        [JsonPropertyName("eligibleattaindegree")]
        public string EligibleAttainDegree { get; set; }

        [JsonPropertyName("authorizedby")]
        public string AuthorizedBy { get; set; }
        [JsonPropertyName("authorizeddate")]
        public string AuthorizedDate { get; set; }

        [JsonPropertyName("searchid")]
        public int SearchId { get; set; }
    }
}
