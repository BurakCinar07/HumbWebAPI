using Humb.Core.Interfaces.ServiceInterfaces.MessageInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.Entities;
using Humb.Core.Constants;
using System.Net;
using System.Web;
using Pelusoft.EasyMapper;
using Humb.Core.DTOs;
using System.IO;
using System.Web.Script.Serialization;

namespace Humb.Service.Services.MessageServiceProviders
{
    public class FcmMessageSender : IMessageSender
    {
        public void SendMessage(int messageId, User fromUser, User toUser, string messageText)
        {
            try
            {
                string applicationId = ResponseConstant.APPLICATION_ID;
                string senderId = fromUser.FcmToken;
                string receiverId = toUser.FcmToken;
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";

                tRequest.ContentType = "application/json";
                var data = new
                {

                    to = receiverId,

                    data = new
                    {
                        fcmDataType = ResponseConstant.FCM_DATA_TYPE_SENT_MESSAGE,
                        message = messageText,
                        messageID = messageId,
                        sender = EasyMapper.Map<UserDTO>(fromUser)
                    }
                };

                var serializer = new JavaScriptSerializer();

                var json = serializer.Serialize(data);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationId));

                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));

                tRequest.ContentLength = byteArray.Length;


                using (Stream dataStream = tRequest.GetRequestStream())
                {

                    dataStream.Write(byteArray, 0, byteArray.Length);


                    using (WebResponse tResponse = tRequest.GetResponse())
                    {

                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {

                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                //Examine response from fcm
                                String sResponseFromServer = tReader.ReadToEnd();

                                string str = sResponseFromServer;

                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
            }
        }

       
    }
}
