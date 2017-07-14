using Humb.Core.Interfaces.ProviderInterfaces.EmailProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.Constants;
using Humb.Core.Entities;
using Humb.Service.Providers.EmailContentGenerators;

namespace Humb.Service.Providers
{
    public class EmailFactory : IEmailFactory
    {
        private readonly IEmailDispatcher _emailDispatcher;
        public EmailFactory(IEmailDispatcher emailDispatcher)
        {
            this._emailDispatcher = emailDispatcher;
        }
        
        public void Initialize(EmailEnums val, object[] objs)
        {
            switch (val)
            {
                case EmailEnums.TurkishForgotPasswordEmail :
                    _emailDispatcher.Dispatch(new EmailGenerator(new TurkishForgottenPasswordEmailContentGenerator(objs), objs[0].ToString()));
                    break;

                default:
                    break;
                    

                    
            };

        }
    }
}
