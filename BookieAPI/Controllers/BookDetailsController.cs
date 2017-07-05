using BookieAPI.Constants;
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
    public class BookDetailsController : ApiController
    {
        Context context = new Context();
        BookDetailsResponse response = new BookDetailsResponse();

        [HttpPost]
        public JsonResult<BookDetailsResponse> BookDetail([FromBody] JObject post)
        {
            ErrorHandler.WithPost(context, post, "email", "password", "bookID")
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
            string strBookID = post["bookID"].ToString();

            int bookID = int.Parse(strBookID);

            response.bookDetails = BookUtils.GetBookDetailsModel(context, bookID);

            response.error = false;
        }
    }
}
