using BookieAPI.Models.ResponseModels.Models.BookDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels
{
    public class BookRequestResponse : BaseResponse
    {

        public BookRequestResponse()
        {
            this.bookRequests = new List<BookRequestModel>();
        }
        public List<BookRequestModel> bookRequests { get; set; }
    }
}