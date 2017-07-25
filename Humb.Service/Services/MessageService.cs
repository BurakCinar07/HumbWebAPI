using Humb.Core.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.Entities;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Humb.Core.Constants;
using Humb.Core.Interfaces.ServiceInterfaces.InformClient;
using Humb.Core.DTOs;
using Humb.Service.Helpers;

namespace Humb.Service.Services
{
    public class MessageService : IMessageService
    {
        private readonly IRepository<Message> _messageRepository;
        private readonly IInformClientService _informClientService;
        private readonly IUserService _userService;
        public MessageService(IRepository<Message> messageRepo, IInformClientService informClientService, IUserService userService)
        {
            _messageRepository = messageRepo;
            _informClientService = informClientService;
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
            _informClientService.InformClient(InformClientEnums.SendMessageRequest, _userService.GetFcmToken(toUserId), _userService.GetUserDTO(fromUserId), messageText, message.Id, ResponseConstant.FCM_DATA_TYPE_SENT_MESSAGE);
            return message.Id;
        }

        public void DeleteConversation(string email, int toUserId)
        {
            int fromUserId = _userService.GetUserId(email);
            IList<Message> messages = _messageRepository.FindBy(x => (x.FromUserId == fromUserId && x.ToUserId == toUserId) || (x.FromUserId == toUserId && x.ToUserId == fromUserId)).ToList();
            foreach (var m in messages)
            {
                if (m.FromUserId == fromUserId)
                {
                    m.FromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_DELETED;
                }
                else if (m.ToUserId == fromUserId)
                {
                    m.ToUserMessageState = ResponseConstant.MESSAGE_TO_USER_STATE_DELETED;
                }
                _messageRepository.Update(m, m.Id);
            }
        }

        public void DeleteMessages(string email, int[] messageIds)
        {
            int userId = _userService.GetUserId(email);
            Message m;
            foreach (var id in messageIds)
            {
                m = _messageRepository.FindSingleBy(x => x.Id== id);
                if (m.FromUserId == userId)
                {
                    if (m.FromUserMessageState != ResponseConstant.MESSAGE_FROM_USER_STATE_DELETED)
                    {
                        m.FromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_DELETED;
                    }
                }
                else if (m.ToUserId == userId)
                {
                    if (m.ToUserMessageState != ResponseConstant.MESSAGE_TO_USER_STATE_DELETED)
                    {
                        m.ToUserMessageState = ResponseConstant.MESSAGE_TO_USER_STATE_DELETED;
                    }
                }
                _messageRepository.Update(m, m.Id);
            }
        }

        public IEnumerable<MessageDTO> GetFetchedMessages(string email, int[] userIds)
        {
            List<MessageDTO> returnMessages = new List<MessageDTO>();
            int userId = _userService.GetUserId(email);
            var userConversationsMessages = _messageRepository.FindBy(x => (x.FromUserId == userId && x.FromUserMessageState != ResponseConstant.MESSAGE_FROM_USER_STATE_DELETED) ||
            (x.ToUserId == userId && x.ToUserMessageState != ResponseConstant.MESSAGE_TO_USER_STATE_DELETED)).ToList();
            foreach(var id in userIds)
            {
                var currentConversation = _messageRepository.FindBy(x => (x.FromUserId == userId && x.ToUserId == id) || (x.ToUserId == userId && x.FromUserId == id));
                foreach (var message in currentConversation)
                {
                    if ((message.FromUserId == userId && message.ToUserId == id && message.FromUserMessageState == ResponseConstant.MESSAGE_FROM_USER_STATE_SENT) || 
                        (message.FromUserId == id && message.ToUserId == userId && message.ToUserMessageState == ResponseConstant.MESSAGE_TO_USER_STATE_NONE))
                    {
                        MessageDTO mDTO = new MessageDTO();
                        mDTO.CreatedAt = message.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                        mDTO.FromUser = _userService.GetUserDTO(message.FromUserId);
                        mDTO.Id = message.Id;
                        mDTO.MessageText = message.MessageText;
                        mDTO.ToUser = _userService.GetUserDTO(message.ToUserId);
                        mDTO.MessageState = ResponseConstant.MESSAGE_TYPE_SENT;
                        returnMessages.Add(mDTO);
                    }
                    userConversationsMessages.Remove(message);
                }                
            }
            foreach (var message in userConversationsMessages)
            {
                MessageDTO mDTO = new MessageDTO();
                mDTO.CreatedAt = message.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                mDTO.FromUser = _userService.GetUserDTO(message.FromUserId);
                mDTO.Id = message.Id;
                mDTO.MessageText = message.MessageText;
                mDTO.ToUser = _userService.GetUserDTO(message.ToUserId);
                mDTO.MessageState = TypeConverter.FromUserMessageStateToMessageType(message.FromUserMessageState);
                returnMessages.Add(mDTO);
            }
            return returnMessages;
        }

        public int GetFromUserIDByMessageID(int messageId)
        {
            return _messageRepository.FindSingleBy(x => x.Id == messageId).FromUserId;
        }

        public void UpdateMessageState(int messageId, string email, int messageType)
        {
            Message m = _messageRepository.FindSingleBy(x => x.Id == messageId);
            int userId = _userService.GetUserId(email);
            if (!(m.FromUserId == userId) && !(m.ToUserId == userId))
                return;

            if (messageType == ResponseConstant.MESSAGE_TYPE_DELIVERED)
            {
                m.ToUserMessageState = ResponseConstant.MESSAGE_TO_USER_STATE_RECIEVED;
                m.FromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_DELIVERED;
                _messageRepository.Save();
                _informClientService.InformClient(InformClientEnums.UpdateMessageStateRequest, _userService.GetFcmToken(m.ToUserId), m.Id, ResponseConstant.FCM_DATA_TYPE_DELIVERED_MESSAGE);
            }
            else if (messageType == ResponseConstant.MESSAGE_TYPE_SEEN)
            {
                m.FromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_SEEN;
                _messageRepository.Save();
                _informClientService.InformClient(InformClientEnums.UpdateMessageStateRequest, _userService.GetFcmToken(m.ToUserId), m.Id, ResponseConstant.FCM_DATA_TYPE_SEEN_MESSAGE);
            }
        }
    }
}
