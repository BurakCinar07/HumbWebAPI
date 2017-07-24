using Humb.Core.Interfaces;
using Humb.Core.Interfaces.ServiceInterfaces.PushNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Service.Services.PushNotificationService
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly IPushNotificationGenerator _pushNotificationGenerator;
        private readonly IPushNotificationSender _pushNotificationSender;
        public PushNotificationService(IPushNotificationGenerator pushNotificationGenerator, IPushNotificationSender pushNotificationSender)
        {
            _pushNotificationGenerator = pushNotificationGenerator;
            _pushNotificationSender = pushNotificationSender;
        }

        public void SendPushNotification(params object[] parameters)
        {
            _pushNotificationSender.SendPushNotification(parameters[0].ToString(), _pushNotificationGenerator.GenerateContent(parameters));
        }
    }
}
