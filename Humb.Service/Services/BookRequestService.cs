using Humb.Core.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.DTOs;
using Humb.Core.Constants;
using Humb.Core.Entities;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Pelusoft.EasyMapper;

namespace Humb.Service.Services
{
    public class BookRequestService : IBookRequestService
    {
        private readonly IBookService _bookService;
        private readonly IBookTransactionService _bookTransactionService;
        private readonly IRepository<BookRequest> _bookRequestRepository;
        private readonly IBookInteractionService _bookInteractionService;
        private readonly IUserService _userService;
        public BookRequestService(IBookService bookService, IUserService userService, IBookInteractionService bookInteractionService, IBookTransactionService bookTransactionService, IRepository<BookRequest> bookRequestRepository)
        {
            _bookService = bookService;
            _bookTransactionService = bookTransactionService;
            _bookRequestRepository = bookRequestRepository;
            _userService = userService;
            _bookInteractionService = bookInteractionService;
        }
        //TODO : Send Notification
        public void AddRequest(int bookId, int requestingUserId, int respondingUserId, int requestType)
        {   
            Book book = _bookService.GetBook(bookId);
            if (book.BookState == ResponseConstant.STATE_READING)
            {
                BookInteraction bookInteraction = new BookInteraction()
                {
                    User = _userService.GetUser(book.OwnerId),
                    Book = book,
                    InteractionType = ResponseConstant.INTERACTION_READ_STOP,
                    CreatedAt = DateTime.Now
                };
                _bookInteractionService.AddInteraction(bookInteraction);
            }
            BookRequest bookRequest = new BookRequest()
            {
                RequestingUserId = requestingUserId,
                RespondingUserId = respondingUserId,
                Book = book,
                RequestType = requestType,
                CreatedAt = DateTime.Now.AddMilliseconds(10)
            };
            _bookRequestRepository.Insert(bookRequest);

            if (requestType == ResponseConstant.REQUEST_ACCEPT)
            {
                _bookTransactionService.AddTransaction(bookId, respondingUserId, requestingUserId, book, ResponseConstant.TRANSACTION_DISPATCH);
                _bookService.UpdateBookState(bookId, ResponseConstant.STATE_ON_ROAD);
            }
        }

        public bool CanAnswerRequest(int bookId, int requestingUserId, int respondingUserId)
        {
            int bookState = _bookService.GetBookState(bookId);
            if (bookState == ResponseConstant.STATE_OPENED_TO_SHARE || bookState == ResponseConstant.STATE_READING)
            {
                BookTransaction transaction = _bookTransactionService.GetBookLastTransaction(bookId);
                if (transaction != null && transaction.TransactionType == ResponseConstant.TRANSACTION_COME_TO_HAND && transaction.TakerUserId == respondingUserId)
                {
                    int requestSent = _bookRequestRepository.FindBy(x => x.BookId == bookId && x.RespondingUserId == respondingUserId && x.RequestingUserId == requestingUserId &&
                    x.RequestType == ResponseConstant.REQUEST_SENT && x.CreatedAt > transaction.CreatedAt).Count();

                    int requestAnswered = _bookRequestRepository.FindBy(x => x.BookId == bookId && x.RespondingUserId == respondingUserId && x.RequestingUserId == requestingUserId &&
                    (x.RequestType == ResponseConstant.REQUEST_REJECT || x.RequestType == ResponseConstant.REQUEST_ACCEPT) && x.CreatedAt > transaction.CreatedAt).Count();

                    return requestSent > requestAnswered;
                }
                else if (transaction == null)
                {
                    Book book = _bookService.GetBook(bookId);
                    if (book.AddedById == respondingUserId && book.OwnerId == respondingUserId)
                    {
                        int requestSent = _bookRequestRepository.FindBy(x => x.BookId == bookId && x.RequestingUserId == requestingUserId && x.RespondingUserId == respondingUserId &&
                        x.RequestType == ResponseConstant.REQUEST_SENT).Count();

                        int requestAnswered = _bookRequestRepository.FindBy(x => x.BookId == bookId && x.RequestingUserId == requestingUserId && x.RespondingUserId == respondingUserId &&
                        x.RequestType == ResponseConstant.REQUEST_REJECT).Count();

                        return requestSent > requestAnswered;
                    }
                }
            }
            return false;
        }

        public bool CanSendRequest(int bookId, int requestingUserId, int respondingUserId)
        {
            int bookState = _bookService.GetBookState(bookId);
            if (bookState == ResponseConstant.STATE_OPENED_TO_SHARE || bookState == ResponseConstant.STATE_READING)
            {
                BookTransaction transaction = _bookTransactionService.GetBookLastTransaction(bookId);
                if (transaction != null && transaction.TransactionType == ResponseConstant.TRANSACTION_COME_TO_HAND)
                {
                    int requestSent = _bookRequestRepository.FindBy(x => x.BookId == bookId && x.RequestingUserId == requestingUserId && x.RespondingUserId == respondingUserId &&
                    x.RequestType == ResponseConstant.REQUEST_SENT && x.CreatedAt > transaction.CreatedAt).Count();

                    int requestAnswered = _bookRequestRepository.FindBy(x => x.BookId == bookId && x.RequestingUserId == requestingUserId && x.RespondingUserId == respondingUserId &&
                    x.RequestType == ResponseConstant.REQUEST_REJECT && x.CreatedAt > transaction.CreatedAt).Count();

                    return requestSent <= requestAnswered;
                }
                else if (transaction == null)
                {
                    Book book = _bookService.GetBook(bookId);
                    if (book.AddedById == respondingUserId && book.OwnerId == respondingUserId)
                    {
                        int requestSent = _bookRequestRepository.FindBy(x => x.BookId == bookId && x.RequestingUserId == requestingUserId &&
                        x.RespondingUserId == respondingUserId && x.RequestType == ResponseConstant.REQUEST_SENT).Count();

                        int requestAnswered = _bookRequestRepository.FindBy(x => x.BookId == bookId && x.RequestingUserId == requestingUserId &&
                        x.RespondingUserId == respondingUserId && x.RequestType == ResponseConstant.REQUEST_REJECT).Count();

                        return requestSent <= requestAnswered;
                    }
                }
            }
            return false;
        }

        public IEnumerable<BookRequestDTO> GetBookRequests(int bookId)
        {
            List<BookRequestDTO> returnRequests = new List<BookRequestDTO>();
            var bookRequests = _bookRequestRepository.FindBy(x => x.BookId == bookId).Select(i => new { i.RequestType, i.CreatedAt, i.RequestingUserId, i.RespondingUserId }).ToList();
            foreach (var request in bookRequests)
            {
                returnRequests.Add(EasyMapper.Map<BookRequestDTO>(request));
            }
            return returnRequests;
        }

        public IEnumerable<BookRequestDTO> GetUsersBookRequests(int bookId, int userId)
        {
            List<BookRequestDTO> returnRequests = new List<BookRequestDTO>();
            DateTime createdAt;
            BookTransaction transaction = _bookTransactionService.GetBookLastTransaction(bookId);
            #region createdAt Setting
            if (transaction != null)
            {
                if (transaction.TransactionType == ResponseConstant.TRANSACTION_COME_TO_HAND && transaction.TakerUserId == userId)
                {
                    createdAt = transaction.CreatedAt;
                }
                else
                {
                    return returnRequests;
                }
            }
            else
            {
                Book book = _bookService.GetBook(bookId);
                if (book.AddedById == userId && book.OwnerId == userId)
                {
                    createdAt = book.CreatedAt;
                }
                else
                {
                    return returnRequests;
                }
            }
            #endregion

            var bookAnsweredRequests = _bookRequestRepository.FindBy(x => x.BookId == bookId && x.RespondingUserId == userId &&
                        (x.RequestType == ResponseConstant.REQUEST_ACCEPT || x.RequestType == ResponseConstant.REQUEST_REJECT) && x.CreatedAt > createdAt).Select(i => new
                        {
                            i.RequestType,
                            i.CreatedAt,
                            i.RequestingUserId,
                            i.RespondingUserId
                        }).ToList();
            var bookSentRequests = _bookRequestRepository.FindBy(x => x.BookId == bookId && x.RespondingUserId == userId && bookAnsweredRequests.All(y => y.RequestingUserId != x.RequestingUserId) &&
                x.RequestType == ResponseConstant.REQUEST_SENT && x.CreatedAt > createdAt).Select(i => new
                {
                    i.RequestType,
                    i.CreatedAt,
                    i.RequestingUserId,
                    i.RespondingUserId
                }).ToList();
            bookSentRequests.AddRange(bookAnsweredRequests);

            foreach (var request in bookSentRequests)
            {
                if (request.RequestType != ResponseConstant.REQUEST_SENT || _bookTransactionService.GetUserBookCounter(request.RequestingUserId) > 0)
                {
                    returnRequests.Add(EasyMapper.Map<BookRequestDTO>(request));
                }                
            }
            return returnRequests;
        }
    }
}
