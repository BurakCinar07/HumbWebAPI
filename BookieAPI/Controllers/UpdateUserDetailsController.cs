using BookieAPI.Constants;
using BookieAPI.Controllers.Utils;
using BookieAPI.Filters.ErrorHandlers;
using BookieAPI.Models.Context;
using BookieAPI.Models.ResponseModels;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;

namespace BookieAPI.Controllers
{
    public class UpdateUserDetailsController : ApiController
    {
        Context context = new Context();
        BaseResponse response = new BaseResponse();

        [HttpPost]
        public JsonResult<BaseResponse> UpdateUser([FromBody] JObject post)
        {
            ErrorHandler.WithPost(context, post, "email", "password", "name", "bio")
                        .isKeysNull()
                        .isValuesNullOrEmpty("bio")
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
            string name = post["name"].ToString();
            string bio = post["bio"].ToString();
           

            
            UserUtils.UpdateUserName(context, email, name);
            if (!string.IsNullOrEmpty(bio))
            {
                UserUtils.UpdateUserBio(context, email, bio);
            }
            else
            {
                UserUtils.UpdateUserBio(context, email, "");
            }

            response.error = false;
        }
    }
}
