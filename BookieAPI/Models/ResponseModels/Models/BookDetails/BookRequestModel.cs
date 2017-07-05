using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels.Models.BookDetails
{
    public class BookRequestModel
    {
        public BookRequestModel()
        {
            this.requester = new UserModel();
            this.responder= new UserModel();
        }
        public int requestType { get; set; }
        public UserModel requester { get; set; }
        public UserModel responder { get; set; }
        public string createdAt { get; set; }
    }
}