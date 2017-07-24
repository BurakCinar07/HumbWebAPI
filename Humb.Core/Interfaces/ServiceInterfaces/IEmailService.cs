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
        void SendEmail(EmailEnums val, LanguageEnums lang, params string[] parameters);
    }
}
