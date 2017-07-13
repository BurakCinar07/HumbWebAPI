using Humb.Core.Interfaces.ProviderInterfaces.EmailProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Humb.Core.Entities;

namespace Humb.Service.Providers
{
    public class ForgottenPasswordEmailGenerator : IEmailGenerator
    {
        public MailMessage Generate(dynamic obj)
        {
            MailMessage msg = new MailMessage();
            var user = obj.user as User;
            var fp = obj.fp;

            if (user == null)
                return msg;

            msg.From = new MailAddress("noreply@mantreads.com");
            msg.To.Add(new MailAddress(user.Email));

            msg.Subject = "From:noreply@mantreads.com";
            msg.IsBodyHtml = true;
            msg.Body = "<html><head></head><body>We recieved you forgot your password " + user.NameSurname + "!<br>Your new password is: " + fp.NewPassword + "<br>If you didn't request a new password please ignore this mail. If not please click the link below to set your password. 82.165.97.141:4000/api/ConfirmPasswordHash?email=" + user.Email + "&token=" + fp.Token + "</body></html>";
            return msg;
        }
    }
}
