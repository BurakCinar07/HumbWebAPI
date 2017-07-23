using Humb.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;
using Humb.Core.Constants;
using Humb.Core.Entities;

namespace Humb.Service.Services.EmailService
{
    public class VerificationEmailContentGenerator : IEmailGenerator
    {
        private readonly string[] _parameters;
        public VerificationEmailContentGenerator(params string[] parameters)
        {
            _parameters = parameters;
        }
        public MailMessage GenerateContent(EmailLanguageEnums val)
        {
            MailMessage msg = new MailMessage();
            msg.IsBodyHtml = true;
            msg.From = new MailAddress(ConfigurationManager.AppSettings["SmtpMailAddress"]);
            msg.To.Add(new MailAddress(_parameters[0]));

            if (val == EmailLanguageEnums.Turkish)
            {
                msg.Subject = "Humb Email Doğrulama";
                msg.Body = string.Format("<html><head></head><body>Thanks for signing up " + _parameters[1] + "!<br>Your account has been created, you can login with the following credentials after you have activated your account by pressing the url below. <br> 82.165.97.141:4000/api/EmailVerification?email=" + _parameters[0] + "&verificationHash=" + _parameters[2] + "</body></html>");
            }
            else
            {
                msg.Subject = "Humb Email Verification";
                msg.Body = string.Format("<html><head></head><body>Thanks for signing up " + _parameters[1] + "!<br>Your account has been created, you can login with the following credentials after you have activated your account by pressing the url below. <br> 82.165.97.141:4000/api/EmailVerification?email=" + _parameters[0] + "&verificationHash=" + _parameters[2] + "</body></html>");
            }
            return msg;
        }
    }
}
