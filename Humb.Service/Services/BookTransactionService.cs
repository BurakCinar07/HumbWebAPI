using Humb.Core.Entities;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Humb.Core.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.DTOs;
using Pelusoft.EasyMapper;
using Humb.Core.Constants;

namespace Humb.Service.Services
{
    public class BookTransactionService : IBookTransactionService
    {
        private readonly IRepository<BookTransaction> _bookTransactionRepository;
        public BookTransactionService(IRepository<BookTransaction> bookTransactionRepo)
        {
            this._bookTransactionRepository = bookTransactionRepo;
        }

        public void AddTransaction(int bookID, int giverUserId, int takerUserId, Book book, int transactionType)
        {
            BookTransaction bookTransaction = new BookTransaction()
            {
                GiverUserId = giverUserId,
                TakerUserId = takerUserId,
                Book = book,
                TransactionType = transactionType,
                CreatedAt = DateTime.Now.AddMilliseconds(20)
            };
            _bookTransactionRepository.Insert(bookTransaction);
        }

        public BookTransaction GetBookLastTransaction(int bookId)
        {
            return _bookTransactionRepository.FindBy(x => x.BookId == bookId).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        }

        public IEnumerable<BookTransaction> GetBookTransactions(int bookId)
        {
            return _bookTransactionRepository.FindBy(x => x.BookId == bookId);
        }        
        public int GetUserTakenBookTransactionCount(int takerUserId, int bookId, int transactionType)
        {
            return _bookTransactionRepository.FindBy(x => x.TakerUserId == takerUserId && x.BookId == bookId && x.TransactionType == transactionType).Count();
        }
        public int GetTakerUserTransactionCount(int takerUserId, int transactionType)
        {
            return _bookTransactionRepository.FindBy(x => x.TransactionType == transactionType && x.TakerUserId == takerUserId).Count();
        }
        public int GetGiverUserTransactionCount(int giverUserId, int transactionType)
        {
            return _bookTransactionRepository.FindBy(x => x.TransactionType == transactionType && x.GiverUserId == giverUserId).Count();
        }
        public IEnumerable<Book> GetUserOnRoadBooks(int userId)
        {
            return _bookTransactionRepository.FindBy(x => x.TakerUserId == userId && x.TransactionType == ResponseConstant.TRANSACTION_DISPATCH &&
                x.Book.BookTransactions.OrderByDescending(y=>y.CreatedAt).FirstOrDefault().GiverUserId == x.Book.OwnerId && 
                x.Book.OwnerId != userId && x.Book.BookState == ResponseConstant.STATE_ON_ROAD).
                OrderByDescending(x=>x.CreatedAt).GroupBy(x=>x.BookId).Select(x=>x.FirstOrDefault()).OrderByDescending(x=>x.CreatedAt).Select(x=>x.Book);
        }

        public BookTransaction GetBookLastTransactionWithGiverUserId(int bookId, int userId)
        {
            return _bookTransactionRepository.FindBy(x => x.BookId == bookId && x.GiverUserId == userId).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        }
        public BookTransaction GetBookTransaction(int transactionId)
        {
            return _bookTransactionRepository.FindSingleBy(x => x.Id == transactionId);
        }
        public void UpdateBookTransaction(BookTransaction transaction)
        {
            _bookTransactionRepository.Update(transaction, transaction.Id);
        }
        public int GetUserBookCounter(int userId)
        {
            int counter = 100;
            counter += GetGiverUserTransactionCount(userId, ResponseConstant.TRANSACTION_DISPATCH);
            counter -= GetTakerUserTransactionCount(userId, ResponseConstant.TRANSACTION_DISPATCH);

            //Dispatch olduğunda puan direk artırıldığı için kitabı kaybeden giverdan önce verilen puan alınır ardından bir puan daha düşürülür, takerdan puan dispatchte düşürüldüğü için puanına dokunulmaz.
            counter -= 2 * GetGiverUserTransactionCount(userId, ResponseConstant.TRANSACTION_LOST);
            return counter;
        }
        public int GetUserGivenBookCount(int userId)
        {
            return GetGiverUserTransactionCount(userId, ResponseConstant.TRANSACTION_COME_TO_HAND);
        }

    }
}
