using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;
using Humb.Core.Interfaces;

namespace Humb.Service.Services.EmailService.EmailDispatchers
{
    public class SmtpEmailSender : IEmailSender
    {       

        public void Send(MailMessage msg)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = false;
                client.Host = ConfigurationManager.AppSettings["SmtpServerHost"];
                client.Port = int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"]);
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SmtpServerUserName"], ConfigurationManager.AppSettings["SmtpServerPassword"]);
                client.UseDefaultCredentials = false;
                client.Credentials = credentials;
                client.Timeout = 1000;

                client.Send(msg);
            }
            catch (Exception e)
            {
                //TODO : LOGGING
            }
        }
    }
}
