using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(Message message)
        {
            var emailmessage = CreateEmailMessage(message);

            Send(emailmessage);
        }

        public async Task SendEmailAsync(Message message)
        {
            var emailmessage = CreateEmailMessage(message);

            await SendAsync(emailmessage);
        }

        private async Task SendAsync(MimeMessage emailmessage)
        {
            using (var client = new SmtpClient())
            {

                try
                {
                   await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                  await  client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

                   await client.SendAsync(emailmessage);

                }
                catch (Exception ex)
                {
                    var exex = ex.Message;
                }
                finally
                {
                   await client.DisconnectAsync(true);
                    client.Dispose();

                }

            }
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<h2 style = 'color-red'>{0}</h2>", message.Content )};

            return emailMessage;
        }

        private void Send(MimeMessage emailmessage)
        {
            using (var client = new SmtpClient())
            {

                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                    client.Send(emailmessage);

                }
                catch (Exception)
                {

                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();

                }

            }
        }

    }
}
