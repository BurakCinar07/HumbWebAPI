using BookieAPI.Constants;
using BookieAPI.Models.Context;
using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels.Models.BookDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Controllers.Utils.ModelUtils
{
    public static class RequestUtils
    {
        public static bool CanSendRequest(Context context, int bookID, int requestingUserID, int respondingUserID)
        {
            int bookState = BookUtils.GetBookState(context, bookID);
            if (bookState == ResponseConstant.STATE_OPENED_TO_SHARE || bookState == ResponseConstant.STATE_READING)
            {
                BookTransaction transaction = TransactionUtils.GetLastTransaction(context, bookID);
                if (transaction != null && transaction.transactionType == ResponseConstant.TRANSACTION_COME_TO_HAND)
                {
                    int requestSent = context.BookRequests.Where(x => x.bookID == bookID && x.requestingUserID == requestingUserID && x.respondingUserID == respondingUserID &&
                    x.requestType == ResponseConstant.REQUEST_SENT && x.createdAt > transaction.createdAt).Count();

                    int requestAnswered = context.BookRequests.Where(x => x.bookID == bookID && x.requestingUserID == requestingUserID && x.respondingUserID == respondingUserID &&
                    x.requestType == ResponseConstant.REQUEST_REJECT && x.createdAt > transaction.createdAt).Count();

                    return requestSent <= requestAnswered;
                }
                else
                {
                    if (transaction == null)
                    {
                        Book book = BookUtils.GetBook(context, bookID);
                        if (book.addedByID == respondingUserID && book.ownerID == respondingUserID)
                        {
                            int requestSent = context.BookRequests.Where(x => x.bookID == bookID && x.requestingUserID == requestingUserID && x.respondingUserID == respondingUserID &&
                            x.requestType == ResponseConstant.REQUEST_SENT).Count();

                            int requestAnswered = context.BookRequests.Where(x => x.bookID == bookID && x.requestingUserID == requestingUserID && x.respondingUserID == respondingUserID &&
                            x.requestType == ResponseConstant.REQUEST_REJECT ).Count();

                            return requestSent <= requestAnswered;
                        }
                    }
                    return false;
                }
            }
            return false;

        }

        public static bool CanAnswerRequest(Context context, int bookID, int requestingUserID, int respondingUserID)
        {
            int bookState = BookUtils.GetBookState(context, bookID);
            if (bookState == ResponseConstant.STATE_OPENED_TO_SHARE || bookState == ResponseConstant.STATE_READING)
            {
                BookTransaction transaction = TransactionUtils.GetLastTransaction(context, bookID);
                if (transaction != null && transaction.transactionType == ResponseConstant.TRANSACTION_COME_TO_HAND && transaction.takerUserID == respondingUserID)
                {
                    int requestSent = context.BookRequests.Where(x => x.bookID == bookID && x.respondingUserID == respondingUserID && x.requestingUserID == requestingUserID &&
                    x.requestType == ResponseConstant.REQUEST_SENT && x.createdAt > transaction.createdAt).Count();

                    int requestAnswered = context.BookRequests.Where(x => x.bookID == bookID && x.respondingUserID == respondingUserID && x.requestingUserID == requestingUserID &&
                    (x.requestType == ResponseConstant.REQUEST_REJECT || x.requestType == ResponseConstant.REQUEST_ACCEPT) && x.createdAt > transaction.createdAt).Count();

                    return requestSent > requestAnswered;
                }
                else
                {
                    if (transaction == null)
                    {
                        Book book = BookUtils.GetBook(context, bookID);
                        if (book.addedByID == respondingUserID && book.ownerID == respondingUserID)
                        {
                            int requestSent = context.BookRequests.Where(x => x.bookID == bookID && x.requestingUserID == requestingUserID && x.respondingUserID == respondingUserID &&
                            x.requestType == ResponseConstant.REQUEST_SENT).Count();

                            int requestAnswered = context.BookRequests.Where(x => x.bookID == bookID && x.requestingUserID == requestingUserID && x.respondingUserID == respondingUserID &&
                            x.requestType == ResponseConstant.REQUEST_REJECT).Count();

                            return requestSent > requestAnswered;
                        }
                    }
                    return false;
                }
            }
            return false;
        }

        public static void AddRequest(Context context, int bookID, int requestingUserID, int respondingUserID, int requestType)
        {

            Book book = BookUtils.GetBook(context, bookID);
            if (book.bookState == ResponseConstant.STATE_READING)
            {
                BookInteraction bookInteraction = new BookInteraction();
                bookInteraction.User = UserUtils.GetUser(context, book.ownerID);
                bookInteraction.Book = book;
                bookInteraction.interactionType = ResponseConstant.INTERACTION_READ_STOP;
                bookInteraction.createdAt = DateTime.Now;
                context.BookInteractions.Add(bookInteraction);
                context.SaveChanges();
            }
            BookRequest bookRequest = new BookRequest();
            bookRequest.requestingUserID = requestingUserID;
            bookRequest.respondingUserID = respondingUserID;
            bookRequest.Book = book;
            bookRequest.requestType = requestType;
            bookRequest.createdAt = DateTime.Now.AddMilliseconds(10);
            context.BookRequests.Add(bookRequest);
            context.SaveChanges();

            if (requestType == ResponseConstant.REQUEST_ACCEPT)
            {
                TransactionUtils.AddTransaction(context, bookID, respondingUserID, requestingUserID, ResponseConstant.TRANSACTION_DISPATCH);
                BookUtils.UpdateBookState(context, bookID, ResponseConstant.STATE_ON_ROAD);
            }

        }

        //Kitaba gelen bütün requestleri çeker.
        public static List<BookRequestModel> GetRequests(Context context, int bookID)
        {
            var bookRequests = context.BookRequests.Where(x => x.bookID == bookID).Select(i => new { i.requestType, i.createdAt, i.requestingUserID, i.respondingUserID }).ToList();
            List<BookRequestModel> brms = new List<BookRequestModel>();
            BookRequestModel brm;
            foreach (var br in bookRequests)
            {
                brm = new BookRequestModel();
                brm.createdAt = br.createdAt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                brm.requestType = br.requestType;
                brm.requester = UserUtils.GetUserModel(context, br.requestingUserID);
                brm.responder = UserUtils.GetUserModel(context, br.respondingUserID);
                brms.Add(brm);
            }
            return brms;
        }
        //Verilen user için gelen requestleri çeker.
        public static List<BookRequestModel> GetBookRequests(Context context, int bookID, int userID)
        {
            List<BookRequestModel> returnRequests = new List<BookRequestModel>();
            BookTransaction transaction = TransactionUtils.GetLastTransaction(context, bookID);
            DateTime createdAt;

            BookRequestModel brm;

            if (transaction != null)
            {
                if (transaction.transactionType == ResponseConstant.TRANSACTION_COME_TO_HAND && transaction.takerUserID == userID)
                {
                    createdAt = transaction.createdAt;
                }
                else
                {
                    return returnRequests;
                }
            }
            else
            {
                Book book = BookUtils.GetBook(context, bookID);
                if(book.addedByID == userID && book.ownerID == userID)
                {
                    createdAt = book.createdAt;
                }
                else
                {
                    return returnRequests;
                }
            }

            var bookAnsweredRequests = context.BookRequests.Where(x => x.bookID == bookID && x.respondingUserID == userID &&
                        (x.requestType == ResponseConstant.REQUEST_ACCEPT || x.requestType == ResponseConstant.REQUEST_REJECT) && x.createdAt > createdAt).Select(i => new
                        {
                            i.requestType,
                            i.createdAt,
                            i.requestingUserID,
                            i.respondingUserID
                        }).ToList();

            var answeredRequestingIDs = bookAnsweredRequests.Select(x => x.requestingUserID);

            var bookSentRequests = context.BookRequests.Where(x => x.bookID == bookID && x.respondingUserID == userID && !answeredRequestingIDs.Contains(x.requestingUserID) &&
                x.requestType == ResponseConstant.REQUEST_SENT && x.createdAt > createdAt).Select(i => new
                {
                    i.requestType,
                    i.createdAt,
                    i.requestingUserID,
                    i.respondingUserID
                }).ToList();

            bookSentRequests.AddRange(bookAnsweredRequests);
            foreach (var request in bookSentRequests)
            {
                if (request.requestType == ResponseConstant.REQUEST_SENT && UserUtils.GetUserBookCounter(context, request.requestingUserID) <= 0)
                {
                    //BookCounterı geçersiz olan kullanıcıları listeye eklemez.
                }
                else
                {
                    brm = new BookRequestModel();
                    brm.createdAt = request.createdAt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                    brm.requestType = request.requestType;

                    brm.requester = UserUtils.GetUserModel(context, request.requestingUserID);
                    brm.responder = UserUtils.GetUserModel(context, request.respondingUserID);
                    returnRequests.Add(brm);
                }

            }
            return returnRequests;
        }
    }
}
