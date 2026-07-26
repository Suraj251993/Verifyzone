using System;
using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class EmployeeQuestionaireViewModel
    {
        [JsonPropertyName("questionId")]
        public string QuestionId { get; set; }
        [JsonPropertyName("questionname")]
        public string Questionname { get; set; }
        [JsonPropertyName("answer")]
        public string Answer { get; set; }
    }
}
