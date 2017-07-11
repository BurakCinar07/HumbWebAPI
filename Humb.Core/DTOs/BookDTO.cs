using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.DTOs
{
    public class BookDTO
    {
        public BookDTO()
        {
            this.Owner = new UserDTO();
        }
        public int ID { get; set; }
        public string BookName { get; set; }
        public string BookPictureURL { get; set; }
        public string BookPictureThumbnailURL { get; set; }
        public string Author { get; set; }
        public int BookState { get; set; }
        public int GenreCode { get; set; }
        public UserDTO Owner { get; set; }
    }
}
