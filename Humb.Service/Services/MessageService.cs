using Humb.Core.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.Entities;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Humb.Core.Interfaces.ServiceInterfaces.MessageInterfaces;
using Humb.Core.Constants;

namespace Humb.Service.Services
{
    public class MessageService : IMessageService
    {
        private readonly IRepository<Message> _messageRepository;
        private readonly IMessageSender _messageSender;
        private readonly IUserService _userService;
        public MessageService(IRepository<Message> messageRepo, IMessageSender messageSender, IUserService userService)
        {
            _messageRepository = messageRepo;
            _messageSender = messageSender;
            _userService = userService;
        }
        public int CreateMessage(int fromUserId, int toUserId, string messageText)
        {
            Message message = new Message()
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                MessageText = messageText,
                FromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_SENT,
                ToUserMessageState = ResponseConstant.MESSAGE_TO_USER_STATE_NONE,
                CreatedAt = DateTime.Now
            };
            _messageRepository.Insert(message);
            _messageSender.SendMessage(_userService.GetUser(fromUserId), _userService.GetUser(toUserId), message.Id, messageText);
            return message.Id;
        }

        public void DeleteConversation(string email, int toUserId)
        {
            throw new NotImplementedException();
        }

        public void DeleteMessages(string email, int[] messageIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Message> GetFetchedMessages(string email, int[] userIds)
        {
            throw new NotImplementedException();
        }

        public int GetFromUserIDByMessageID(int messageID)
        {
            throw new NotImplementedException();
        }

        public void UpdateMessageState(int messageId, string email, int messageType)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Core.Entities.Message> IMessageService.GetFetchedMessages(string email, int[] userIds)
        {
            throw new NotImplementedException();
        }
    }
}
