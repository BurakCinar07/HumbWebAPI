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
        public BookInteractionService(IRepository<BookInteraction> bookInteractionRepo)
        {
            _bookInteractionRepository = bookInteractionRepo;
        }
        public bool AddInteraction(Book book, string email, int interactionType)
        {
            throw new NotImplementedException();
        }

        public bool CanAddInteraction(int interactionType, int bookState)
        {
            throw new NotImplementedException();
        }

        public int GetInteractionCount(int bookID)
        {
            throw new NotImplementedException();
        }
        public int GetBookPopularity(string bookName, DateTime dateTime)
        {
            int popularity = 0;
            popularity += _bookInteractionRepository.FindBy(x => (x.InteractionType == ResponseConstant.INTERACTION_READ_START ||
            x.InteractionType == ResponseConstant.INTERACTION_READ_STOP) && x.Book.BookName == bookName && x.CreatedAt > dateTime).GroupBy(x => x.UserId).Select(x => x.FirstOrDefault()).Count();

            return popularity;
        }

        public IList<BookInteraction> GetInteractions(int bookID)
        {
            throw new NotImplementedException();
        }
    }
}
