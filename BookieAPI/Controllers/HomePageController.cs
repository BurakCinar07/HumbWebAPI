using BookieAPI.Constants;
using BookieAPI.Filters;
using BookieAPI.Filters.ErrorHandlers;
using BookieAPI.Models.Context;
using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels;
using BookieAPI.Models.ResponseModels.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using WebApi.OutputCache.V2;
using BookieAPI.Controllers.Utils;
//UnTouched
//List booklar ve gelen null değilse list bookta göstermicen
namespace BookieAPI.Controllers
{

    public class HomePageController : ApiController
    {
        const int HEADER_BOOK_COUNT = 5;
        const int TIME_OUT_SECS = 15;
        Context context = new Context();
        HomePageResponse response = new HomePageResponse();

        [HttpGet]
        [CacheWebApi(Duration = 20)]
        public JsonResult<HomePageResponse> HomePage(string email, string password, string bookIDs)
        {
            ErrorHandler.WithGet(context, email, password, bookIDs).isValuesNullOrEmpty()
                        .isEmailValid()
                        .isUserExist().
                        addOnErrorListener(new EventHandler(OnError))
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
            string[] paramaters = ((SuccessGetEventArgs)e).parameters;
            string email = paramaters[0];
            string password = paramaters[1];
            string bookIDs = paramaters[2];

            if (bookIDs == "-1")
            {
                response.headerBooks = HomePageUtils.GetHeaderBooks(context, email);
                response.listBooks = HomePageUtils.GetListBooks(context, email);
                response.error = false;
            }
            else
            {
                int[] bookIDss = bookIDs.Split('_').Select(n => Convert.ToInt32(n)).ToArray();
                response.listBooks = HomePageUtils.GetExcludedListBooks(context, email, bookIDss);
                response.error = false;
            }
        }
    }
}
