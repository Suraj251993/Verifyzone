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

        public void SendConsentRequestEmail(string toEmail, string employeeName, string companyName, string consentLink, string expiryDate, string hrContact, string baseUrl)
        {
            string subject = $"Action required: {companyName} has requested your consent on VerifyZone";
            string body = BuildConsentRequestEmailBody(employeeName, companyName, toEmail, consentLink, expiryDate, hrContact, baseUrl);

            SendEmail(toEmail, string.Empty, string.Empty, subject, body);
        }

        // Template is dynamic by design - variables (name, company, link, expiry, contact) are interpolated
        // so the copy/branding below can evolve without touching the calling code.
        private string BuildConsentRequestEmailBody(string employeeName, string companyName, string employeeEmail, string consentLink, string expiryDate, string hrContact, string baseUrl)
        {
            const string purple = "#5c249a";
            const string darkPurple = "#340c62";
            const string pageBg = "#f4f1f8";
            const string cardBg = "#ffffff";
            const string cardBorder = "#e8e1f2";
            const string textDark = "#2b2340";
            const string textMuted = "#6b6478";
            string safeEmployeeName = WebUtility.HtmlEncode(employeeName);
            string safeCompanyName = WebUtility.HtmlEncode(companyName);
            string safeEmployeeEmail = WebUtility.HtmlEncode(employeeEmail);
            string safeHrContact = WebUtility.HtmlEncode(hrContact);
            string safeConsentLink = WebUtility.HtmlEncode(consentLink);
            string logoUrl = $"{baseUrl}/assets/img/white-logo.png";

            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<title>VerifyZone Consent Request</title>
<style>
  @media only screen and (max-width: 600px) {{
    .vz-container {{ width: 100% !important; }}
    .vz-px {{ padding-left: 20px !important; padding-right: 20px !important; }}
    .vz-py {{ padding-top: 28px !important; padding-bottom: 28px !important; }}
  }}
</style>
</head>
<body style=""margin:0; padding:0; background-color:{pageBg}; font-family:'IBM Plex Sans', Arial, Helvetica, sans-serif;"">
  <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:{pageBg}; padding:32px 0;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" class=""vz-container"" width=""600"" cellpadding=""0"" cellspacing=""0"" style=""width:600px; max-width:600px; background-color:{cardBg}; border-radius:12px; overflow:hidden; border:1px solid {cardBorder};"">

          <!-- Header -->
          <tr>
            <td class=""vz-px vz-py"" align=""center"" style=""background:linear-gradient(135deg, {purple} 0%, {darkPurple} 100%); padding:36px 40px;"">
              <img src=""{logoUrl}"" alt=""VerifyZone"" height=""34"" style=""display:block; margin:0 auto 18px auto; height:34px;"" />
              <div style=""color:#ffffff; font-size:20px; font-weight:600; line-height:1.4;"">
                Welcome to VerifyZone &mdash; trusted employment verification, made simple.
              </div>
            </td>
          </tr>

          <!-- Main content -->
          <tr>
            <td class=""vz-px"" style=""padding:36px 40px 8px 40px;"">
              <p style=""margin:0 0 6px 0; font-size:15px; color:{textMuted};"">Hello {safeEmployeeName},</p>
              <h1 style=""margin:0 0 14px 0; font-size:22px; line-height:1.35; color:{textDark}; font-weight:600;"">
                Your consent is requested for employment verification
              </h1>
              <p style=""margin:0 0 20px 0; font-size:15px; line-height:1.6; color:{textMuted};"">
                <strong style=""color:{textDark};"">{safeCompanyName}</strong> has requested your consent as part of an
                employment verification process on VerifyZone. Please review your details below and confirm your
                consent so the request can proceed.
              </p>
            </td>
          </tr>

          <!-- Employee info card -->
          <tr>
            <td class=""vz-px"" style=""padding:0 40px 24px 40px;"">
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:{pageBg}; border:1px solid {cardBorder}; border-radius:10px;"">
                <tr>
                  <td style=""padding:20px 24px;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""padding:6px 0; font-size:13px; color:{textMuted}; width:40%;"">Employee name</td>
                        <td style=""padding:6px 0; font-size:14px; color:{textDark}; font-weight:600; text-align:right;"">{safeEmployeeName}</td>
                      </tr>
                      <tr>
                        <td style=""padding:6px 0; font-size:13px; color:{textMuted}; border-top:1px solid {cardBorder};"">Registered email</td>
                        <td style=""padding:6px 0; font-size:14px; color:{textDark}; font-weight:600; text-align:right; border-top:1px solid {cardBorder};"">{safeEmployeeEmail}</td>
                      </tr>
                      <tr>
                        <td style=""padding:6px 0; font-size:13px; color:{textMuted}; border-top:1px solid {cardBorder};"">Requested by</td>
                        <td style=""padding:6px 0; font-size:14px; color:{textDark}; font-weight:600; text-align:right; border-top:1px solid {cardBorder};"">{safeCompanyName}</td>
                      </tr>
                      <tr>
                        <td style=""padding:6px 0; font-size:13px; color:{textMuted}; border-top:1px solid {cardBorder};"">Link expires on</td>
                        <td style=""padding:6px 0; font-size:14px; color:{textDark}; font-weight:600; text-align:right; border-top:1px solid {cardBorder};"">{expiryDate}</td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- CTA -->
          <tr>
            <td class=""vz-px"" align=""center"" style=""padding:0 40px 32px 40px;"">
              <table role=""presentation"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td align=""center"" style=""border-radius:8px; background-color:{purple};"">
                    <a href=""{safeConsentLink}"" target=""_blank""
                       style=""display:inline-block; padding:14px 32px; font-size:15px; font-weight:600; color:#ffffff; text-decoration:none; border-radius:8px;"">
                      Review &amp; Provide Consent
                    </a>
                  </td>
                </tr>
              </table>
              <p style=""margin:14px 0 0 0; font-size:12px; color:{textMuted};"">
                This link is valid until <strong>{expiryDate}</strong> and can only be used once.
              </p>
              <p style=""margin:6px 0 0 0; font-size:12px; word-break:break-all;"">
                <a href=""{safeConsentLink}"" style=""color:{purple};"">{safeConsentLink}</a>
              </p>
            </td>
          </tr>

          <!-- Platform introduction -->
          <tr>
            <td style=""padding:0 40px 8px 40px;"">
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""border-top:1px solid {cardBorder}; padding-top:24px;"">
                <tr>
                  <td>
                    <p style=""margin:0 0 14px 0; font-size:13px; font-weight:600; letter-spacing:0.04em; text-transform:uppercase; color:{purple};"">
                      Why VerifyZone
                    </p>
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td style=""padding:0 0 14px 0; vertical-align:top; width:28px; font-size:16px; color:{purple};"">&#9679;</td>
                        <td style=""padding:0 0 14px 0; font-size:14px; line-height:1.5; color:{textMuted};"">
                          <strong style=""color:{textDark};"">Secure, one-time consent links</strong> &mdash; every request is
                          protected by an expiring, single-use token.
                        </td>
                      </tr>
                      <tr>
                        <td style=""padding:0 0 14px 0; vertical-align:top; width:28px; font-size:16px; color:{purple};"">&#9679;</td>
                        <td style=""padding:0 0 14px 0; font-size:14px; line-height:1.5; color:{textMuted};"">
                          <strong style=""color:{textDark};"">Full transparency</strong> &mdash; track exactly what
                          information is shared and with whom, at every step.
                        </td>
                      </tr>
                      <tr>
                        <td style=""padding:0; vertical-align:top; width:28px; font-size:16px; color:{purple};"">&#9679;</td>
                        <td style=""padding:0; font-size:14px; line-height:1.5; color:{textMuted};"">
                          <strong style=""color:{textDark};"">Built for the long term</strong> &mdash; add an alternate
                          email so you can keep track of your verification status even after you move on.
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style=""padding:28px 40px 32px 40px; background-color:{pageBg};"">
              <p style=""margin:0 0 6px 0; font-size:13px; color:{textDark}; font-weight:600;"">VerifyZone</p>
              <p style=""margin:0 0 12px 0; font-size:12px; color:{textMuted};"">
                Questions about this request? Contact {safeHrContact}.
              </p>
              <p style=""margin:0; font-size:11px; color:{textMuted};"">
                This is an automated message regarding your employment verification consent. If you weren't
                expecting this, you can safely ignore it or reach out to the sender above.
              </p>
              <p style=""margin:14px 0 0 0; font-size:11px; color:{textMuted};"">
                &copy; {DateTime.UtcNow.Year} VerifyZone. All rights reserved.
              </p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        }
    }
}
