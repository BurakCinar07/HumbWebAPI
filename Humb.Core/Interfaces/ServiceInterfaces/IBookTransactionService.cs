﻿using System.Collections.Generic;
using Humb.Core.Entities;

namespace Humb.Core.Interfaces.ServiceInterfaces
{
    public interface IBookTransactionService
    {
        void AddTransaction(int bookID, int giverUserId, int takerUserId, Book book, int transactionType);
        IEnumerable<BookTransaction> GetBookTransactions(int bookId);
        BookTransaction GetBookLastTransaction(int bookId);
        BookTransaction GetBookLastTransactionWithGiverUserId(int bookId, int userId);
        int GetUserTakenBookTransactionCount(int takerUserId, int bookId, int transactionType);
        int GetTakerUserTransactionCount(int takerUserId, int transactionType);
        int GetGiverUserTransactionCount(int giverUserId, int transactionType);
        void UpdateBookTransaction(BookTransaction bookTransaction);
        IEnumerable<Book> GetUserOnRoadBooks(int userId);
    }
}
