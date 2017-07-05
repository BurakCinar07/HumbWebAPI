using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels.Models.BookDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels.Models
{
    public class BookDetailsModel
    {
        public BookDetailsModel()
        {
            this.owner = new UserModel();
            this.addedBy = new UserModel();
        }
        public int ID { get; set; }        
        public string bookName { get; set; }
        public int bookState { get; set; }       
        public string author { get; set; }
        public int genreCode { get; set; }        
        public string bookPictureURL { get; set; }        
        public string bookPictureThumbnailURL { get; set; }
        public string createdAt { get; set; }
        public UserModel owner { get; set; }
        public UserModel addedBy { get; set; }
        public List<BookInteractionModel> bookInteractions { get; set; }
        public List<BookRequestModel> bookRequests { get; set; }
        public List<BookTransactionModel> bookTransactions { get; set; }
    }
}