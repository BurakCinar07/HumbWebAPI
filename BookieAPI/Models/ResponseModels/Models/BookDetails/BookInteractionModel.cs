using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels.Models.BookDetails
{
    public class BookInteractionModel
    {
        public BookInteractionModel()
        {
            this.user = new UserModel();
        }
        public int interactionType { get; set; }
        public UserModel user { get; set; }
        public string createdAt { get; set; }
    }
}