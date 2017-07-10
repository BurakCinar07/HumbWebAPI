using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.DTO.DTOs
{
    public class BookDetailsDTO : IBookDTO
    {
        public BookDetailsDTO()
        {
            this.Owner = new UserDTO();
            this.AddedBy = new UserDTO();
        }
        public int ID { get; set; }
        public string BookName { get; set; }
        public int BookState { get; set; }
        public string Author { get; set; }
        public int GenreCode { get; set; }
        public string BookPictureURL { get; set; }
        public string BookPictureThumbnailURL { get; set; }
        public string CreatedAt { get; set; }
        public UserDTO Owner { get; set; }
        public UserDTO AddedBy { get; set; }
        public IList<BookInteraction> BookInteractions { get; set; }
        public IList<BookRequest> BookRequests { get; set; }
        public IList<BookTransaction> BookTransactions { get; set; }
    }

    public class BookTransaction
    {
        public BookTransaction()
        {
            this.Giver = new UserDTO();
            this.Taker = new UserDTO();
        }
        public int TransactionType { get; set; }
        public UserDTO Giver { get; set; }
        public UserDTO Taker { get; set; }
        public string CreatedAt { get; set; }
    }

    public class BookRequest
    {
        public BookRequest()
        {
            this.Requester = new UserDTO();
            this.Responder = new UserDTO();
        }
        public int RequestType { get; set; }
        public UserDTO Requester { get; set; }
        public UserDTO Responder { get; set; }
        public string CreatedAt { get; set; }
    }

    public class BookInteraction
    {
        public BookInteraction()
        {
            this.User = new UserDTO();
        }
        public int InteractionType { get; set; }
        public UserDTO User { get; set; }
        public string CreatedAt { get; set; }
    }
}
