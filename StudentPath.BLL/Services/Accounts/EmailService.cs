using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using MailKit.Security;
using StudentPath.BLL.Dtos.Accounts;
using Microsoft.AspNetCore.Http;


namespace StudentPath.BLL.Services.AccountService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GeneralRespnose> SendEmailAsync(string email, string subject, string message)
        {
            
            var response = new GeneralRespnose();
            var emailMessage = new MimeMessage();

            // Sender
            emailMessage.From.Add(new MailboxAddress(_configuration["EmailSettings:DisplayName"], _configuration["EmailSettings:Email"]));

            // Receiver
            emailMessage.To.Add(new MailboxAddress("", email));

            // Subject
            emailMessage.Subject = subject;

           
            emailMessage.Body = new TextPart("html") { Text = message };


            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(
                        _configuration["EmailSettings:Host"],
                        int.Parse(_configuration["EmailSettings:Port"]),
                        SecureSocketOptions.StartTls
                    );

                    await client.AuthenticateAsync(
                        _configuration["EmailSettings:Email"],
                        _configuration["EmailSettings:Password"]
                    );

                    await client.SendAsync(emailMessage);
                    response.successed = true;
                }
                catch (Exception ex)
                {
                    response.Errors.Add(ex.Message);
                }
                finally
                {
                    if (client.IsConnected)
                    {
                        await client.DisconnectAsync(true);
                    }
                }
            }

            return response;
        }

    }
}