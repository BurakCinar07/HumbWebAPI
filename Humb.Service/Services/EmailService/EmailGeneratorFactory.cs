using Humb.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.Constants;

namespace Humb.Service.Services.EmailService
{
    public class EmailGeneratorFactory : IEmailGeneratorFactory
    {
        public IEmailGenerator GetEmailGenerator(EmailEnums val)
        {
            if(val == EmailEnums.TurkishVerificationEmail)
            {
            }
            return null;
        }
    }
}
