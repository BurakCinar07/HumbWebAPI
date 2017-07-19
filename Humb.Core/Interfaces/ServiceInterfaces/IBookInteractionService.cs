using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces
{
    public interface IBookInteractionService
    {
        bool AddInteraction(Book book, string email, int interactionType);
        IList<BookInteraction> GetInteractions(int bookID);
        int GetInteractionCount(int bookID);
        bool CanAddInteraction(int interactionType, int bookState);
        int GetBookPopularity(string bookName, DateTime dateTime);
    }
}
