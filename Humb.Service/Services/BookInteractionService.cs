using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.Entities;
using Humb.Core.Interfaces.ServiceInterfaces;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Humb.Core.Constants;

namespace Humb.Service.Services
{
    public class BookInteractionService : IBookInteractionService
    {
        private readonly IRepository<BookInteraction> _bookInteractionRepository;
        private readonly IRepository<Book> _bookRepository;

        private readonly IRepository<User> _userRepository;
        public BookInteractionService(IRepository<User> userRepository, IRepository<Book> bookRepository, IRepository<BookInteraction> bookInteractionRepo)
        {
            _bookInteractionRepository = bookInteractionRepo;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
        }
        public bool AddInteraction(Book book, string email, int interactionType)
        {
            User user = _userRepository.FindSingleBy(x => x.Email == email);
            BookInteraction bi = new BookInteraction()
            {
                Book = book,
                User = user,
                CreatedAt = DateTime.Now
            };

            if (!IsBookInteractionExist(book.Id))
            {
                bi.InteractionType = ResponseConstant.INTERACTION_ADD;
            }
            else if (book.BookState == ResponseConstant.STATE_READING)
            {
                bi.InteractionType = ResponseConstant.INTERACTION_READ_STOP;
            }
            _bookInteractionRepository.Insert(bi);

            BookInteraction bi2 = new BookInteraction()
            {
                User = user,
                Book = book,
                InteractionType = interactionType,
                CreatedAt = DateTime.Now.AddMilliseconds(10)
            };
            _bookInteractionRepository.Insert(bi2);

            return true;
        }
        public void AddInteraction(BookInteraction bookInteraction)
        {
            _bookInteractionRepository.Insert(bookInteraction);
        }
        public bool CanAddInteraction(int interactionType, int bookState)
        {
            bool canAdd = false;
            //User can't add this interactions. This intractions added automatically.
            if (bookState == ResponseConstant.STATE_LOST || interactionType == ResponseConstant.INTERACTION_READ_STOP || interactionType == ResponseConstant.INTERACTION_ADD)
            {
                //InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_INJECTION, "This case shouldn't occur");
            }
            else if (bookState == ResponseConstant.STATE_OPENED_TO_SHARE && interactionType == ResponseConstant.INTERACTION_OPEN_TO_SHARE)
            {
                //InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_INJECTION, "This case shouldn't occur");
            }
            else if (bookState == ResponseConstant.STATE_CLOSED_TO_SHARE && interactionType == ResponseConstant.INTERACTION_CLOSE_TO_SHARE)
            {
                //InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_INJECTION, "This case shouldn't occur");
            }
            else if (bookState == ResponseConstant.STATE_READING && interactionType == ResponseConstant.INTERACTION_READ_START)
            {
                //InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_INJECTION, "This case shouldn't occur");
            }
            else
            {
                canAdd = true;
            }
            return canAdd;
        }
        public int GetBookInteractionCount(int bookId)
        {
            return _bookInteractionRepository.FindBy(x => x.BookId == bookId).Count();
        }
        public int GetUserInteractionCountWithType(int userId, int interactionType)
        {
            return _bookInteractionRepository.FindBy(x => x.UserId == userId && x.InteractionType == interactionType).Count();
        }

        public IEnumerable<BookInteraction> GetBookInteractions(int bookId)
        {
            return _bookInteractionRepository.FindBy(x => x.BookId == bookId);
        }
        public IEnumerable<Book> GetUserBooksOnHand(int userId)
        {
            return _bookInteractionRepository.FindBy(x => (x.Book.BookState == ResponseConstant.STATE_OPENED_TO_SHARE || x.Book.BookState == ResponseConstant.STATE_CLOSED_TO_SHARE ||
                x.Book.BookState == ResponseConstant.STATE_ON_ROAD || x.Book.BookState == ResponseConstant.STATE_LOST) && x.Book.OwnerId == userId).GroupBy(x => x.BookId).
                Select(x => x.OrderByDescending(j => j.CreatedAt)).Select(x => x.FirstOrDefault()).OrderByDescending(x => x.CreatedAt).Select(x => x.Book);
        }
        public IEnumerable<Book> GetUserReadBooks(int userId)
        {
            return _bookInteractionRepository.FindBy(x => x.InteractionType == ResponseConstant.INTERACTION_READ_STOP && x.UserId == userId).
                GroupBy(x => x.BookId).Select(x => x.OrderByDescending(j => j.CreatedAt)).Select(x => x.FirstOrDefault()).OrderByDescending(x => x.CreatedAt).Select(x => x.Book);
        }
        public int GetUserInteractionCountWithTypeDistinct(int userId, int interactionType)
        {
            return _bookInteractionRepository.FindBy(x => x.UserId == userId && x.InteractionType == interactionType).GroupBy(x => x.BookId).Select(x => x.FirstOrDefault()).Count();
        }
        public bool IsBookInteractionExist(int bookId)
        {
            return _bookInteractionRepository.Any(x => x.BookId == bookId);
        }
        
        
        
    }
}
