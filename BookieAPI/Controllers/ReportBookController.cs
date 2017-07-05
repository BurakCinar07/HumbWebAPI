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
    public class ReportBookController : ApiController
    {
        Context context = new Context();
        BaseResponse response = new BaseResponse();

        [HttpPost]
        public JsonResult<BaseResponse> Report([FromBody] JObject post)
        {
            ErrorHandler.WithPost(context, post, "email", "password", "bookID", "reportCode", "reportInfo")
                        .isKeysNull()
                        .isValuesNullOrEmpty("reportInfo")
                        .isEmailValid()
                        .isUserExist()
                        .isUserVerified()
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
            string strReportCode = post["reportCode"].ToString();
            string reportInfo = post["reportInfo"].ToString();

            int userID = UserUtils.GetUserID(context, email);
            int bookID = int.Parse(strBookID);
            int reportCode = int.Parse(strReportCode);

            BookUtils.ReportBook(context, userID, bookID, reportCode, reportInfo);
            response.error = false;
        }
    }
}
