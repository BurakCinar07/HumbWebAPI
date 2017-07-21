using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces
{
    public interface IEmailGenerator
    {
        MailMessage GenerateContent();
    }
}
