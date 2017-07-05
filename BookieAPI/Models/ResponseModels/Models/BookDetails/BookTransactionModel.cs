using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels.Models.BookDetails
{
    public class BookTransactionModel
    {
        public BookTransactionModel()
        {
            this.giver = new UserModel();
            this.taker = new UserModel();            
        }
        public int transactionType { get; set; }
        public UserModel giver { get; set; }
        public UserModel taker { get; set; }
        public string createdAt { get; set; }
    }
}