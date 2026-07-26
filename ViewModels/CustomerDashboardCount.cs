using System.Text.Json.Serialization;

namespace OrgCheck.ViewModels
{
    public class CustomerDashboardCount
    {
        [JsonPropertyName("completedcount")]
        public int CompletedCount { get; set; } = 0;
        [JsonPropertyName("approvalcount")]
        public int ApprovalCount { get; set; } = 0;
        [JsonPropertyName("requestcount")]
        public int RequestCount { get; set; } = 0;
        [JsonPropertyName("downloadcount")]
        public int DownloadCount { get; set; } = 0;
        [JsonPropertyName("balancecount")]
        public int BalanceCount { get; set; } = 0;
        [JsonPropertyName("month")]
        public string Month { get; set; }
        [JsonPropertyName("year")]
        public int Year { get; set; }
    }
}
