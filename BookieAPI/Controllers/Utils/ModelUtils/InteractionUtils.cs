using BookieAPI.Constants;
using BookieAPI.Models.Context;
using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels.Models.BookDetails;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookieAPI.Controllers.Utils.ModelUtils
{
    public static class InteractionUtils
    {
        public static bool AddInteraction(Context context, Book book, string email, int interactionType)
        {
            BookInteraction bookInteraction = new BookInteraction();

            //if book has no interactions add Constant.INTERACTION_ADD before given interaction type
            if (GetInteractionCount(context, book.bookID) == 0)
            {

                bookInteraction = new BookInteraction();
                bookInteraction.User = UserUtils.GetUser(context, email);
                bookInteraction.Book = book;
                bookInteraction.interactionType = ResponseConstant.INTERACTION_ADD;
                bookInteraction.createdAt = DateTime.Now;
                context.BookInteractions.Add(bookInteraction);

                context.SaveChanges();
            }
            //if book is now reading add Constant.INTERACTION_READ_STOP before given interaction type
            else if (book.bookState == ResponseConstant.STATE_READING)
            {

                bookInteraction = new BookInteraction();
                bookInteraction.User = UserUtils.GetUser(context, email);
                bookInteraction.Book = book;
                bookInteraction.interactionType = ResponseConstant.INTERACTION_READ_STOP;
                bookInteraction.createdAt = DateTime.Now;
                context.BookInteractions.Add(bookInteraction);

                context.SaveChanges();
            }

            bookInteraction = new BookInteraction();
            bookInteraction.User = UserUtils.GetUser(context, email);
            bookInteraction.Book = book;
            bookInteraction.interactionType = interactionType;
            bookInteraction.createdAt = DateTime.Now.AddMilliseconds(10);
            context.BookInteractions.Add(bookInteraction);

            context.SaveChanges();

            //Change book state from interaction type
            if (interactionType == ResponseConstant.INTERACTION_READ_START)
            {
                book.bookState = ResponseConstant.STATE_READING;
            }
            else if (interactionType == ResponseConstant.INTERACTION_OPEN_TO_SHARE)
            {
                book.bookState = ResponseConstant.STATE_OPENED_TO_SHARE;
            }
            else if (interactionType == ResponseConstant.INTERACTION_CLOSE_TO_SHARE)
            {
                book.bookState = ResponseConstant.STATE_CLOSED_TO_SHARE;
            }

            context.SaveChanges();

            return true;

        }

        public static List<BookInteractionModel> GetInteractions(Context context, int bookID)
        {
            var bookInteractions = context.BookInteractions.Where(x => x.bookID == bookID).Select(i => new { i.createdAt, i.userID, i.interactionType }).ToList();
            List<BookInteractionModel> bims = new List<BookInteractionModel>();
            BookInteractionModel bim;
            foreach (var bi in bookInteractions)
            {
                bim = new BookInteractionModel();
                bim.createdAt = bi.createdAt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                bim.interactionType = bi.interactionType;
                bim.user = UserUtils.GetUserModel(context, bi.userID);
                bims.Add(bim);
            }
            return bims;
        }

        public static int GetInteractionCount(Context context, int bookID)
        {
            return context.BookInteractions.Where(x => x.bookID == bookID).Count();
        }

        public static bool CanAddInteraction(Context context, int interactionType, int bookState)
        {
            bool isValid = false;
            //User can't add this interactions. This intractions added automatically.
            if (bookState == ResponseConstant.STATE_LOST || interactionType == ResponseConstant.INTERACTION_READ_STOP || interactionType == ResponseConstant.INTERACTION_ADD)
            {
                InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_INJECTION, "This case shouldn't occur");
            }
            else if (bookState == ResponseConstant.STATE_OPENED_TO_SHARE && interactionType == ResponseConstant.INTERACTION_OPEN_TO_SHARE)
            {
                InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_INJECTION, "This case shouldn't occur");
            }
            else if (bookState == ResponseConstant.STATE_CLOSED_TO_SHARE && interactionType == ResponseConstant.INTERACTION_CLOSE_TO_SHARE)
            {
                InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_INJECTION, "This case shouldn't occur");
            }
            else if (bookState == ResponseConstant.STATE_READING && interactionType == ResponseConstant.INTERACTION_READ_START)
            {
                InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_INJECTION, "This case shouldn't occur");
            }
            else
            {
                isValid = true;
            }
            return isValid;
        }
    }
}