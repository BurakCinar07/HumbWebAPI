using Humb.Core.Interfaces.ProviderInterfaces.EmailProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;

namespace Humb.Service.Providers
{
    public class SmtpEmailDispatcher : IEmailDispatcher
    {        
        public void Dispatch(IEmailGenerator emailGenerator)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = false;
                client.Host = "mail.mantreads.com";

                client.Port = 587;
                // setup Smtp authentication
                System.Net.NetworkCredential credentials =
                    new System.Net.NetworkCredential("noreply@mantreads.com", "Burakasdf");
                client.UseDefaultCredentials = false;
                client.Credentials = credentials;
                client.Timeout = 1000;

                client.Send(emailGenerator.Generate());
            }
            catch(Exception e)
            {
                //TODO : LOGGING
            }
        }
    }
}
