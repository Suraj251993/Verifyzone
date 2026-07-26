using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class CustomerEmailSettingViewModel
    {
        [JsonPropertyName("customerid")]
        public string Customerid { get; set; }
        [JsonPropertyName("customername")]
        public string Customername { get; set; }
        [JsonPropertyName("templateid")]
        public string Templateid { get; set; }
        [JsonPropertyName("templatecontent")]
        public string Templatecontent { get; set; }
    }
}
