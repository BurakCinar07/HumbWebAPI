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
        private readonly IBookService _bookService;
        private readonly IBookTransactionService _bookTransactionService;
        private readonly IRepository<User> _userRepository;
        public BookInteractionService(IBookService bookService, IBookTransactionService bookTransactionService, IRepository<User> userRepository, IRepository<Book> bookRepository, IRepository<BookInteraction> bookInteractionRepo)
        {
            _bookService = bookService;
            _bookInteractionRepository = bookInteractionRepo;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _bookTransactionService = bookTransactionService;
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
        public int GetBookPopularity(string bookName, DateTime dateTime)
        {
            int popularity = 0;
            popularity += _bookInteractionRepository.FindBy(x => (x.InteractionType == ResponseConstant.INTERACTION_READ_START ||
            x.InteractionType == ResponseConstant.INTERACTION_READ_STOP) && x.Book.BookName == bookName && x.CreatedAt > dateTime).GroupBy(x => x.UserId).Select(x => x.FirstOrDefault()).Count();

            return popularity;
        }
        public IEnumerable<Book> GetPopularBooks(User user)
        {
            List<Book> returnBooks = new List<Book>();
            Dictionary<string, int> bookPopularities = new Dictionary<string, int>();
            int days = -14;
            DateTime dateTime = DateTime.Now.AddDays(days);
            var bookInteractions = _bookInteractionRepository.FindBy(x => x.CreatedAt > dateTime).GroupBy(x => x.Book.BookName).
                    Select(y => y.FirstOrDefault()).Select(i => new { i.Book.BookName });

            //Son 2 hafta içinde 5 tane kitap bulamazsa 2 hafta daha geriden bakar
            while (bookInteractions.Count() < ResponseConstant.POPULAR_BOOKS_COUNT && days > -42)
            {
                days -= 14;
                dateTime = dateTime.AddDays(days);
                bookInteractions = _bookInteractionRepository.FindBy(x => x.CreatedAt > dateTime && x.UserId != user.Id).GroupBy(x => x.Book.BookName).
                    Select(y => y.FirstOrDefault()).Select(i => new { i.Book.BookName });
            }

            //Kitapları isimlerine göre gruplar populerliklerine göre sıralar ilk 5 i döndürür.
            foreach (var interaction in bookInteractions)
            {
                bookPopularities.Add(interaction.BookName, GetBookPopularity(interaction.BookName, dateTime));
            }
            var sortedBookPopularities = bookPopularities.OrderByDescending(x => x.Value).Take(5);
            foreach (var entry in sortedBookPopularities)
            {
                returnBooks.Add(_bookService.GetRandomBookByBookName(entry.Key));
            }
            return returnBooks;
        }
        public int GetUserProfilePoint(int userId)
        {
            int point = 0;
            point += GetUserInteractionCountWithType(userId, ResponseConstant.INTERACTION_ADD) * 2;
            point += GetUserInteractionCountWithTypeDistinct(userId, ResponseConstant.INTERACTION_READ_STOP);
            point += _bookTransactionService.GetGiverUserTransactionCount(userId, ResponseConstant.TRANSACTION_COME_TO_HAND) * 5;
            point += _bookTransactionService.GetTakerUserTransactionCount(userId, ResponseConstant.TRANSACTION_COME_TO_HAND) * 3;

            point -= _bookTransactionService.GetGiverUserTransactionCount(userId, ResponseConstant.TRANSACTION_LOST) * 10;
            point -= _bookTransactionService.GetTakerUserTransactionCount(userId, ResponseConstant.TRANSACTION_LOST) * 10;
            return point;
        }
    }
}
