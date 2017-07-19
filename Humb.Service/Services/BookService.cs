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
using System.Configuration;

namespace Humb.Service.Services
{
    public class BookService : IBookService
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IUserService _userService;
        private readonly IBookInteractionService _bookInteractionService;
        public BookService(IRepository<Book> bookRepo, IUserService userService, IBookInteractionService bookInteractionService)
        {
            this._bookRepository = bookRepo;
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
                BookPictureUrl = ConfigurationManager.AppSettings["ImageSaveUrl"] + "BookPictures/" + path,
                BookPictureThumbnailUrl = ConfigurationManager.AppSettings["ImageSaveUrl"] + "BookPicturesThumbnails/" + thumbnailPath,
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
