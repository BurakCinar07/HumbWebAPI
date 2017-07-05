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
    public class SearchController : ApiController
    {
        Context context = new Context();
        SearchResponse response = new SearchResponse();

        [HttpPost]
        public JsonResult<SearchResponse> Search([FromBody] JObject post)
        {
            ErrorHandler.WithPost(context, post, "email", "password", "searchString", "searchGenre", "searchPressed")
                        .isKeysNull()
                        .isValuesNullOrEmpty("searchString", "searchGenre")
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
            string searchString = post["searchString"].ToString();
            string strSearchPressed = post["searchPressed"].ToString();
            string strSearchGenre = post["searchGenre"].ToString();

            bool searchPressed = Convert.ToBoolean(strSearchPressed);
            int searchGenre = int.Parse(strSearchGenre);
            if (searchGenre != -1)
            {
                response.listBooks = BookUtils.GetBookModelsByGenre(context, searchGenre, email, searchPressed);
                response.error = false;
            }
            else if(!string.IsNullOrEmpty(searchString))
            {                
                if (!searchPressed)
                {
                    response.listBooks = BookUtils.GetBooksBySearchStringNotPressed(context, searchString, email);
                    response.listUsers = UserUtils.GetUsersBySearchStringNotPressed(context, searchString, email);
                }
                else
                {
                    response.listBooks = BookUtils.GetBooksBySearchString(context, searchString, email);
                    response.listUsers = UserUtils.GetUsersBySearchString(context, searchString, email);
                }
                response.error = false;
            }
            else
            {
                response.error = false;
            }
        }
    }
}
