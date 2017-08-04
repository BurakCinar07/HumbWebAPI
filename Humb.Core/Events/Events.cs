using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Events
{
    public class EventBase
    {
        public DateTime EventTime { get; protected set; }

        public EventBase()
        {
            this.EventTime = DateTime.Now;
        }
    }

    public abstract class BookEvent : EventBase
    {
        public int BookId { get; private set; }

        protected BookEvent(int bookId)
        {
            this.BookId = bookId;
        }
    }

    public class BookAdded : BookEvent
    {
        public int UserId { get; private set; }
        public int InteractionType { get; private set; }
        public BookAdded(int bookId, int userId, int interactionType) : base(bookId)
        {
            this.UserId = userId;
            this.InteractionType = interactionType;
        }
    }
}
