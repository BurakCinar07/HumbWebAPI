using Humb.Core.Interfaces.ServiceInterfaces.PushNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Humb.Service.Services.PushNotificationService
{
    public class FcmPushNotificationContentGenerator : IPushNotificationGenerator
    {
        public byte[] GenerateContent(params object[] parameters)
        {
            var data = new
            {
                to = parameters[1],
                data = new
                {
                    sender = parameters[2],
                    book = parameters[3],
                    fcmDataType = parameters[4]
                }
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            return Encoding.UTF8.GetBytes(json);            
        }
    }
}
