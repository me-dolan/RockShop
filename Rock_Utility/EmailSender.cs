using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using System;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Configuration;

namespace Rock_Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config; 
        public EmailSettings _emailSettings { get; set; }
        public EmailSender(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _emailSettings = _config.GetSection("EmailConfiguration").Get<EmailSettings>();
            
            MimeMessage emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_emailSettings.Author, _emailSettings.Email));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
            };

            try
            {
                using (SmtpClient client = new SmtpClient())
                {
                    await client.ConnectAsync(_emailSettings.SmtpSever, _emailSettings.SmtpPort, true);
                    await client.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);

                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                            
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
