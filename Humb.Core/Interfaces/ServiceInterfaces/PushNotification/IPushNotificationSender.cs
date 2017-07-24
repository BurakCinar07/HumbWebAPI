using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces.PushNotification
{
    public interface IPushNotificationSender
    {
        void SendPushNotification(string senderFcmToken, byte[] content);
    }
}
