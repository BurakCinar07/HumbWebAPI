using Humb.Core.Interfaces.ServiceInterfaces.InformClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Humb.Service.Services.PushNotificationService
{
    public class FcmPushNotificationContentGenerator : IInformClientContentGenerator
    {
        private readonly object[] _parameters;
        public FcmPushNotificationContentGenerator(params object[] parameters)
        {
            _parameters = parameters;
        }
        public byte[] GenerateContent()
        {
            var data = new
            {
                to = _parameters[0],
                data = new
                {
                    sender = _parameters[1],
                    book = _parameters[2],
                    fcmDataType = _parameters[3]
                }
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
