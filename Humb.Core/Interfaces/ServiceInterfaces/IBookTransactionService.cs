using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.Entities;
using Humb.Core.DTOs;

namespace Humb.Core.Interfaces.ServiceInterfaces
{
    public interface IBookTransactionService
    {
        void AddTransaction(int bookID, int giverUserId, int takerUserId, Book book, int transactionType);
        List<BookTransactionDTO> GetBookTransactions(int bookId);
        BookTransaction GetBookLastTransaction(int bookId);
        int GetUserTakenBookTransactionCount(int takerUserId, int bookId, int transactionType);
    }
}
