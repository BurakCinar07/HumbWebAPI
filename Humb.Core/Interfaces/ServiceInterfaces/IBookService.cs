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
        int CreateBook(int userId, string path, string thumbnailPath, string bookName, string author, int bookState, int genreCode);
        Book GetBook(int Id);
        bool IsBookAddedByUser(int bookId, int userId);
        int GetBookState(int bookId);
        string GetBookPictureUrl(int bookId);
        string GetBookPictureThumbnailURL(int bookId);
        Book GetRandomBookByBookName(string bookName);
        void UpdateBookState(int bookId, int state);
        string[] UpdateBookPicture(int bookId, string picturePath, string thumbnailPath);
        void UpdateBookOwner(int bookId, int userId);
        void UpdateBookDetails(int bookId, string bookName, string author, int genreCode);
        bool IsUserBookOwner(int bookId, int userId);
        void ReportBook(int userId, int bookId, int reportCode, string reportInfo);
        IList<BookDTO> GetBookDTOByGenre(int genreCode, string email, bool searchPressed);
        int GetBookOwnerId(int bookId);
        bool SetBookStateLost(string email, int bookId);
        IEnumerable<Book> GetBooksByLovedGenres(ICollection<LovedGenre> lovedGenres);
        IEnumerable<Book> GetUserCurrentlyReadingBooks(int userId);
        IEnumerable<Book> GetFirstViewedBookList(User user);
        IEnumerable<Book> GetScrolledBookList(User user, int[] bookIds);
    }
}
