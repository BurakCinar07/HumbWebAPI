using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Entities
{

    public partial class Book : BaseEntity
    {
        public Book()
        {
            BookRequests = new HashSet<BookRequest>();
            BookInteractions = new HashSet<BookInteraction>();
            BookTransactions = new HashSet<BookTransaction>();
        }

        public string BookName { get; set; }

        public int BookState { get; set; }

        public string Author { get; set; }

        public int GenreCode { get; set; }

        public string BookPictureUrl { get; set; }
        
        public string BookPictureThumbnailUrl { get; set; }

        public int OwnerId { get; set; }

        public int AddedById { get; set; }

        public virtual ICollection<BookRequest> BookRequests { get; set; }

        public virtual ICollection<BookInteraction> BookInteractions { get; set; }

        public virtual ICollection<BookTransaction> BookTransactions { get; set; }
    }

}
