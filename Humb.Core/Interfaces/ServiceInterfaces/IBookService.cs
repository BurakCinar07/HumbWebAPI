using Humb.Core.DTOs;
using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//TO DO : Book utilsde olup burda olmayan metodları search servisine ekle.
namespace Humb.Core.Interfaces.ServiceInterfaces
{
    public interface IBookService
    {
        int CreateBook(string email, string path, string thumbnailPath, string bookName, string author, int bookState, int genreCode);
        Book GetBook(int ID);
        bool BookAddedByUser(int bookID, int userID);
        int GetBookState(int bookID);
        string GetBookPictureUrl(int bookID);
        string GetBookPictureThumbnailURL(int bookID);
        void UpdateBookState(int bookID, int state);
        string[] UpdateBookPicture(string picturePath, string thumbnailPath, int bookID);
        void UpdateBookOwner(int bookID, int userID);
        void UpdateBookDetails(int bookID, string bookName, string author, int genreCode);
        bool DoesBookOwnerExist(int bookID, int userID);
        void ReportBook(int userID, int bookID, int reportCode, string reportInfo);
        IList<BookDTO> GetBookDTOByGenre(int genreCode, string email, bool searchPressed);
        User GetBookOwner(int bookID);
        bool SetBookStateLost(string email, int bookID);
        int GetBookPopularity(string bookName, DateTime dateTime);
        IList<Book> GetBooksByLovedGenres(ICollection<LovedGenre> lovedGenres);

    }
}
