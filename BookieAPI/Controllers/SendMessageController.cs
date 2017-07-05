using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;
using BookieAPI.Constants;
using BookieAPI.Models.ResponseModels.Models;
using System.Web.Http.Results;
using BookieAPI.Models.ResponseModels;
using BookieAPI.Models.Context;
using BookieAPI.Controllers.Utils.ModelUtils;
using BookieAPI.Controllers.Utils;
using BookieAPI.Filters.ErrorHandlers;

namespace BookieAPI.Controllers
{
    //base response döndür
    public class SendMessageController : ApiController
    {
        Context context = new Context();
        MessageSenderResponse response = new MessageSenderResponse();

        [HttpPost]
        public JsonResult<MessageSenderResponse> SendMessage([FromBody] JObject post)
        {
            ErrorHandler.WithPost(context, post, "email", "password", "message", "toUserID", "oldMessageID")
                        .isKeysNull()
                        .isValuesNullOrEmpty()
                        .isEmailValid()
                        .isUserExist()
                        .isUserVerified()
                        .addOnErrorListener(new EventHandler(OnError))
                        .addOnSuccessListener(new EventHandler(OnSuccess))
                        .Check();

            return Json(response);
        }

        private void OnError(Object s, EventArgs e)
        {
            Filters.ErrorHandlers.ErrorEventArgs error = (Filters.ErrorHandlers.ErrorEventArgs)e;
            if (error.errorCode != ResponseConstant.ERROR_NONE)
            {
                response.errorCode = error.errorCode;
            }
            else
            {
                response.errorCode = ResponseConstant.ERROR_UNKNOWN;
            }
        }

        private void OnSuccess(Object s, EventArgs e)
        {
            JObject post = ((SuccessPostEventArgs)e).postObject;

            string email = post["email"].ToString();
            string password = post["password"].ToString();
            string message = post["message"].ToString();
            string strToUserID = post["toUserID"].ToString();
            string oldMessageID = post["oldMessageID"].ToString();

            int toUserID = int.Parse(strToUserID);
            int fromUserID = UserUtils.GetUserID(context, email);

            try
            {
                string applicationID = ResponseConstant.APPLICATION_ID;

                string senderId = UserUtils.GetUserFcmToken(context, fromUserID);

                string receiverId = UserUtils.GetUserFcmToken(context, toUserID);


                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";

                tRequest.ContentType = "application/json";
                UserModel um = new UserModel();
                um = UserUtils.GetUserModel(context, fromUserID);
                response.newMessageID = MessageUtils.AddMessage(context, fromUserID, toUserID, message);
                response.oldMessageID = int.Parse(oldMessageID);
                var data = new
                {

                    to = receiverId,

                    data = new
                    {
                        fcmDataType = ResponseConstant.FCM_DATA_TYPE_SENT_MESSAGE,
                        message = message,
                        messageID = response.newMessageID,
                        sender = um
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

                response.error = false;
            }

            catch (Exception ex)
            {
                OnError(this, new Filters.ErrorHandlers.ErrorEventArgs(ResponseConstant.ERROR_UNKNOWN));
            }
        }
    }
}