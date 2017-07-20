using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces
{
    public interface IMessageService
    {
        int CreateMessage(int fromUserId, int toUserId, string messageText);
        void UpdateMessageState(int messageId, string email, int messageType);
        void DeleteMessages(string email, int[] messageIds);
        void DeleteConversation(string email, int toUserId);
        IEnumerable<Message> GetFetchedMessages(string email, int[] userIds);
        int GetFromUserIDByMessageID(int messageID);
    }
}
