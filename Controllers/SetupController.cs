using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OrgCheck.Controllers
{
    // Temporary, secret-protected endpoint used once to load the database schema from inside
    // Render's network (works around external networks that block TLS on port 5432).
    // Remove this controller once the database has been initialized.
    [Route("api/setup")]
    public class SetupController : Controller
    {
        [HttpPost("run")]
        public async Task<IActionResult> Run(string secret)
        {
            string expectedSecret = Environment.GetEnvironmentVariable("SETUP_SECRET");
            if (string.IsNullOrEmpty(expectedSecret) || secret != expectedSecret)
                return Unauthorized();

            string databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (string.IsNullOrEmpty(databaseUrl))
                return BadRequest("DATABASE_URL not set");

            var results = new List<object>
            {
                await RunCommandAsync("pg_restore", new[] { "--no-owner", "-d", databaseUrl, "/app/dbscripts/orgcheck_17022025.sql" }),
                await RunCommandAsync("psql", new[] { databaseUrl, "-f", "/app/dbscripts/employeeconsent_schema.sql" }),
                await RunCommandAsync("psql", new[] { databaseUrl, "-f", "/app/dbscripts/employeeconsent_v2_document_audit.sql" }),
                await RunCommandAsync("psql", new[] { databaseUrl, "-f", "/app/dbscripts/demo_accounts.sql" })
            };

            return Json(results);
        }

        [HttpPost("test-smtp")]
        public async Task<IActionResult> TestSmtp(string secret)
        {
            string expectedSecret = Environment.GetEnvironmentVariable("SETUP_SECRET");
            if (string.IsNullOrEmpty(expectedSecret) || secret != expectedSecret)
                return Unauthorized();

            // 1) Raw TCP reachability to the SMTP port from inside Render's network.
            string tcpResult;
            try
            {
                using var tcpClient = new TcpClient();
                await tcpClient.ConnectAsync("smtp.gmail.com", 587, new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
                tcpResult = "TCP connect to smtp.gmail.com:587 succeeded";
            }
            catch (Exception ex)
            {
                tcpResult = $"TCP connect failed: {ex.GetType().Name}: {ex.Message}";
            }

            // 2) Actual synchronous SmtpClient send (same client/config the app uses, but awaited so the real
            // exception surfaces instead of being silently dropped like the app's fire-and-forget SendAsync).
            string sendResult;
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("it.support@verifyzone.in", "VerifyZone Support"),
                    Subject = "SetupController SMTP diagnostic",
                    Body = "test",
                    IsBodyHtml = false
                };
                mailMessage.To.Add("it.support@verifyzone.in");

                using var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("it.support@verifyzone.in", Environment.GetEnvironmentVariable("ApplicationSettings__EmailFromPass")),
                    EnableSsl = true,
                    Timeout = 15000
                };
                await smtpClient.SendMailAsync(mailMessage);
                sendResult = "SendMailAsync succeeded";
            }
            catch (Exception ex)
            {
                sendResult = $"SendMailAsync failed: {ex.GetType().Name}: {ex.Message} | InnerException: {ex.InnerException?.Message}";
            }

            return Json(new { tcpResult, sendResult });
        }

        private async Task<object> RunCommandAsync(string fileName, string[] args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            foreach (var a in args)
                psi.ArgumentList.Add(a);

            using var process = Process.Start(psi);
            string stdout = await process.StandardOutput.ReadToEndAsync();
            string stderr = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            return new
            {
                command = fileName,
                exitCode = process.ExitCode,
                stdout = Truncate(stdout, 4000),
                stderr = Truncate(stderr, 4000)
            };
        }

        private static string Truncate(string s, int max)
            => string.IsNullOrEmpty(s) || s.Length <= max ? s : s.Substring(0, max) + "...(truncated)";
    }
}
