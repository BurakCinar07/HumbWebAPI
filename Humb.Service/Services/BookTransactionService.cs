using Humb.Core.Entities;
using Humb.Core.Interfaces.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Service.Services
{
    public static class BookTransactionService
    {
        public static BookTransaction GetLastBookTransaction(IRepository<BookTransaction> bookTransactionRepository, int bookId)
        {
           return bookTransactionRepository.FindBy(x => x.BookId == bookId).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        }
    }
}
