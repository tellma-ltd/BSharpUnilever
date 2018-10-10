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
    /// get a free key from SendGrid for up to 100 emails per month
    /// </summary>
    public class SendGridEmailSender : IEmailSender
    {
        private const string APIKEY_CONFIG_KEY = "SendGrid:ApiKey";
        private readonly IConfiguration _config;

        public SendGridEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmail(string emailAddress, string subject, string htmlContent)
        {
            // Read from the configuration provider
            string fromEmail = _config["ApplicationEmail"] ?? "noreply@bsharp.online";
            string sendGridApiKey = _config[APIKEY_CONFIG_KEY];

            // Scream for missing and required stuff
            if (string.IsNullOrWhiteSpace(sendGridApiKey))
                throw new InvalidOperationException($"A SendGrid API Key must be in a configuration provider under the key '{APIKEY_CONFIG_KEY}', you can get a free key on https://sendgrid.com/");

            // Prepare the message
            var client = new SendGridClient(sendGridApiKey);
            var from = new EmailAddress(fromEmail, "BSharp ERP");
            var to = new EmailAddress(emailAddress);
            SendGridMessage message =
                MailHelper.CreateSingleEmail(
                            from: from,
                            to: to,
                            subject: subject,
                            plainTextContent: "",
                            htmlContent: htmlContent);

            // Send the message
            var response = await client.SendEmailAsync(message);

            // Handle returned errors
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                // SendGrid has a quota depending on your subscription, on a free account you only get 100 emails per month
                throw new InvalidOperationException("The SendGrid subscription configured in the system has reached its limit, please contact support");
            }

            if (response.StatusCode >= HttpStatusCode.BadRequest)
            {
                throw new InvalidOperationException($"The SendGrid API returned an unknown error {response.StatusCode} when trying to send the email through it");
            }

            else return;
        }
    }
}


