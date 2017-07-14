using Humb.Core.Entities;
using Humb.Core.Interfaces.ProviderInterfaces.EmailProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Service.Providers.EmailContentGenerators
{
    public class TurkishForgottenPasswordEmailContentGenerator : IEmailContentGenerator
    {
        private readonly object[] _objs;
        public TurkishForgottenPasswordEmailContentGenerator(object[] objs)
        {
            this._objs = objs;
        }
        public MailMessage GenerateEmailContent()
        {
            User user = _objs.ElementAtOrDefault(0) as User;
            ForgottenPassword fp = _objs.ElementAtOrDefault(1) as ForgottenPassword;
            if (user == null || fp == null)
                return null;
            MailMessage msg = new MailMessage();
            msg.Subject = "From:noreply@mantreads.com";
            msg.IsBodyHtml = true;
            msg.Body = "<html><head></head><body>We recieved you forgot your password " + user.NameSurname + "!<br>Your new password is: " + fp.NewPassword + "<br>If you didn't request a new password please ignore this mail. If not please click the link below to set your password. 82.165.97.141:4000/api/ConfirmPasswordHash?email=" + user.Email + "&token=" + fp.Token + "</body></html>";
            return msg;
        }
    }
}
