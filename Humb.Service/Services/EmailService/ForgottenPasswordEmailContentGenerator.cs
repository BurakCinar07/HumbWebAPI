using Humb.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.Constants;
using System.Net.Mail;
using System.Configuration;

namespace Humb.Service.Services.EmailService
{
    public class ForgottenPasswordEmailContentGenerator : IEmailGenerator
    {
        private readonly string[] _parameters;
        public ForgottenPasswordEmailContentGenerator(params string[] parameters)
        {
            _parameters = parameters;
        }
        public MailMessage GenerateContent(LanguageEnums val)
        {
            MailMessage msg = new MailMessage();
            msg.IsBodyHtml = true;
            msg.From = new MailAddress(ConfigurationManager.AppSettings["SmtpMailAddress"]);
            msg.To.Add(new MailAddress(_parameters[0]));

            if (val == LanguageEnums.Turkish)
            {
                msg.Subject = "Humb Şifremi Unuttum";
                msg.Body = string.Format("<html><head></head><body>We recieved you forgot your password " + _parameters[1] + "!<br>Your new password is: " + _parameters[2] + "<br>If you didn't request a new password please ignore this mail. If not please click the link below to set your password. 82.165.97.141:4000/api/ConfirmPasswordHash?email=" + _parameters[0] + "&token=" + _parameters[3] + "</body></html>");
            }
            else
            {
                msg.Subject = "Humb Forgot Password";
                msg.Body = string.Format("<html><head></head><body>We recieved you forgot your password " + _parameters[1] + "!<br>Your new password is: " + _parameters[2] + "<br>If you didn't request a new password please ignore this mail. If not please click the link below to set your password. 82.165.97.141:4000/api/ConfirmPasswordHash?email=" + _parameters[0] + "&token=" + _parameters[3] + "</body></html>");
            }
            return msg;
        }
    }
}
