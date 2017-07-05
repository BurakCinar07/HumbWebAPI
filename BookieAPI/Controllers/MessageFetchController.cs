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
    public class MessageFetchController : ApiController
    {
        Context context = new Context();
        FetchMessageResponse response = new FetchMessageResponse();

        [HttpPost]
        public JsonResult<FetchMessageResponse> FetchDelete([FromBody] JObject post)
        {
            ErrorHandler.WithPost(context, post, "email", "password", "userIDs")
                        .isKeysNull()
                        .isValuesNullOrEmpty("userIDs")
                        .isEmailValid()
                        .isUserExist()
                        .addOnErrorListener(new EventHandler(OnError))
                        .addOnSuccessListener(new EventHandler(OnSuccess))
                        .Check();

            return Json(response);
        }

        private void OnError(Object s, EventArgs e)
        {
            ErrorEventArgs error = (ErrorEventArgs)e;
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
            string strUserIDs = post["userIDs"].ToString();

            int[] userIDs = TextUtils.ConvertStringToIntArray(strUserIDs);

            response.Messages = MessageUtils.GetFetchedMessages(context, email, userIDs);
            response.error = false;
        }
    }
}
