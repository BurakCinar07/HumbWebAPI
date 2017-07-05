using BookieAPI.Controllers.Utils;
using BookieAPI.Filters.ErrorHandlers;
using BookieAPI.Models.Context;
using System.Web.Http;

//UnTouched
namespace BookieAPI.Controllers
{
    public class ConfirmPasswordHashController : ApiController
    {
        Context context = new Context();

        [HttpGet]
        public string ForgotPassword(string email, string token)
        {
            string response = "Invalid Request" ;
            if (string.IsNullOrEmpty(email))
            {
                return response;
            }
            else if (string.IsNullOrEmpty(email))
            {
                return response;
            }
            else
            {
                if (UserUtils.UserExist(context, UserUtils.GetUserID(context, email)))
                {
                    UserUtils.ConfirmPasswordChange(context, email, token);
                    response = "Your password has changed.";
                }                
            }
            return response;
        }
    }
}
