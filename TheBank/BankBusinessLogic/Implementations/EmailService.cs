using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace BankBusinessLogic.Implementations
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;

        public EmailService(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
        }

        public static EmailService CreateYandexService()
        {
            return new EmailService(
                smtpServer: "smtp.yandex.ru",
                smtpPort: 465,
                smtpUsername: "egoffevgeny@yandex.com",
                smtpPassword: "mpaffjmvyulsdpev"
            );
        }

        public async Task SendReportAsync(string toEmail, string subject, string body, string attachmentPath = null)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Bank System", _smtpUsername));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = body;

            if (!string.IsNullOrEmpty(attachmentPath))
            {
                builder.Attachments.Add(attachmentPath);
            }

            email.Body = builder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync(_smtpUsername, _smtpPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }

        public async Task SendTestEmailAsync(string toEmail)
        {
            await SendReportAsync(
                toEmail: toEmail,
                subject: "Тестовое письмо от банковской системы",
                body: "<h1>Тестовое письмо</h1><p>Это тестовое письмо отправлено для проверки работы системы.</p>"
            );
        }
    }
} 