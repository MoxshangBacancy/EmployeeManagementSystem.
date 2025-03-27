using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Employee_Management_System.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSettings = _config.GetSection("EmailSettings");
            string smtpServer = smtpSettings["SmtpServer"];
            int smtpPort = int.Parse(smtpSettings["SmtpPort"]);
            string smtpUser = smtpSettings["SmtpUser"];
            string smtpPass = smtpSettings["SmtpPass"];

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.EnableSsl = true; 

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
