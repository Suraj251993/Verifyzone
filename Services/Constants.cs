using System;
namespace OrgCheck.Services
{
    public class Constants
    {
        public string SecretKey { get; set; }
        public string EmailAPIKey { get; set; }
        public string EmailFromId { get; set; }
        public string EmailFromPass { get; set; }
        public string EmailFromUsername { get; set; }
        public string PasswordSalt { get; set; }
        public string SmtpClient { get; set; }
        public int SmtpPort { get; set; }
        public string NoRecordNotificationEmail { get; set; }
        public string AppLog { get; set; }
        public string UploadPath { get; set; }
        public string Reports { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public string AWSBucketName { get; set; }
        public int CreditQuestionThreshold { get; set; }
    }
}
