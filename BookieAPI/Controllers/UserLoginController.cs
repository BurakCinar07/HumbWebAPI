using BookieAPI.Constants;
using BookieAPI.Models.Context;
using BookieAPI.Models.ResponseModels;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json.Linq;
using BookieAPI.Controllers.Utils;
using BookieAPI.Filters.ErrorHandlers;
using System;

namespace BookieAPI.Controllers
{
    public class UserLoginController : ApiController
    {
        Context context = new Context();
        LoginResponse response = new LoginResponse();

        [HttpPost]
        public JsonResult<LoginResponse> Login([FromBody] JObject post)
        {
            ErrorHandler.WithPost(context, post, "email", "password")
                        .isKeysNull()
                        .isValuesNullOrEmpty()
                        .isEmailValid()
                        .addOnErrorListener(new EventHandler(OnError))
                        .addOnSuccessListener(new EventHandler(OnSuccess))
                        .Check();

            return Json(response);
        }

        private void OnError(Object s,EventArgs e)
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
            
            password = TextUtils.SanitizeInput(password);
            password = TextUtils.CalculateMD5Hash(password);

            //Check database for user existance
            if (context.Users.Any(x => x.email == email && x.password == password))
            {
                response.userLoginModel = UserUtils.GetUserLoginModel(context, email);
                response.lovedGenres = LovedGenreUtils.GetGenreCodes(context, email);

                response.error = false;
            }
            else
            {
                OnError(this, new ErrorEventArgs(ResponseConstant.ERROR_FALSE_COMBINATION));
            }
        }
    }
}
