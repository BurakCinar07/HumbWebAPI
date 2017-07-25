using Humb.Core.Interfaces.ServiceInterfaces.InformClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Humb.Service.Services.InformClient.FcmService
{
    //Inputs: 0: reciverFcmToken, 1:MessageId, 2:fcmDataType
    public class FcmUpdateMessageStateContentGenerator : IInformClientContentGenerator
    {
        private readonly object[] _parameters;
        public FcmUpdateMessageStateContentGenerator(params object[] parameters)
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
                    messageID = _parameters[1],
                    fcmDataType = _parameters[2],
                }
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
