using SchedulerTasks.Models.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchedulerTasks.Constant;
using SchedulerTasks.Models;

namespace SchedulerTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            CheckBookTransactions();
        }
        private static void CheckBookTransactions()
        {
            try
            {
                Context context = new Context();
                DateTime dateTime = DateTime.Now.AddDays(-14);
                DateTime aDayEarlier = dateTime.AddDays(-1);
                var bookTransactions = context.BookTransactions.Where(x => x.createdAt < dateTime && x.createdAt > aDayEarlier && x.transactionType == Constants.TRANSACTION_DISPATCH);
                foreach(var transaction in bookTransactions)
                {
                    var lastTransaction = context.BookTransactions.Where(x => x.bookID == transaction.bookID && x.giverUserID == transaction.giverUserID && x.takerUserID == transaction.takerUserID &&
                    x.transactionType != Constants.TRANSACTION_DISPATCH && x.createdAt > dateTime);
                    if(lastTransaction == null)                  
                    {
                        Book book = context.Books.FirstOrDefault(x => x.bookID == transaction.bookID);
                        book.bookState = Constants.STATE_LOST;
                        BookTransaction bt = new BookTransaction()
                        {
                            bookID = transaction.bookID,
                            createdAt = DateTime.Now,
                            giverUserID = transaction.giverUserID,
                            takerUserID = transaction.takerUserID,
                            transactionType = Constants.TRANSACTION_LOST
                        };
                        context.BookTransactions.Add(bt);
                    }
                }
                context.SaveChanges();
            }
            catch(Exception e)
            {
                string errorPath = @"C:\SchedulerErrors.txt";
                File.AppendAllText(errorPath, Environment.NewLine + e.Message + "\t Date : " + DateTime.Now);
                if (!string.IsNullOrEmpty(e.InnerException.Message))
                {
                    File.AppendAllText(errorPath, " \t Inner Exception : " + e.InnerException.Message);
                }
            }
        }
    }
}
