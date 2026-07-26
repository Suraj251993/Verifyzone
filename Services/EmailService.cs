using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OrgCheck.Services
{
    public class EmailService
    {
        private readonly Constants _constants;
        private readonly LogService _logService;
        public EmailService(Constants constants, LogService logService)
        {
            _constants = constants;
            _logService = logService;
        }
        public void SendEmail(string toEmail, string ccEmail, string bccEmail, string subject, string body)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_constants.EmailFromId, _constants.EmailFromUsername);
            mailMessage.To.Add(toEmail);
            if (!string.IsNullOrEmpty(ccEmail))
                mailMessage.CC.Add(ccEmail);
            if (!string.IsNullOrEmpty(bccEmail))
                mailMessage.Bcc.Add(bccEmail);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient(_constants.SmtpClient);
            smtpClient.Port = _constants.SmtpPort;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(_constants.EmailFromId, _constants.EmailFromPass);
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.SendAsync(mailMessage, null);
            }
            catch (Exception ex)
            {
                _logService.Log(ex);
            }
        }

        public void SendConsentRequestEmail(string toEmail, string employeeName, string companyName, string consentLink, string expiryDate, string hrContact)
        {
            // Template is dynamic - variables can be re-arranged/re-styled once the final copy is supplied.
            string subject = $"Action required: Consent request from {companyName}";
            string body = $@"
                <p>Hello {employeeName},</p>
                <p>{companyName} has requested your consent as part of their verification process on VerifyZone.</p>
                <p>Please click the link below to review and submit your consent. This link is valid until <strong>{expiryDate}</strong> and can be used only once.</p>
                <p><a href='{consentLink}'>{consentLink}</a></p>
                <p>If you have any questions, please contact {hrContact}.</p>
                <br>
                <p>Regards,<br>{companyName}</p>";

            SendEmail(toEmail, string.Empty, string.Empty, subject, body);
        }
    }
}
