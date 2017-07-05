using BookieAPI.Constants;
using BookieAPI.Controllers.Utils;
using BookieAPI.Filters.ErrorHandlers;
using BookieAPI.Models.Context;
using BookieAPI.Models.ResponseModels;
using Newtonsoft.Json.Linq;
using System;
using System.Web.Http;
using System.Web.Http.Results;

namespace BookieAPI.Controllers
{
    public class ResendEmailVerificationController : ApiController
    {
        Context context = new Context();
        BaseResponse response = new BaseResponse();

        [HttpPost]
        public JsonResult<BaseResponse> Resend([FromBody] JObject post)
        {
            ErrorHandler.WithPost(context, post, "email", "password")
                         .isKeysNull()
                         .isValuesNullOrEmpty()
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

            if (UserUtils.IsUserVerified(context, email))
            {
                OnError(this, new ErrorEventArgs(ResponseConstant.ERROR_USER_ALREADY_VERIFIED));
            }
            else
            {
                try
                {
                    UserUtils.ResendEmailVerification(context, email);
                    response.error = false;
                }
                catch (Exception ex)
                {
                    OnError(this, new ErrorEventArgs(ResponseConstant.ERROR_UNKNOWN));
                }
            }
        }
    }
}
