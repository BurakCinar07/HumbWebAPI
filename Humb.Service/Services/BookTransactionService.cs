using Humb.Core.Entities;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Humb.Core.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.DTOs;

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
            BookTransaction bookTransaction = new BookTransaction();
            bookTransaction.GiverUserId = giverUserId;
            bookTransaction.TakerUserId = takerUserId;
            bookTransaction.Book = book;
            bookTransaction.TransactionType = transactionType;
            bookTransaction.CreatedAt = DateTime.Now.AddMilliseconds(20);
            _bookTransactionRepository.Insert(bookTransaction);
        }

        public BookTransaction GetBookLastTransaction(int bookId)
        {
            throw new NotImplementedException();
        }

        public List<BookTransactionDTO> GetBookTransactions(int bookId)
        {
            throw new NotImplementedException();
        }

        public int GetUserTakenBookTransactionCount(int takerUserId, int bookId, int transactionType)
        {
            throw new NotImplementedException();
        }
    }
}
