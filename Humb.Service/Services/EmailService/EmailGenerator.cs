using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Humb.Core.Entities;
using System.Configuration;
using Humb.Core.Interfaces;

namespace Humb.Service.Services.EmailService.EmailGenerators
{
    public class EmailGenerator : IEmailGenerator
    {
        private readonly string _sendToEmailAddress;
        private readonly string _subject;
        private readonly string _body;
        public EmailGenerator(string sendToEmailAddress, string subject, string body)
        {
            this._sendToEmailAddress = sendToEmailAddress;
            this._subject = subject;
            this._body = body;
        }
        public MailMessage GenerateContent()
        {
            //Eğer ing ise ayrı tr ise ayrı
            MailMessage msg = new MailMessage();
            msg.Subject = _subject;
            msg.IsBodyHtml = true;
            msg.Body = _body;
            msg.From = new MailAddress(ConfigurationManager.AppSettings["SmtpMailAddress"]);
            msg.To.Add(new MailAddress(_sendToEmailAddress));
            return msg;
        }
    }
}
