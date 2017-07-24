using Humb.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces
{
    public interface IEmailService
    {
        void SendEmail(EmailEnums val, EmailLanguageEnums lang, params string[] parameters);
    }
}
