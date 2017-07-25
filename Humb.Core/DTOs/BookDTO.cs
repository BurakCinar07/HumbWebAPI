using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.DTOs
{
    public class BookDTO
    {        
        public int Id { get; set; }
        public string BookName { get; set; }
        public string BookPictureUrl { get; set; }
        public string BookPictureThumbnailUrl { get; set; }
        public string Author { get; set; }
        public int BookState { get; set; }
        public int GenreCode { get; set; }
        public UserDTO Owner { get; set; }
    }
}
