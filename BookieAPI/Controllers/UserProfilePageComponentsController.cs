using BookieAPI.Constants;
using BookieAPI.Models.Context;
using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels;
using System;
using System.Web.Http;
using System.Web.Http.Results;
using BookieAPI.Filters;
using BookieAPI.Controllers.Utils;
using BookieAPI.Filters.ErrorHandlers;

namespace BookieAPI.Controllers
{
    public class UserProfilePageComponentsController : ApiController
    {
        Context context = new Context();
        ProfilePageResponse response = new ProfilePageResponse();

        [HttpGet]
        [CacheWebApi(Duration = 20)]
        public JsonResult<ProfilePageResponse> UserProfilePage(string email, string password, string userID)
        {
            ErrorHandler.WithGet(context, email, password, userID)
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
            string[] parameters = ((SuccessGetEventArgs)e).parameters;

            string email = parameters[0];
            string password = parameters[1];
            string strUserID = parameters[2];

            int uID = Int32.Parse(strUserID);

            if (UserUtils.GetUserID(context, email) == uID)
            {
                User user = UserUtils.GetUser(context, uID);
                response.userDetails.ID = user.userID;
                response.userDetails.nameSurname = user.nameSurname;
                response.userDetails.profilePictureURL = user.profilePictureURL;
                response.userDetails.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
                response.userDetails.latitude = user.latitude;
                response.userDetails.longitude = user.longitude;
                response.userDetails.password = user.password;
                response.userDetails.email = user.email;
                response.userDetails.emailVerified = user.emailVerified;
                response.userDetails.bio = user.bio;
                response.userDetails.counter = UserUtils.GetUserBookCounter(context, uID);
                response.userDetails.point = UserUtils.GetUserPoint(context, uID);
                response.userDetails.shared = UserUtils.GetUserSharedCount(context, uID);
                response.currentlyReading = UserUtils.GetUserCurrentlyReadingBooks(context, uID);
                response.booksOnHand = UserUtils.GetUserBooksOnHand(context, uID);
                response.readBooks = UserUtils.GetUserReadBooks(context, uID);
                response.onRoadBooks = UserUtils.GetUserOnRoadBooks(context, uID);
                response.error = false;                
            }
            else
            {
                User user = UserUtils.GetUser(context, uID);
                response.userDetails.ID = user.userID;
                response.userDetails.nameSurname = user.nameSurname;
                response.userDetails.profilePictureURL = user.profilePictureURL;
                response.userDetails.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
                response.userDetails.latitude = user.latitude;
                response.userDetails.longitude = user.longitude;
                response.userDetails.password = user.password;
                response.userDetails.email = user.email;
                response.userDetails.emailVerified = user.emailVerified;
                response.userDetails.bio = user.bio;
                response.userDetails.counter = UserUtils.GetUserBookCounter(context, uID);
                response.userDetails.point = UserUtils.GetUserPoint(context, uID);
                response.userDetails.shared = UserUtils.GetUserSharedCount(context, uID);
                response.currentlyReading = UserUtils.GetUserCurrentlyReadingBooks(context, uID);
                response.booksOnHand = UserUtils.GetUserBooksOnHand(context, uID);
                response.readBooks = UserUtils.GetUserReadBooks(context, uID);
                response.error = false;
            }
        }
    }
}
