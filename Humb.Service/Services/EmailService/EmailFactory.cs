using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.Constants;
using Humb.Core.Entities;
using Humb.Service.Services.EmailService.EmailGenerators;
using Humb.Service.Services.EmailService.EmailContentGenerators;
using Humb.Core.Interfaces.ServiceInterfaces.EmailInterfaces;

namespace Humb.Service.Services.EmailService
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
                case EmailEnums.TurkishForgotPasswordEmail:
                    _emailDispatcher.Dispatch(new EmailGenerator(new TurkishForgottenPasswordEmailContentGenerator(objs), objs[0].ToString()));
                    break;
                case EmailEnums.TurkishVerificationEmail:
                    _emailDispatcher.Dispatch(new EmailGenerator(new TurkishVerificationEmailContentGenerator(objs), objs[0].ToString()));
                    break;
                default:
                    break;
                    

                    
            };

        }
    }
}
