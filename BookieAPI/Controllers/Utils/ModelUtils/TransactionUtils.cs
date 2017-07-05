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
    public static class TransactionUtils
    {
        public static void AddTransaction(Context context, int bookID, int giverUserID, int takerUserID, int transactiontype)
        {
            BookTransaction bookTransaction = new BookTransaction();
            bookTransaction.giverUserID = giverUserID;
            bookTransaction.takerUserID = takerUserID;
            bookTransaction.Book = BookUtils.GetBook(context, bookID);
            bookTransaction.transactionType = transactiontype;
            bookTransaction.createdAt = DateTime.Now.AddMilliseconds(20);
            context.BookTransactions.Add(bookTransaction);

            context.SaveChanges();
        }

        public static List<BookTransactionModel> GetTransactions(Context context, int bookID)
        {
            var bookTransactions = context.BookTransactions.Where(x => x.bookID == bookID).Select(i => new { i.createdAt, i.giverUserID, i.takerUserID, i.transactionType }).ToList();
            List<BookTransactionModel> btms = new List<BookTransactionModel>();
            BookTransactionModel btm;
            foreach (var bi in bookTransactions)
            {
                btm = new BookTransactionModel();
                btm.createdAt = bi.createdAt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                btm.transactionType = bi.transactionType;

                btm.giver = UserUtils.GetUserModel(context, bi.giverUserID);   
                btm.taker = UserUtils.GetUserModel(context, bi.takerUserID);                
                btms.Add(btm);
            }
            return btms;
        }

        public static BookTransaction GetLastTransaction(Context context, int bookID)
        {
            return context.BookTransactions.Where(x => x.bookID == bookID).OrderByDescending(x => x.createdAt).FirstOrDefault();
        }

        public static int GetTransactionCount(Context context, int takerUserID, int bookID, int transactionType)
        {
            return context.BookTransactions.Where(x => x.takerUserID == takerUserID && x.bookID == bookID && x.transactionType == transactionType).Count();
        }
    }
}