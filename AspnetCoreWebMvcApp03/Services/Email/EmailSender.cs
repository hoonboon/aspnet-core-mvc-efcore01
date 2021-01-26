using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreWebMvcApp03.Services.Email
{
    public class EmailSender : IEmailSender
    {
        public EmailSenderOptions Options { get; }
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public Task SendEmailAsync(string targetEmail, string subject, string htmlMessage)
        {
            Task response;
            if (!string.IsNullOrEmpty(Options.SendGridApiKey))
            {
                response = SendViaSendGridApiAsync(targetEmail, subject, htmlMessage);
            }
            else if (!string.IsNullOrEmpty(Options.SmtpServer)
                && !string.IsNullOrEmpty(Options.SmtpPort)
                && !string.IsNullOrEmpty(Options.SmtpUsername)
                && !string.IsNullOrEmpty(Options.SmtpPassword))
            {
                response = SendViaSmtpAsync(targetEmail, subject, htmlMessage);
            }
            // TODO: implement other sending providers here
            else
            {
                throw new NotImplementedException();
            }
            return response;
        }

        private async Task SendViaSmtpAsync(string targetEmail, string subject, string htmlMessage)
        {
            _logger.LogInformation("SendViaSmtpAsync() start");
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(Options.SenderName, Options.SenderEmail));
            msg.To.Add(new MailboxAddress(targetEmail, targetEmail));
            msg.Subject = subject;
            msg.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(Options.SmtpServer, int.Parse(Options.SmtpPort), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(Options.SmtpUsername, Options.SmtpPassword);
            await smtp.SendAsync(msg);
            await smtp.DisconnectAsync(true);
            _logger.LogInformation("SendViaSmtpAsync() end");
        }

        private async Task SendViaSendGridApiAsync(string targetEmail, string subject, string htmlMessage)
        {
            _logger.LogInformation("SendViaSendGridApiAsync() start");
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Options.SenderEmail, Options.SenderName),
                Subject = subject,
                //PlainTextContent = message,
                HtmlContent = htmlMessage
            };
            msg.AddTo(new EmailAddress(targetEmail));

            msg.SetClickTracking(false, false);

            var client = new SendGridClient(Options.SendGridApiKey);
            await client.SendEmailAsync(msg);
            _logger.LogInformation("SendViaSendGridApiAsync() end");
        }
    }
}
