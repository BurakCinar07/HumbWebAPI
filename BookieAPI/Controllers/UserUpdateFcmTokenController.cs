using BookieAPI.Constants;
using BookieAPI.Filters.ErrorHandlers;
using BookieAPI.Models.Context;
using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;

namespace BookieAPI.Controllers
{
    public class UserUpdateFcmTokenController : ApiController
    {
        Context context = new Context();
        BaseResponse response = new BaseResponse();

        [HttpPost]
        public JsonResult<BaseResponse> UpdateUserFcmToken([FromBody] JObject post)
        {
            ErrorHandler.WithPost(context, post, "email", "password", "token")
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
            string token = post["token"].ToString();

            User updatedUser = context.Users.Where(x => x.email == email && x.password == password).FirstOrDefault();
            updatedUser.fcmToken = token;
            context.SaveChanges();

            response.error = false;
        }
    }
}
