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
        private readonly object[] objs;
        public TurkishForgottenPasswordEmailContentGenerator(dynamic objs)
        {
            this.objs = objs;
        }
        public MailMessage GenerateEmailContent()
        {
            MailMessage msg = new MailMessage();
            msg.Subject = "From:noreply@mantreads.com";
            msg.IsBodyHtml = true;
            msg.Body = "<html><head></head><body>We recieved you forgot your password " + objs[1] + "!<br>Your new password is: " + objs[2] + "<br>If you didn't request a new password please ignore this mail. If not please click the link below to set your password. 82.165.97.141:4000/api/ConfirmPasswordHash?email=" + objs[0] + "&token=" + objs[3] + "</body></html>";
            return msg;
        }
    }
}
