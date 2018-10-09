using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BSharpUnilever.Services
{
    /// <summary>
    /// An implementation of IEmailSender that sends the email over the SendGrid service
    /// https://sendgrid.com/. Using this implementation requires that you configure
    /// a SendGrid API key in a configuration provider under "SendGrid:ApiKey", you can 
    /// get a free key from SendGrid and you get to send up to 100 emails per month
    /// </summary>
    public class SendGridEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public SendGridEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmail(string emailAddress, string htmlContent)
        {
            string fromEmail = _config["ApplicationEmail"] ?? "noreply@bsharp.online";
            string sendGridApiKey = _config["SendGrid:ApiKey"];
            if (string.IsNullOrWhiteSpace(sendGridApiKey))
                throw new InvalidOperationException("A SendGrid API Key must be in a configuration provider under the key 'SendGrid:ApiKey', you can get a free key on https://sendgrid.com/");

            var client = new SendGridClient(sendGridApiKey);
            var from = new EmailAddress(fromEmail, "BSharp ERP");
            var to = new EmailAddress(emailAddress);
            var subject = "Password Reset Link";

            SendGridMessage message =
                MailHelper.CreateSingleEmail(
                            from: from,
                            to: to,
                            subject: subject,
                            plainTextContent: "",
                            htmlContent: htmlContent);

            var response = await client.SendEmailAsync(message);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                // SendGrid has a quota depending on your subscription, you only get 100 emails per month for free
                throw new InvalidOperationException("The SendGrid subscription configured in the system has been reached its limit, please contact support");
            }

            if (response.StatusCode >= HttpStatusCode.BadRequest)
            {
                throw new InvalidOperationException("An unknown error occured while sending the email through SendGrid");
            }

            else return;
        }
    }
}


