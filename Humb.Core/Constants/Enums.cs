using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Constants
{
    public enum EmailEnums
    {
        VerificationEmail,
        ForgottenPasswordEmail
    }
    public enum LanguageEnums
    {
        Turkish,
        English
    }
    public enum InformClientEnums
    {
        NotificationRequest,
        EmailVerifiedRequest,
        SendMessageRequest,
        UpdateMessageStateRequest
    }
}
