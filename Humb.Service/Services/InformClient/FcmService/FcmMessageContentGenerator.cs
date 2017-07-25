using Humb.Core.Interfaces.ServiceInterfaces.InformClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Humb.Service.Services.InformClient.FcmService
{
    //Inputs: 0: recieverFcmToken, 1: senderUser object, 2: MessageText, 3: MessageId, 4: fcmDataType
    public class FcmMessageContentGenerator : IInformClientContentGenerator
    {
        private readonly object[] _parameters;
        public FcmMessageContentGenerator(params object[] parameters)
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
                    message = _parameters[2],
                    messageID = _parameters[3],
                    fcmDataType = _parameters[4]
                }
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
