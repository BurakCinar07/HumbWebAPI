using Humb.Core.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.DTOs;
using Humb.Core.Entities;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Humb.Core.Constants;

namespace Humb.Service.Services
{
    public class BookService : IBookService
    {
        private readonly IRepository<Book> _bookRepository;
        public BookService(IRepository<Book> bookRepo)
        {
            this._bookRepository = bookRepo;
        }
        public bool BookAddedByUser(int bookID, int userID)
        {
            throw new NotImplementedException();
        }

        public int CreateBook(string email, string path, string thumbnailPath, string bookName, string author, int bookState, int genreCode)
        {
            throw new NotImplementedException();
        }

        public bool DoesBookOwnerExist(int bookID, int userID)
        {
            throw new NotImplementedException();
        }

        public Book GetBook(int ID)
        {
            throw new NotImplementedException();
        }

        public IList<BookDTO> GetBookDTOByGenre(int genreCode, string email, bool searchPressed)
        {
            throw new NotImplementedException();
        }

        public User GetBookOwner(int bookID)
        {
            throw new NotImplementedException();
        }

        public string GetBookPictureThumbnailURL(int bookID)
        {
            throw new NotImplementedException();
        }

        public string GetBookPictureUrl(int bookID)
        {
            throw new NotImplementedException();
        }

        public int GetBookPopularity(string bookName, DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public IList<Book> GetBooksByLovedGenres(ICollection<LovedGenre> lovedGenres)
        {
            throw new NotImplementedException();
        }

        public int GetBookState(int bookID)
        {
            throw new NotImplementedException();
        }

        public void ReportBook(int userID, int bookID, int reportCode, string reportInfo)
        {
            throw new NotImplementedException();
        }

        public bool SetBookStateLost(string email, int bookID)
        {
            throw new NotImplementedException();
        }

        public void UpdateBookDetails(int bookID, string bookName, string author, int genreCode)
        {
            throw new NotImplementedException();
        }

        public void UpdateBookOwner(int bookID, int userID)
        {
            throw new NotImplementedException();
        }

        public string[] UpdateBookPicture(string picturePath, string thumbnailPath, int bookID)
        {
            throw new NotImplementedException();
        }

        public void UpdateBookState(int bookID, int state)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Book> GetUserCurrentlyReadingBooks(int userId)
        {
            return _bookRepository.FindBy(x => x.BookState == ResponseConstant.STATE_READING && x.OwnerId == userId);
        }
    }
}
