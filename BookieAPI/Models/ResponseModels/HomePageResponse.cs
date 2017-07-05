using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels
{
    public class HomePageResponse : BaseResponse
    {
        public HomePageResponse()
        {
            this.headerBooks = new List<BookModel>();
            this.listBooks = new List<BookModel>();
        }
        public List<BookModel> headerBooks { get; set; }
        public List<BookModel> listBooks { get; set; }



    }
}