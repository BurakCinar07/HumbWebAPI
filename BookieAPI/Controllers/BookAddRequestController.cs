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
    public class BookAddRequestController : ApiController
    {
        Context context = new Context();
        BaseResponse response = new BaseResponse();

        [HttpPost]
        public JsonResult<BaseResponse> AddRequest([FromBody] JObject post)
        {
            ErrorHandler.WithPost(context, post, "email", "password", "bookID", "requesterID", "responderID", "requestType")
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
            string strResquestingUserID = post["requesterID"].ToString();
            string strRespondingUserID = post["responderID"].ToString();
            string strRequestType = post["requestType"].ToString();

            int bookID = int.Parse(strBookID);
            int requestingUserID = int.Parse(strResquestingUserID);
            int respondingUserID = int.Parse(strRespondingUserID);
            int requestType = int.Parse(strRequestType);

            if ((requestType == ResponseConstant.REQUEST_SENT && UserUtils.GetUserID(context, email) != requestingUserID) ||
                ((requestType == ResponseConstant.REQUEST_ACCEPT || requestType == ResponseConstant.REQUEST_REJECT) && UserUtils.GetUserID(context, email) != respondingUserID))
            {
                OnError(this, new ErrorEventArgs(InfiltratorConstant.ERROR_INJECTION));
            }
            else if (UserUtils.IsUserBlocked(context, requestingUserID, respondingUserID))
            {
                OnError(this, new ErrorEventArgs(ResponseConstant.ERROR_USER_BLOCKED));
            }
            else if (!BookUtils.IsBookOwnerExist(context, bookID, respondingUserID))
            {
                OnError(this, new ErrorEventArgs(InfiltratorConstant.ERROR_INJECTION));
            }
            else
            {
                switch (requestType)
                {
                    case ResponseConstant.REQUEST_SENT:
                        {
                            if (!(UserUtils.GetUserBookCounter(context, requestingUserID) > 0))
                            {
                                OnError(this, new ErrorEventArgs(ResponseConstant.ERROR_BOOK_COUNT_INSUFFICIENT));
                            }
                            else if (!RequestUtils.CanSendRequest(context, bookID, requestingUserID, respondingUserID))
                            {
                                OnError(this, new ErrorEventArgs(InfiltratorConstant.ERROR_INJECTION));
                            }
                            else
                            {
                                RequestUtils.AddRequest(context, bookID, requestingUserID, respondingUserID, requestType);
                                FcmUtils.SendRequestNotification(context, bookID, requestingUserID, respondingUserID, ResponseConstant.FCM_DATA_TYPE_REQUEST_SENT);
                                response.error = false;
                            }
                        }
                        break;
                    case ResponseConstant.REQUEST_REJECT:
                        {
                            if (!RequestUtils.CanAnswerRequest(context, bookID, requestingUserID, respondingUserID))
                            {
                                OnError(this, new ErrorEventArgs(InfiltratorConstant.ERROR_INJECTION));
                            }
                            else
                            {
                                RequestUtils.AddRequest(context, bookID, requestingUserID, respondingUserID, requestType);
                                FcmUtils.SendRequestNotification(context, bookID, respondingUserID, requestingUserID, ResponseConstant.FCM_DATA_TYPE_REQUEST_REJECTED);
                                response.error = false;
                            }
                        }
                        break;
                    case ResponseConstant.REQUEST_ACCEPT:
                        {
                            if (!(UserUtils.GetUserBookCounter(context, requestingUserID) > 0))
                            {
                                OnError(this, new ErrorEventArgs(InfiltratorConstant.ERROR_INJECTION));
                            }
                            else if (!RequestUtils.CanAnswerRequest(context, bookID, requestingUserID, respondingUserID))
                            {
                                OnError(this, new ErrorEventArgs(InfiltratorConstant.ERROR_INJECTION));
                            }
                            else
                            {
                                RequestUtils.AddRequest(context, bookID, requestingUserID, respondingUserID, requestType);                                
                                FcmUtils.SendRequestNotification(context, bookID, respondingUserID, requestingUserID, ResponseConstant.FCM_DATA_TYPE_REQUEST_ACCEPTED);
                                response.error = false;
                            }
                        }
                        break;
                    default:
                        OnError(this, new ErrorEventArgs(ResponseConstant.ERROR_INVALID_REQUEST));
                        break;
                }
            }
        }
    }
}
