using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels.Models
{
    public class BookModel
    {
        public BookModel()
        {
            this.owner = new UserModel();
        }
        public int ID { get; set; }        
        public string bookName { get; set; }
        public string bookPictureURL { get; set; }
        public string bookPictureThumbnailURL { get; set; }
        public string author { get; set; }
        public int bookState { get; set; }
        public int genreCode { get; set; }
        public UserModel owner { get; set; }
    }
}