using Humb.Core.Interfaces.ServiceInterfaces.EmailInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Service.Services.EmailService.EmailContentGenerators
{
    public class TurkishVerificationEmailContentGenerator : IEmailContentGenerator
    {
        private readonly object[] _obj;
        public TurkishVerificationEmailContentGenerator(object[] obj)
        {
            this._obj = obj;
        }
        public MailMessage GenerateEmailContent()
        {
            throw new NotImplementedException();
        }
    }
}
