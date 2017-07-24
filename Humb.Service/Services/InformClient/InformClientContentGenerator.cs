using Humb.Core.Interfaces.ServiceInterfaces.InformClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Humb.Service.Services.InformClient
{
    public class InformClientContentGenerator : IInformClientContentGenerator
    {
        public Byte[] GenerateContent(params string[] parameters)
        {
            var data = new
            {
                to = parameters[0],
                data = new
                {
                    fcmDataType = parameters[1],
                }
            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
