using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces
{
    public interface IHomepageService
    {
        IEnumerable<Book> GetHomepagePopularBooks(int userId);
        int GetBookPopularity(string bookName, DateTime dateTime);
        IEnumerable<Book> GetFirstViewedBookList(int userId);
        IEnumerable<Book> GetScrolledBookList(int userId, int[] bookIds);
    }
}
