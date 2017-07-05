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
    public class UpdateBookDetailsController : ApiController
    {
        Context context = new Context();
        BaseResponse response = new BaseResponse();

        [HttpPost]
        public JsonResult<BaseResponse> Update([FromBody] JObject post)
        {
            ErrorHandler.WithPost(context, post, "email", "password", "bookID", "bookName", "author", "genreCode")
                       .isKeysNull()
                       .isValuesNullOrEmpty()
                       .isEmailValid()
                       .isUserExist()
                       .isUserVerified()
                       .isUserLocationExist()
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
            string author = post["author"].ToString();
            string bookName = post["bookName"].ToString();
            string strGenreCode = post["genreCode"].ToString();

            int bookID = int.Parse(strBookID);
            int genreCode = int.Parse(strGenreCode);
            int userID = UserUtils.GetUserID(context, email);
            if (BookUtils.IsBookOwnerExist(context, bookID, userID) && BookUtils.IsUserBooksCreater(context, bookID, userID))
            {
                BookUtils.UpdateBookDetails(context, bookID, bookName, author, genreCode);
                response.error = false;
            }
        }
    }
}
