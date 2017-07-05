using BookieAPI.Models.ResponseModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels
{
    public class SearchResponse : BaseResponse
    {
        public SearchResponse()
        {
            this.listBooks = new List<BookModel>();
            this.listUsers = new List<UserModel>();
        }
        public List<BookModel> listBooks { get; set; }
        public List<UserModel> listUsers { get; set; }
    }
}