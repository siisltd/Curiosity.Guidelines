using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using Razor.Templating.Core;
using RazorHtmlEmails.AspNetCore.Views.Emails.ConfirmAccount;


namespace RazorHtmlEmails.AspNetCore.Common
{
    public class RegisterAccountService : IRegisterAccountService
    {

        public RegisterAccountService()
        {
        }

        public async Task Register(string email, string baseUrl)
        {
            var confirmAccountModel = new ConfirmAccountEmailViewModel($"{baseUrl}/{Guid.NewGuid()}");

            string body = await RazorTemplateEngine.RenderAsync("/Views/Emails/ConfirmAccount/ConfirmAccountEmail.cshtml", confirmAccountModel);

            var toAddresses = new List<string> { email };

            SendEmail(toAddresses, "donotreply@example.com", "Confirm your Account", body);
        }
        private void SendEmail(List<string> toAddresses, string fromAddress, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SenderFirstName SenderLastName", fromAddress));
            foreach (var to in toAddresses)
            {
                message.To.Add(new MailboxAddress("RecipientFirstName RecipientLastName", to));
            }
            message.Subject = subject;

            message.Body = new TextPart(TextFormat.Html)
            {
                Text = body
            };

            using var client = new SmtpClient
            {
                ServerCertificateValidationCallback = (a, b, c, d) => true
            };

            client.Connect("127.0.0.1", 25, false);

            client.Send(message);
            client.Disconnect(true);
        }
    }

    public interface IRegisterAccountService
    {
        Task Register(string email, string baseUrl);
    }
}
