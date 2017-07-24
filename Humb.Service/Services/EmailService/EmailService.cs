using Humb.Core.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.Constants;
using Humb.Core.Interfaces;

namespace Humb.Service.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IEmailGeneratorFactory _emailGeneratorFactory;
        private readonly IEmailSender _emailSender;
        public EmailService(IEmailGeneratorFactory emailGeneratorFactory, IEmailSender emailSender)
        {
            _emailGeneratorFactory = emailGeneratorFactory;
            _emailSender = emailSender;
        }
        public void SendEmail(EmailEnums val, LanguageEnums lang, params string[] parameters)
        {
            _emailSender.Send(_emailGeneratorFactory.GetEmailGenerator(val, parameters).GenerateContent(lang));
        }
    }
}
