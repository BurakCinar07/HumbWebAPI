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
                to = parameters[0],
                data = new
                {
                    sender = parameters[1],
                    book = parameters[2],
                    fcmDataType = parameters[3]
                }
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            return Encoding.UTF8.GetBytes(json);            
        }
    }
}
