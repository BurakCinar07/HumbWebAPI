using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels
{
    public class ProfilePageResponse : BaseResponse
    {        

        public ProfilePageResponse()
        {
            this.userDetails = new UserDetailsModel();
            this.currentlyReading = new List<BookModel>();
            this.booksOnHand = new List<BookModel>();
            this.readBooks = new List<BookModel>();
            this.onRoadBooks = new List<BookModel>();
        }
        public UserDetailsModel userDetails { get; set; }         
        public List<BookModel> currentlyReading { get; set; }
        public List<BookModel> booksOnHand { get; set; }
        public List<BookModel> readBooks { get; set; }
        public List<BookModel> onRoadBooks { get; set; }
    }
}