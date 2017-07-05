using BookieAPI.Constants;
using BookieAPI.Controllers.Utils.ModelUtils;
using BookieAPI.Models.Context;
using BookieAPI.Models.ResponseModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace BookieAPI.Controllers.Utils
{
    public static class FcmUtils
    {
        internal static void SendRequestNotification(Context context, int bookID, int fromUserID, int toUserID, int dataType)
        {

            string applicationID = ResponseConstant.APPLICATION_ID;

            string senderId = UserUtils.GetUserFcmToken(context, fromUserID);
            string receiverId = UserUtils.GetUserFcmToken(context, toUserID);

            if (!string.IsNullOrEmpty(senderId) && !string.IsNullOrEmpty(receiverId))
            {
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";

                tRequest.ContentType = "application/json";
                UserModel um = new UserModel();
                um = UserUtils.GetUserModel(context, fromUserID);
                BookModel bm = BookUtils.GetBookModel(context, bookID);
                var data = new
                {

                    to = receiverId,
                    data = new
                    {
                        fcmDataType = dataType,
                        sender = um,
                        book = bm
                    }

                };

                var serializer = new JavaScriptSerializer();

                var json = serializer.Serialize(data);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

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

                                String sResponseFromServer = tReader.ReadToEnd();

                                string str = sResponseFromServer;

                            }
                        }
                    }
                }

            }
        }

        internal static void UpdateMessageState(Context context, int messageID, int toUserID, int fromUserID, int dataType)
        {
            try
            {
                string applicationID = ResponseConstant.APPLICATION_ID;

                string senderId = UserUtils.GetUserFcmToken(context, fromUserID);

                string receiverId = UserUtils.GetUserFcmToken(context, toUserID);


                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";

                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = receiverId,
                    data = new
                    {
                        fcmDataType = dataType,
                        messageID = messageID
                    }
                };

                var serializer = new JavaScriptSerializer();

                var json = serializer.Serialize(data);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

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

        public static void EmailVerified(Context context, string email)
        {
            try
            {
                string applicationID = ResponseConstant.APPLICATION_ID;

                string receiverId = UserUtils.GetUserFcmToken(context, UserUtils.GetUserID(context, email));
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                var data = new
                {

                    to = receiverId,

                    data = new
                    {
                        fcmDataType = ResponseConstant.FCM_DATA_TYPE_USER_VERIFIED,
                    }
                };

                var serializer = new JavaScriptSerializer();

                var json = serializer.Serialize(data);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
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