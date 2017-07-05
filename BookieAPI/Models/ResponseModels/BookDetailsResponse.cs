using BookieAPI.Models.ResponseModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels
{
    public class BookDetailsResponse : BaseResponse
    {
        public BookDetailsResponse()
        {
            this.bookDetails = new BookDetailsModel();
        }
        public BookDetailsModel bookDetails { get; set; }

    }
}