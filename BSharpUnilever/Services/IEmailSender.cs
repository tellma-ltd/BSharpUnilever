using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSharpUnilever.Services
{
    public interface IEmailSender
    {
        Task SendEmail(string destinationEmailAddress, string subject, string htmlEmail);
    }
}
