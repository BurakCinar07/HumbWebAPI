using Humb.Core.Interfaces.ProviderInterfaces.EmailProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Service.Providers.EmailContentGenerators
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
