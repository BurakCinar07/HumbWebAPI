using BookieAPI.Constants;
using BookieAPI.Controllers.Utils;
using BookieAPI.Controllers.Utils.ModelUtils;
using BookieAPI.Filters.ErrorHandlers;
using BookieAPI.Models.Context;
using BookieAPI.Models.ResponseModels;
using Newtonsoft.Json.Linq;
using System;
using System.Web.Http;
using System.Web.Http.Results;

namespace BookieAPI.Controllers
{
    public class UpdateMessageStateController : ApiController
    {
        Context context = new Context();
        BaseResponse response = new BaseResponse();

        [HttpPost]
        public JsonResult<BaseResponse> SendMessage([FromBody] JObject post)
        {
            ErrorHandler.WithPost(context, post, "email", "password", "messageID", "messageState")
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
            string messageID = post["messageID"].ToString();
            string messageState = post["messageState"].ToString();

            int fromUserID = UserUtils.GetUserID(context, email);
            int mState = int.Parse(messageState);
            int mID = int.Parse(messageID);
            int toUserID = MessageUtils.GetFromUserIDByMessageID(context, mID);

            switch (mState)
            {
                case ResponseConstant.MESSAGE_TYPE_DELIVERED:
                    FcmUtils.UpdateMessageState(context, mID, toUserID, fromUserID, ResponseConstant.FCM_DATA_TYPE_DELIVERED_MESSAGE);
                    MessageUtils.UpdateMessageState(context, mID, email, ResponseConstant.MESSAGE_TYPE_DELIVERED);

                    response.error = false;
                    break;

                case ResponseConstant.MESSAGE_TYPE_SEEN:
                    FcmUtils.UpdateMessageState(context, mID, toUserID, fromUserID, ResponseConstant.FCM_DATA_TYPE_SEEN_MESSAGE);
                    MessageUtils.UpdateMessageState(context, mID, email, ResponseConstant.MESSAGE_TYPE_SEEN);

                    response.error = false;
                    break;
            }
        }
    }
}
