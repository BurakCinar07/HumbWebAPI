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
        private const string imageURL = "http://82.165.97.141:4000/Images/";

        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<ReportBook> _reportedBookRepository;
        private readonly IUserService _userService;
        private readonly IBookInteractionService _bookInteractionService;
        public BookService(IRepository<Book> bookRepo, IRepository<ReportBook> reportedBookRepo, IUserService userService, IBookInteractionService bookInteractionService)
        {
            this._bookRepository = bookRepo;
            this._reportedBookRepository = reportedBookRepo;
            this._userService = userService;
            this._bookInteractionService = bookInteractionService;
        }
        public bool IsBookAddedByUser(int bookID, int userID)
        {
            return _bookRepository.Any(x => x.Id == bookID && x.AddedById == userID);
        }

        public int CreateBook(string email, string path, string thumbnailPath, string bookName, string author, int bookState, int genreCode)
        {
            int userId = _userService.GetUserId(email);
            Book book = new Book()
            {
                BookName = bookName.Replace('_', ' '),
                Author = author.Replace('_', ' '),
                OwnerId = userId,
                AddedById = userId,
                CreatedAt = DateTime.Now,
                GenreCode = genreCode,
                BookState = bookState,
                BookPictureUrl = imageURL + "BookPictures/" + path,
                BookPictureThumbnailUrl = imageURL + "BookPicturesThumbnails/" + thumbnailPath,
            };
            _bookRepository.Insert(book);
            int interactionType;
            switch (bookState)
            {
                case ResponseConstant.STATE_OPENED_TO_SHARE:
                    interactionType = ResponseConstant.INTERACTION_OPEN_TO_SHARE;
                    break;
                case ResponseConstant.STATE_READING:
                    interactionType = ResponseConstant.INTERACTION_READ_START;
                    break;
                default:
                    interactionType = ResponseConstant.INTERACTION_CLOSE_TO_SHARE;
                    break;
            }
            _bookInteractionService.AddInteraction(book, email, interactionType);
            return book.Id;
        }

        public bool IsUserBookOwner(int bookId, int userId)
        {
            return _bookRepository.Any(x => x.Id == bookId && x.OwnerId == userId);
        }

        public Book GetBook(int bookId)
        {
            return _bookRepository.FindSingleBy(x => x.Id == bookId);
        }

        public IList<BookDTO> GetBookDTOByGenre(int genreCode, string email, bool searchPressed)
        {
            throw new NotImplementedException();
        }

        public int GetBookOwnerId(int bookId)
        {
            return _bookRepository.FindSingleBy(x => x.Id == bookId).OwnerId;
        }

        public string GetBookPictureThumbnailURL(int bookId)
        {
            return _bookRepository.FindSingleBy(x => x.Id == bookId).BookPictureThumbnailUrl;
        }

        public string GetBookPictureUrl(int bookId)
        {
            return _bookRepository.FindSingleBy(x => x.Id == bookId).BookPictureUrl;
        }

        

        public IEnumerable<Book> GetBooksByLovedGenres(ICollection<LovedGenre> lovedGenres)
        {
            throw new NotImplementedException();
        }

        public int GetBookState(int bookID)
        {
            return _bookRepository.FindSingleBy(x => x.Id == bookID).BookState;
        }

        public void ReportBook(int userId, int bookId, int reportCode, string reportInfo)
        {
            ReportBook rb = new ReportBook()
            {
                BookId = bookId,
                UserId = userId,
                ReportCode = reportCode,
                ReportInfo = reportInfo,
                CreatedAt = DateTime.Now
            };
            _reportedBookRepository.Insert(rb);
        }

        public bool SetBookStateLost(string email, int bookID)
        {
            throw new NotImplementedException();
        }

        public void UpdateBookDetails(int bookId, string bookName, string author, int genreCode)
        {
            Book book = GetBook(bookId);
            book.BookName = bookName;
            book.Author = author;
            book.GenreCode = genreCode;
            _bookRepository.Update(book, bookId);
        }

        public void UpdateBookOwner(int bookId, int userId)
        {
            Book book = GetBook(bookId);
            book.OwnerId = userId;
            _bookRepository.Update(book, bookId);
        }

        public string[] UpdateBookPicture(int bookId, string picturePath, string thumbnailPath)
        {
            Book book = GetBook(bookId);
            book.BookPictureUrl = imageURL + "BookPictures/" + picturePath;
            book.BookPictureThumbnailUrl = imageURL + "BookPicturesThumbnails/" + thumbnailPath;
            _bookRepository.Update(book, bookId);
            return new string[] { book.BookPictureUrl, book.BookPictureThumbnailUrl };
        }

        public void UpdateBookState(int bookId, int bookState)
        {
            Book book = GetBook(bookId);
            book.BookState = bookState;
            _bookRepository.Update(book, bookId);
        }
        public IEnumerable<Book> GetUserCurrentlyReadingBooks(int userId)
        {
            return _bookRepository.FindBy(x => x.BookState == ResponseConstant.STATE_READING && x.OwnerId == userId);
        }
    }
}
