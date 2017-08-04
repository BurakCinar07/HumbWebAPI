using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.Events;

namespace Humb.Core.Interfaces.ServiceInterfaces
{
    public interface IBookInteractionService
    {
        bool AddInteraction(Book book, string email, int interactionType);
        void AddInteraction(BookInteraction bookInteraction);
        IEnumerable<BookInteraction> GetBookInteractions(int bookId);
        int GetBookInteractionCount(int bookId);
        int GetUserInteractionCountWithType(int userId, int interactionType);
        int GetUserInteractionCountWithTypeDistinct(int userId, int interactionType);
        bool CanAddInteraction(int interactionType, int bookState);
        IEnumerable<Book> GetUserBooksOnHand(int userId);
        IEnumerable<Book> GetUserReadBooks(int userId);
        bool IsBookInteractionExist(int bookId);
        void BookAddedListener(BookAdded args);
    }
}
