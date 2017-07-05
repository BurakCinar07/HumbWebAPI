using BookieAPI.Constants;
using BookieAPI.Controllers.Utils;
using BookieAPI.Controllers.Utils.ModelUtils;
using BookieAPI.Filters.ErrorHandlers;
using BookieAPI.Models.Context;
using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels;
using Newtonsoft.Json.Linq;
using System;
using System.Web.Http;
using System.Web.Http.Results;

namespace BookieAPI.Controllers
{
    public class BookAddInteractionController : ApiController
    {
        Context context = new Context();
        BaseResponse response = new BaseResponse();

        [HttpPost]
        public JsonResult<BaseResponse> AddInteraction([FromBody] JObject post)
        {
            ErrorHandler.WithPost(context, post, "email", "password", "bookID", "interactionType")
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
            string strInteractionType = post["interactionType"].ToString();

            int userID = UserUtils.GetUserID(context, email);
            int bookID = Int32.Parse(strBookID);
            int interactionType = Int32.Parse(strInteractionType);

            if (BookUtils.IsBookOwnerExist(context, bookID, userID))
            {
                Book book = BookUtils.GetBook(context, bookID);
                if (InteractionUtils.CanAddInteraction(context, interactionType, book.bookState))
                {
                    InteractionUtils.AddInteraction(context, book, email, interactionType);
                    response.error = false;
                }
            }
            else
            {
                Book book = BookUtils.GetBook(context, bookID);
                if (InteractionUtils.CanAddInteraction(context, interactionType, book.bookState))
                {
                    int dispatchCount = TransactionUtils.GetTransactionCount(context, userID, bookID, ResponseConstant.TRANSACTION_DISPATCH);
                    int comeToHandCount = TransactionUtils.GetTransactionCount(context, userID, bookID, ResponseConstant.TRANSACTION_COME_TO_HAND);

                    if (dispatchCount <= comeToHandCount)
                    {
                        OnError(this, new ErrorEventArgs(ResponseConstant.ERROR_INVALID_REQUEST));
                    }
                    else
                    {
                        TransactionUtils.AddTransaction(context, bookID, BookUtils.GetBook(context, bookID).ownerID, userID, ResponseConstant.TRANSACTION_COME_TO_HAND);
                        
                        InteractionUtils.AddInteraction(context, book, email, interactionType);

                        FcmUtils.SendRequestNotification(context, bookID, userID, BookUtils.GetBook(context, bookID).ownerID, ResponseConstant.FCM_DATA_TYPE_TRANSACTION_COME_TO_HAND);

                        BookUtils.UpdateBookOwner(context, bookID, userID);
                        response.error = false;
                    }
                }
            }
        }
    }
}
