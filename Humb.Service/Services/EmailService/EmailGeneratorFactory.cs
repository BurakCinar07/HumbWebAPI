using Humb.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.Constants;
using Humb.Core.Entities;

namespace Humb.Service.Services.EmailService
{
    public class EmailGeneratorFactory : IEmailGeneratorFactory
    {
        public IEmailGenerator GetEmailGenerator(EmailEnums val, params string[] parameters)
        {
            if (val == EmailEnums.VerificationEmail)
                return new VerificationEmailContentGenerator(parameters);
            else
                return new ForgottenPasswordEmailContentGenerator(parameters);
        }
    }
}
