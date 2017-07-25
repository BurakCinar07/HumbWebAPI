using Humb.Core.Interfaces.ServiceInterfaces.InformClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Humb.Service.Services.InformClient
{

    //Inputs : 0: reciverFcmToken, 1: FcmDataType
    public class BasicFcmContentGenerator : IInformClientContentGenerator
    {
        private readonly object[] _parameters;
        public BasicFcmContentGenerator(params object[] parameters)
        {
            _parameters = parameters;
        }
        public Byte[] GenerateContent()
        {
            var data = new
            {
                to = _parameters[0],
                data = new
                {
                    fcmDataType = _parameters[1],
                }
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
