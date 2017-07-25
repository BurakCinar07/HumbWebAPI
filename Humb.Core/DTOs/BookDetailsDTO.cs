using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.DTOs
{
    public class BookDetailsDTO
    {        
        public int Id { get; set; }
        public string BookName { get; set; }
        public int BookState { get; set; }
        public string Author { get; set; }
        public int GenreCode { get; set; }
        public string BookPictureUrl { get; set; }
        public string BookPictureThumbnailUrl { get; set; }
        public string CreatedAt { get; set; }
        public UserDTO Owner { get; set; }
        public UserDTO AddedBy { get; set; }
        public IList<BookInteractionDTO> BookInteractions { get; set; }
        public IList<BookRequestDTO> BookRequests { get; set; }
        public IList<BookTransactionDTO> BookTransactions { get; set; }
    }

    public class BookTransactionDTO
    {        
        public int TransactionType { get; set; }
        public UserDTO Giver { get; set; }
        public UserDTO Taker { get; set; }
        public string CreatedAt { get; set; }
    }

    public class BookRequestDTO
    {      
        public int RequestType { get; set; }
        public UserDTO Requester { get; set; }
        public UserDTO Responder { get; set; }
        public string CreatedAt { get; set; }
    }

    public class BookInteractionDTO
    {
        public int InteractionType { get; set; }
        public UserDTO User { get; set; }
        public string CreatedAt { get; set; }
    }
}
