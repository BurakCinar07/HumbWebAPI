using BookieAPI.Models.ResponseModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels
{
    public class BookCreateResponse : BaseResponse
    {
        public BookModel book { get; set; }
    }
}