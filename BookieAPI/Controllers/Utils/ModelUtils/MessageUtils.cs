using BookieAPI.Constants;
using BookieAPI.Models.Context;
using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Controllers.Utils.ModelUtils
{
    public static class MessageUtils
    {
        internal static void DeleteConversation(Context context, string email, int toUserID)
        {
            int fromUserID = UserUtils.GetUserID(context, email);
            IList<Message> messages = context.Messages.Where(x => (x.fromUserID == fromUserID && x.toUserID == toUserID) || (x.fromUserID == toUserID && x.toUserID == fromUserID)).ToList();
            foreach (var m in messages)
            {
                if (m.fromUserID == fromUserID)
                {
                    m.fromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_DELETED;
                }
                else if (m.toUserID == fromUserID)
                {
                    m.toUserMessageState = ResponseConstant.MESSAGE_TO_USER_STATE_DELETED;
                }
                context.SaveChanges();
            }
        }

        internal static void DeleteMessages(Context context, string email, int[] messageIDs)
        {
            int userID = UserUtils.GetUserID(context, email);
            Message m;
            foreach (var id in messageIDs)
            {
                m = new Message();
                m = context.Messages.FirstOrDefault(x => x.messageID == id);
                if (m.fromUserID == userID)
                {
                    if (m.fromUserMessageState != ResponseConstant.MESSAGE_FROM_USER_STATE_DELETED)
                    {
                        m.fromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_DELETED;
                    }
                }
                else if (m.toUserID == userID)
                {
                    if (m.toUserMessageState != ResponseConstant.MESSAGE_TO_USER_STATE_DELETED)
                    {
                        m.toUserMessageState = ResponseConstant.MESSAGE_TO_USER_STATE_DELETED;
                    }
                }
                context.SaveChanges();
            }

        }

        internal static List<FetchMessageModel> GetFetchedMessages(Context context, string email, int[] userIDs)
        {
            List<FetchMessageModel> fm = new List<FetchMessageModel>();
            FetchMessageModel fmm = new FetchMessageModel();
            int userID = UserUtils.GetUserID(context, email);
            var messages = context.Messages.Where(x => (x.fromUserID == userID && x.fromUserMessageState != ResponseConstant.MESSAGE_FROM_USER_STATE_DELETED) || (x.toUserID == userID && x.toUserMessageState != ResponseConstant.MESSAGE_TO_USER_STATE_DELETED)).ToList();
            //Usera mesaj ulaşmamışsa
            foreach (var id in userIDs)
            {
                var m = messages.Where(x => (x.fromUserID == userID && x.toUserID == id) || (x.toUserID == userID && x.fromUserID == id)).ToList();
                foreach (var mm in m)
                {
                    if ((mm.fromUserID == userID && mm.toUserID == id && mm.fromUserMessageState == ResponseConstant.MESSAGE_FROM_USER_STATE_SENT) || (mm.fromUserID == id && mm.toUserID == userID && mm.toUserMessageState == ResponseConstant.MESSAGE_TO_USER_STATE_NONE))
                    {
                        fmm = new FetchMessageModel();
                        fmm.createdAt = mm.createdAt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                        fmm.fromUser = UserUtils.GetUserModel(context, mm.fromUserID);
                        fmm.messageID = mm.messageID;
                        fmm.messageText = mm.messageText;
                        fmm.toUser = UserUtils.GetUserModel(context, mm.toUserID);
                        fmm.messageState = ResponseConstant.MESSAGE_TYPE_SENT;
                        fm.Add(fmm);
                    }
                    messages.Remove(mm);
                }
            }
            foreach (var mes in messages)
            {
                fmm = new FetchMessageModel();
                fmm.createdAt = mes.createdAt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                fmm.fromUser = UserUtils.GetUserModel(context, mes.fromUserID);
                fmm.messageID = mes.messageID;
                fmm.messageText = mes.messageText;
                fmm.toUser = UserUtils.GetUserModel(context, mes.toUserID);
                switch (mes.fromUserMessageState)
                {
                    case ResponseConstant.MESSAGE_FROM_USER_STATE_SENT:
                        fmm.messageState = ResponseConstant.MESSAGE_TYPE_SENT;
                        break;

                    case ResponseConstant.MESSAGE_FROM_USER_STATE_DELIVERED:
                        fmm.messageState = ResponseConstant.MESSAGE_TYPE_DELIVERED;
                        break;

                    case ResponseConstant.MESSAGE_FROM_USER_STATE_SEEN:
                        fmm.messageState = ResponseConstant.MESSAGE_TYPE_SEEN;
                        break;

                    default:
                        fmm.messageState = ResponseConstant.MESSAGE_TYPE_SENT;
                        break;
                }
                fm.Add(fmm);
            }
            return fm;
        }


        internal static int AddMessage(Context context, int fromUserID, int toUserID, string message)
        {
            Message m = new Message();
            m.createdAt = DateTime.Now;
            m.fromUserID = fromUserID;
            m.toUserID = toUserID;
            m.fromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_SENT;
            m.toUserMessageState = ResponseConstant.MESSAGE_TO_USER_STATE_NONE;
            m.messageText = message;
            context.Messages.Add(m);
            context.SaveChanges();
            return m.messageID;
        }

        internal static int GetFromUserIDByMessageID(Context context, int messageID)
        {
            return context.Messages.Where(x => x.messageID == messageID).FirstOrDefault().fromUserID;
        }

        internal static void UpdateMessageState(Context context, int messageID, string email, int messageType)
        {
            if (messageType == ResponseConstant.MESSAGE_TYPE_DELIVERED)
            {
                Message m = new Message();
                m = context.Messages.FirstOrDefault(x => x.messageID == messageID);
                int userID = UserUtils.GetUserID(context, email);
                if (m.fromUserID == userID || m.toUserID == userID)
                {
                    m.toUserMessageState = ResponseConstant.MESSAGE_TO_USER_STATE_RECIEVED;
                    m.fromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_DELIVERED;
                    context.SaveChanges();
                }
            }
            else if (messageType == ResponseConstant.MESSAGE_TYPE_SEEN)
            {
                Message m = new Message();
                m = context.Messages.FirstOrDefault(x => x.messageID == messageID);
                int userID = UserUtils.GetUserID(context, email);
                if (m.fromUserID == userID || m.toUserID == userID)
                {
                    m.fromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_SEEN;
                    context.SaveChanges();
                }
            }
        }
    }
}