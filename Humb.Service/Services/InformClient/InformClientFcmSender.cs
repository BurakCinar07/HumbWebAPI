using Humb.Core.Constants;
using Humb.Core.Interfaces.ServiceInterfaces.InformClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Service.Services.InformClient
{
    public class InformClientFcmSender : IInformClientDataSender
    {
        public void SendData(byte[] content)
        {
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = "application/json";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", ResponseConstant.APPLICATION_ID));
            tRequest.ContentLength = content.Length;
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(content, 0, content.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
                        {
                            String sResponseFromServer = tReader.ReadToEnd();
                            string str = sResponseFromServer;
                        }
                    }
                }
            }
        }
    }
}
