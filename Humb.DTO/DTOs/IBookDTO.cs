using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.DTO.DTOs
{
    interface IBookDTO
    {
        string Author { get; set; }
        string BookName { get; set; }
        string BookPictureThumbnailURL { get; set; }
        string BookPictureURL { get; set; }
        int BookState { get; set; }
        int GenreCode { get; set; }
        UserDTO Owner { get; set; }
    }
}
