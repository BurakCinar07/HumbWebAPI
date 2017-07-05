using BookieAPI.Constants;
using BookieAPI.Controllers.Utils;
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
    public class UserRegisterController : ApiController
    {
        Context context = new Context();
        RegisterResponse response = new RegisterResponse();
        
        [HttpPost]
        public JsonResult<RegisterResponse> Register([FromBody] JObject post)
        {
            ErrorHandler.WithPost(context, post, "email", "password", "nameSurname")
                        .isKeysNull()
                        .isValuesNullOrEmpty()
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
            string nameSurname = post["nameSurname"].ToString();

            if (password.Length < ResponseConstant.PASSWORD_LENGTH_MIN)
            {
                OnError(this, new ErrorEventArgs(ResponseConstant.ERROR_SHORT_PASSWORD));
            }
            else if (password.Length > ResponseConstant.PASSWORD_LENGTH_MAX)
            {
                OnError(this, new ErrorEventArgs(ResponseConstant.ERROR_LONG_PASSWORD));
            }
            else if (!TextUtils.IsEmailValid(email))
            {
                OnError(this, new ErrorEventArgs(ResponseConstant.ERROR_INVALID_EMAIL));
            }
            else if (!TextUtils.IsNameSurnameValid(nameSurname))
            {
                OnError(this, new ErrorEventArgs(ResponseConstant.ERROR_INVALID_NAME_SURNAME));
            }
            else if (context.Users.Any(x => x.email == email))
            {
                OnError(this, new ErrorEventArgs(ResponseConstant.ERROR_EMAIL_TAKEN));
            }
            else
            {
                password = TextUtils.CalculateMD5Hash(password);
                User user = new User();
                user.email = email;
                user.nameSurname = nameSurname;
                user.password = password;
                user.createdAt = DateTime.Now;
                user.emailVerified = false;
                user.verificationHash = TextUtils.CalculateMD5Hash(new Random().Next(0, 1000).ToString());

                try
                {
                    UserUtils.SendEmail(context, email, nameSurname, user.verificationHash);
                }
                catch (Exception ex)
                {
                    OnError(this, new ErrorEventArgs(ResponseConstant.ERROR_UNKNOWN));
                }

                context.Users.Add(user);
                context.SaveChanges();
                
                response.userLoginModel = UserUtils.GetUserLoginModel(context, email);

                response.error = false;
            }
        }
    }
}
