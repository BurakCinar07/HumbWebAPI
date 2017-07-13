using Humb.Core.Interfaces.ProviderInterfaces.EmailProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Humb.Core.Entities;
using System.Configuration;

namespace Humb.Service.Providers
{
    public class VerificationEmailGenerator : IEmailGenerator
    {
        public MailMessage Generate(object obj)
        {
            MailMessage msg = new MailMessage();
            var user = obj as User;

            if (user == null)
                return msg;

            msg.From = new MailAddress("noreply@mantreads.com");
            msg.To.Add(new MailAddress(user.Email));

            msg.Subject = "From:noreply@mantreads.com";
            msg.IsBodyHtml = true;
            msg.Body = string.Format("<html><head></head><body>Thanks for signing up " + user.NameSurname + "!<br>Your account has been created, you can login with the following credentials after you have activated your account by pressing the url below. <br> 82.165.97.141:4000/api/EmailVerification?email=" + user.Email + "&verificationHash=" + user.VerificationHash + "</body></html>");

            return msg;
        }
    }
}
