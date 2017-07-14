using Humb.Core.Interfaces.ProviderInterfaces.EmailProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Humb.Core.Entities;
using System.Configuration;

namespace Humb.Service.Providers
{
    public class VerificationEmailGenerator : IEmailGenerator
    {
        private readonly IEmailContentGenerator _emailContentGenerator;
        private readonly string _sendToEmailAddress;
        public VerificationEmailGenerator(IEmailContentGenerator emailContentGenerator, string sendToEmailAddress)
        {
            this._emailContentGenerator = emailContentGenerator;
            this._sendToEmailAddress = sendToEmailAddress;
        }
        public MailMessage Generate()
        {
            MailMessage msg = _emailContentGenerator.GenerateEmailContent();
            msg.From = new MailAddress("noreply@mantreads.com");
            msg.To.Add(new MailAddress(_sendToEmailAddress));
            return msg;
        }
    }
}
