using Incubator_2.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace Incubator_2.Common
{

    public static class MailService
    {
        public static async Task SendEmailAsync(string email, string subject, string message, MailProfile profile)
        {
            using MimeMessage emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(profile.name, profile.email));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using SmtpClient client = new SmtpClient();
            await client.ConnectAsync(profile.host, profile.port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(profile.email, profile.password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}
