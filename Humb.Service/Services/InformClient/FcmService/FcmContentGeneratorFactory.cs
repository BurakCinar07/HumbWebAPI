using Humb.Core.Constants;
using Humb.Core.Interfaces.ServiceInterfaces.InformClient;
using Humb.Service.Services.InformClient.FcmService;
using Humb.Service.Services.PushNotificationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Service.Services.InformClient
{
    public class FcmContentGeneratorFactory : IInformClientContentGeneratorFactory
    {       
        
        public IInformClientContentGenerator GenerateWebRequest(InformClientEnums val, params object[] parameters)
        {
            if (val == InformClientEnums.NotificationRequest)
                return new FcmPushNotificationContentGenerator(parameters);
            else if (val == InformClientEnums.EmailVerifiedRequest)
                return new BasicFcmContentGenerator(parameters);
            else if (val == InformClientEnums.MessageRecievedRequest)
                return new FcmMessageContentGenerator(parameters);
            else if (val == InformClientEnums.UpdateMessageStateRequest)
                return new FcmUpdateMessageStateContentGenerator(parameters);

            return null;
        }        
    }
}
