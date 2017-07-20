using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.Constants;
using Humb.Core.Entities;
using Humb.Service.Services.EmailService.EmailGenerators;
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
                    _emailDispatcher.Dispatch(new EmailGenerator(objs[0].ToString() , "Humb Forgot Password",
                        "<html><head></head><body>We recieved you forgot your password " + objs[1] + "!<br>Your new password is: " + objs[2] + "<br>If you didn't request a new password please ignore this mail. If not please click the link below to set your password. 82.165.97.141:4000/api/ConfirmPasswordHash?email=" + objs[0] + "&token=" + objs[3] + "</body></html>"));
                    break;
                case EmailEnums.TurkishVerificationEmail:
                    _emailDispatcher.Dispatch(new EmailGenerator("","",""));
                    break;
                default:
                    break;
            };

        }
    }
}
