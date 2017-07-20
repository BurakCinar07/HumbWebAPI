using Humb.Core.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.Entities;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Humb.Core.Constants;

namespace Humb.Service.Services
{
    public class MessageService //: IMessageService
    {
        //private readonly IRepository<Message> _messageRepository;
        //private readonly IMessageSender _messageSender;
        //private readonly IUserService _userService;
        //public MessageService(IRepository<Message> messageRepo, IMessageSender messageSender, IUserService userService)
        //{
        //    _messageRepository = messageRepo;
        //    _messageSender = messageSender;
        //    _userService = userService;
        //}
        //public int CreateMessage(int fromUserId, int toUserId, string messageText)
        //{
        //    Message message = new Message()
        //    {
        //        FromUserId = fromUserId,
        //        ToUserId = toUserId,
        //        MessageText = messageText,
        //        FromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_SENT,
        //        ToUserMessageState = ResponseConstant.MESSAGE_TO_USER_STATE_NONE,
        //        CreatedAt = DateTime.Now
        //    };
        //    _messageRepository.Insert(message);
        //    _messageSender.SendMessage(message.Id, _userService.GetUser(fromUserId), _userService.GetUser(toUserId), messageText);
        //    return message.Id;
        //}

        //public void DeleteConversation(string email, int toUserId)
        //{
        //    throw new NotImplementedException();
        //}

        //public void DeleteMessages(string email, int[] messageIds)
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerable<Message> GetFetchedMessages(string email, int[] userIds)
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetFromUserIDByMessageID(int messageId)
        //{
        //    return _messageRepository.FindSingleBy(x => x.Id == messageId).FromUserId;
        //}

        //public void UpdateMessageState(int messageId, string email, int messageType)
        //{
        //    Message m = _messageRepository.FindSingleBy(x => x.Id == messageId);
        //    int userId = _userService.GetUserId(email);
        //    if (!(m.FromUserId == userId) && !(m.ToUserId == userId))
        //        return;
            
        //    if (messageType == ResponseConstant.MESSAGE_TYPE_DELIVERED)
        //    {
        //        m.ToUserMessageState = ResponseConstant.MESSAGE_TO_USER_STATE_RECIEVED;
        //        m.FromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_DELIVERED;
        //        _messageRepository.Save();
        //    }
        //    else if (messageType == ResponseConstant.MESSAGE_TYPE_SEEN)
        //    {
        //        m.FromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_SEEN;
        //        _messageRepository.Save();
        //    }
        //}
    }
}
