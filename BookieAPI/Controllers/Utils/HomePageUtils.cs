using BookieAPI.Models.Context;
using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookieAPI.Controllers.Utils.ModelUtils;
using BookieAPI.Constants;
namespace BookieAPI.Controllers.Utils
{
    public static class HomePageUtils
    {
        const int HEADER_BOOK_COUNT = 5;
        //Anasayfa ilk kez yüklenirken ya da yenilenince populer kitapları döndürür.
        public static List<BookModel> GetHeaderBooks(Context context, string email)
        {
            List<BookModel> returnBooks = new List<BookModel>();
            User user = UserUtils.GetUser(context, email);
            Dictionary<string, int> bookPopularities = new Dictionary<string, int>();
            int days = -14;
            DateTime dateTime = DateTime.Now.AddDays(days);
            var bookInteractions = context.BookInteractions.Where(x => x.createdAt > dateTime).GroupBy(x => x.Book.bookName).
                    Select(y => y.FirstOrDefault()).Select(i => new { i.Book.bookName });

            //Son 2 hafta içinde 5 tane kitap bulamazsa 2 hafta daha geriden bakar
            while (bookInteractions.Count() < HEADER_BOOK_COUNT && days > -42)
            {
                days -= 14;
                dateTime = dateTime.AddDays(days);
                bookInteractions = context.BookInteractions.Where(x => x.createdAt > dateTime && x.userID != user.userID).GroupBy(x => x.Book.bookName).
                    Select(y => y.FirstOrDefault()).Select(i => new { i.Book.bookName });
            }

            //Kitapları isimlerine göre gruplar populerliklerine göre sıralar ilk 5 i döndürür.
            foreach (var interaction in bookInteractions)
            {
                bookPopularities.Add(interaction.bookName, BookUtils.GetBookPopularity(context, interaction.bookName, dateTime));
            }
            var sortedBookPopularities = bookPopularities.OrderByDescending(x => x.Value).Take(5);
            foreach (var entry in sortedBookPopularities)
            {
                returnBooks.Add(BookUtils.GetBookModelByName(context, entry.Key));
            }

            return returnBooks;
        }
        //Anasayfa ilk kez yüklenirken ya da yenilenince alt taraftaki kitapları döndürür.
        public static List<BookModel> GetListBooks(Context context, string email)
        {
            User user = UserUtils.GetUser(context, email);
            if (user.latitude != null)
            {
                return GetListBooksBasedOnLocation(context, user);
            }
            else
            {
                return GetListBooksNotBasedOnLocation(context, user);
            }
        }
        //Konumu dikkate almadan öncelik olarak loved genrelara bakarak eksik kalırsa random kitap atarak bookmodel listesi döndürür.
        private static List<BookModel> GetListBooksNotBasedOnLocation(Context context, User user)
        {
            List<Book> lovedGenreBooks = new List<Book>();
            List<BookModel> returnBooks = new List<BookModel>();
            BookModel randomBook = new BookModel();
            if (user.LovedGenres.Count > 0)
            {
                lovedGenreBooks = BookUtils.GetBooksByLovedGenres(context, user.LovedGenres);
                User tempUser;
                if (lovedGenreBooks.Count > 20)
                {
                    foreach (var book in lovedGenreBooks.OrderBy(x => Guid.NewGuid()).Take(20))
                    {
                        tempUser = UserUtils.GetUser(context, book.ownerID);
                        returnBooks.Add(BookUtils.GetBookModel(context, book.bookID));
                    }
                    if (returnBooks.Count < 20)
                    {
                        for (int i = returnBooks.Count; i < 20; i++)
                        {
                            randomBook = GetRandomBookModelNotBasedOnLocation(context, user, returnBooks);
                            if (randomBook != null)
                                returnBooks.Add(randomBook);
                        }
                    }
                }
                else
                {
                    foreach (var book in lovedGenreBooks.OrderBy(x => Guid.NewGuid()))
                    {
                        tempUser = UserUtils.GetUser(context, book.ownerID);
                        returnBooks.Add(BookUtils.GetBookModel(context, book.bookID));

                    }
                    if (returnBooks.Count < 20)
                    {
                        int count = returnBooks.Count;
                        for (int i = count; i < 20; i++)
                        {
                            randomBook = GetRandomBookModelNotBasedOnLocation(context, user, returnBooks);
                            if (randomBook != null)
                                returnBooks.Add(randomBook);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 20; i++)
                {
                    randomBook = GetRandomBookModelNotBasedOnLocation(context, user, returnBooks);
                    if (randomBook != null)
                    {
                        returnBooks.Add(randomBook);
                    }
                }
            }
            return returnBooks.OrderBy(x => Guid.NewGuid()).ToList();
        }
        //Konuma bağlı öncelik olarak loved genrelara bakarak eksik kalırsa random kitap atarak bookmodel listesi döndürür.
        private static List<BookModel> GetListBooksBasedOnLocation(Context context, User user)
        {
            List<Book> lovedGenreBooks = new List<Book>();
            List<BookModel> returnBooks = new List<BookModel>();
            BookModel randomBook = new BookModel();
            //Userın loved genre sayısı 0 dan büyükse 
            if (user.LovedGenres.Count > 0)
            {
                //Userın loved genrelarındaki kitapları çeker
                lovedGenreBooks = BookUtils.GetBooksByLovedGenres(context, user.LovedGenres);
                User tempUser;
                //Kitap sayısı 20 den fazlaysa kitabın ownerı ile user arasındaki mesafeye bakar rastgele 20 kitap döndürür.
                if (lovedGenreBooks.Count > 20)
                {
                    foreach (var book in lovedGenreBooks.OrderBy(x => Guid.NewGuid()).Take(20))
                    {
                        tempUser = UserUtils.GetUser(context, book.ownerID);
                        if (UserUtils.GetDistanceBetweenTwoUsers(user.latitude, tempUser.latitude, user.longitude, tempUser.longitude) < ResponseConstant.MIN_DISTANCE)
                        {
                            returnBooks.Add(BookUtils.GetBookModel(context, book.bookID));
                        }
                    }
                    //Konum kıyaslamasından dolayı 20 den az kitap filtrelenmişse kalanları konuma göre random atar.
                    if (returnBooks.Count < 20)
                    {
                        for (int i = returnBooks.Count; i < 20; i++)
                        {
                            randomBook = GetRandomBookModelBasedOnLocation(context, user, returnBooks);
                            if (randomBook != null)
                                returnBooks.Add(randomBook);
                        }
                        //Hala 20 den azsa konuma bakmadan random kitap atar.
                        if (returnBooks.Count < 20)
                        {
                            for (int i = returnBooks.Count; i < 20; i++)
                            {
                                randomBook = GetRandomBookModelNotBasedOnLocation(context, user, returnBooks);
                                if (randomBook != null)
                                    returnBooks.Add(randomBook);
                            }
                        }
                    }
                }
                //Sevdiği genrelardaki toplam kitap sayısı 20 den azsa
                else
                {
                    //Sevdiği genrelardaki kitapların hepsini konuma hesabına bağlı olarak alır.
                    foreach (var book in lovedGenreBooks.OrderBy(x => Guid.NewGuid()))
                    {
                        tempUser = UserUtils.GetUser(context, book.ownerID);
                        if (UserUtils.GetDistanceBetweenTwoUsers(user.latitude, tempUser.latitude, user.longitude, tempUser.longitude) < ResponseConstant.MIN_DISTANCE)
                        {
                            returnBooks.Add(BookUtils.GetBookModel(context, book.bookID));
                        }
                    }
                    //Dönecek kitaplar 20 den azsa kalanları konuma bakarak random atar.
                    int count = returnBooks.Count;
                    for (int i = count; i < 20; i++)
                    {
                        randomBook = GetRandomBookModelBasedOnLocation(context, user, returnBooks);
                        if (randomBook != null)
                            returnBooks.Add(randomBook);
                    }
                    //Hala azsa konumdan bağımsız random atar.
                    if (returnBooks.Count < 20)
                    {
                        for (int i = returnBooks.Count; i < 20; i++)
                        {
                            randomBook = GetRandomBookModelNotBasedOnLocation(context, user, returnBooks);
                            if (randomBook != null)
                                returnBooks.Add(randomBook);
                        }
                    }

                }
            }
            //Userın loved genre seçmediyse
            else
            {
                //Konuma bağlı rasgele 20 kitap atar.
                for (int i = 0; i < 20; i++)
                {
                    randomBook = GetRandomBookModelBasedOnLocation(context, user, returnBooks);
                    if (randomBook != null)
                    {
                        returnBooks.Add(randomBook);
                    }
                }
                //20 i doldurmazsa konumdan bağımsız rasgele kitap atar.
                if (returnBooks.Count < 20)
                {
                    for (int i = returnBooks.Count; i < 20; i++)
                    {
                        randomBook = GetRandomBookModelNotBasedOnLocation(context, user, returnBooks);
                        if (randomBook != null)
                            returnBooks.Add(randomBook);
                    }
                }
            }
            //Dönecek kitapların sırasını değiştirir sürekli aynı kitaplar dönmesin diye.
            return returnBooks.OrderBy(x => Guid.NewGuid()).ToList();
        }
        //Userın konumu varsa baktığı kitapları tekrar usera göstermez.
        public static List<BookModel> GetExcludedListBooks(Context context, string email, int[] bookIDs)
        {
            User user = UserUtils.GetUser(context, email);
            if (user.latitude != null)
            {
                return GetExcludedListBooksBasedOnLocation(context, user, bookIDs);
            }
            else
            {
                return GetExcludedListBooksNotBasedOnLocation(context, user, bookIDs);
            }
        }
        //Userın konumu varsa baktığı kitaplar haricindeki diğer kitapları konuma göre kitapları döndürür.
        private static List<BookModel> GetExcludedListBooksBasedOnLocation(Context context, User user, int[] bookIDs)
        {
            List<BookModel> returnBooks = new List<BookModel>();
            List<Book> lovedGenreBooks = new List<Book>();
            var booksID = context.Books.Where(x => x.ownerID != user.userID && (x.bookState == ResponseConstant.STATE_OPENED_TO_SHARE || x.bookState == ResponseConstant.STATE_READING)).Select(y => y.bookID);
            if (booksID.All(x => bookIDs.Contains(x)))
            {
                return returnBooks;
            }
            BookModel randomBook = new BookModel();
            if (user.LovedGenres.Count > 0)
            {
                lovedGenreBooks = BookUtils.GetBooksByLovedGenres(context, user.LovedGenres);
                User tempUser;
                if (lovedGenreBooks.Count > 20)
                {
                    foreach (var book in lovedGenreBooks.OrderBy(x => Guid.NewGuid()).Take(20))
                    {
                        tempUser = UserUtils.GetUser(context, book.ownerID);
                        if (UserUtils.GetDistanceBetweenTwoUsers(user.latitude, tempUser.latitude, user.longitude, tempUser.longitude) < ResponseConstant.MIN_DISTANCE && !booksID.Contains(book.bookID))
                        {
                            returnBooks.Add(BookUtils.GetBookModel(context, book.bookID));
                        }
                    }
                    if (returnBooks.Count < 20)
                    {
                        for (int i = returnBooks.Count; i < 20; i++)
                        {
                            randomBook = GetRandomExcludedBookModelBasedOnLocation(context, user, returnBooks, bookIDs);
                            if (randomBook != null)
                                returnBooks.Add(randomBook);
                        }
                        if (returnBooks.Count < 20)
                        {
                            for (int i = returnBooks.Count; i < 20; i++)
                            {
                                randomBook = GetRandomExcludedBookModelNotBasedOnLocation(context, user, returnBooks, bookIDs);
                                if (randomBook != null)
                                    returnBooks.Add(randomBook);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var book in lovedGenreBooks.OrderBy(x => Guid.NewGuid()))
                    {
                        tempUser = UserUtils.GetUser(context, book.ownerID);
                        if (UserUtils.GetDistanceBetweenTwoUsers(user.latitude, tempUser.latitude, user.longitude, tempUser.longitude) < ResponseConstant.MIN_DISTANCE && !bookIDs.Contains(book.bookID))
                        {
                            returnBooks.Add(BookUtils.GetBookModel(context, book.bookID));
                        }
                    }
                    if (returnBooks.Count < 20)
                    {
                        int count = returnBooks.Count;
                        for (int i = count; i < 20; i++)
                        {
                            randomBook = GetRandomExcludedBookModelBasedOnLocation(context, user, returnBooks, bookIDs);
                            if (randomBook != null)
                                returnBooks.Add(randomBook);
                        }
                        if (returnBooks.Count < 20)
                        {
                            for (int i = returnBooks.Count; i < 20; i++)
                            {
                                randomBook = GetRandomExcludedBookModelNotBasedOnLocation(context, user, returnBooks, bookIDs);
                                if (randomBook != null)
                                    returnBooks.Add(randomBook);
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 20; i++)
                {
                    randomBook = GetRandomExcludedBookModelBasedOnLocation(context, user, returnBooks, bookIDs);
                    if (randomBook != null)
                    {
                        returnBooks.Add(randomBook);
                    }
                }
                if (returnBooks.Count < 20)
                {
                    for (int i = returnBooks.Count; i < 20; i++)
                    {
                        randomBook = GetRandomExcludedBookModelNotBasedOnLocation(context, user, returnBooks, bookIDs);
                        if (randomBook != null)
                            returnBooks.Add(randomBook);
                    }
                }
            }
            return returnBooks.OrderBy(x => Guid.NewGuid()).ToList();
        }
        //Userın konumu yoksa baktığı kitaplar haricindeki diğer kitapları konumdan bağımsız olarak döndürür.
        private static List<BookModel> GetExcludedListBooksNotBasedOnLocation(Context context, User user, int[] bookIDs)
        {
            List<BookModel> returnBooks = new List<BookModel>();
            List<Book> lovedGenreBooks = new List<Book>();
            var booksID = context.Books.Where(x => x.ownerID != user.userID && (x.bookState == ResponseConstant.STATE_OPENED_TO_SHARE || x.bookState == ResponseConstant.STATE_READING)).Select(y => y.bookID);
            if (booksID.All(x => bookIDs.Contains(x)))
            {
                return returnBooks;
            }
            BookModel randomBook = new BookModel();
            if (user.LovedGenres.Count > 0)
            {
                lovedGenreBooks = BookUtils.GetBooksByLovedGenres(context, user.LovedGenres);
                User tempUser;
                if (lovedGenreBooks.Count > 20)
                {
                    foreach (var book in lovedGenreBooks.OrderBy(x => Guid.NewGuid()).Take(20))
                    {
                        tempUser = UserUtils.GetUser(context, book.ownerID);
                        if (!booksID.Contains(book.bookID))
                        {
                            returnBooks.Add(BookUtils.GetBookModel(context, book.bookID));
                        }
                    }
                    if (returnBooks.Count < 20)
                    {
                        for (int i = returnBooks.Count; i < 20; i++)
                        {
                            randomBook = GetRandomExcludedBookModelNotBasedOnLocation(context, user, returnBooks, bookIDs);
                            if (randomBook != null)
                                returnBooks.Add(randomBook);
                        }
                    }
                }
                else
                {
                    foreach (var book in lovedGenreBooks.OrderBy(x => Guid.NewGuid()))
                    {
                        tempUser = UserUtils.GetUser(context, book.ownerID);
                        if (!bookIDs.Contains(book.bookID))
                        {
                            returnBooks.Add(BookUtils.GetBookModel(context, book.bookID));
                        }
                    }
                    if (returnBooks.Count < 20)
                    {
                        int count = returnBooks.Count;
                        for (int i = count; i < 20; i++)
                        {
                            randomBook = GetRandomExcludedBookModelNotBasedOnLocation(context, user, returnBooks, bookIDs);
                            if (randomBook != null)
                                returnBooks.Add(randomBook);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 20; i++)
                {
                    randomBook = GetRandomExcludedBookModelNotBasedOnLocation(context, user, returnBooks, bookIDs);
                    if (randomBook != null)
                    {
                        returnBooks.Add(randomBook);
                    }
                }
            }
            return returnBooks.OrderBy(x => Guid.NewGuid()).ToList();
        }
        //Fetchlenen kitaplar dışındaki kitaplar içinden konumdan bağımsız olarak random bookmodel döndürür
        private static BookModel GetRandomExcludedBookModelNotBasedOnLocation(Context context, User user, List<BookModel> returnBooks, int[] bookIDs)
        {
            User tempUser;
            var availableBookIDs = context.Books.Where(x => x.ownerID != user.userID && (x.bookState == ResponseConstant.STATE_OPENED_TO_SHARE || x.bookState == ResponseConstant.STATE_READING)).Select(x => x.bookID).ToList();
            List<int> randomIDs = new List<int>();
            if (returnBooks.Count > 0)
            {
                foreach (int id in availableBookIDs)
                {
                    tempUser = BookUtils.GetBookOwner(context, id);
                    if (!returnBooks.Any(x => x.ID == id) && !bookIDs.Contains(id))
                    {
                        randomIDs.Add(id);
                    }
                }
            }

            BookModel randomBook = new BookModel();
            Random random = new Random();
            if (randomIDs.Count > 0)
            {
                int index = random.Next(0, randomIDs.Count - 1);
                randomBook = BookUtils.GetBookModel(context, randomIDs.ElementAtOrDefault(index));
            }
            else
            {
                return null;
            }
            return randomBook;
        }
        //Fetchlenen kitaplar dışındaki kitaplar içinden konuma bağlı olarak random bookmodel döndürür
        private static BookModel GetRandomExcludedBookModelBasedOnLocation(Context context, User user, List<BookModel> returnBooks, int[] bookIDs)
        {
            User tempUser;
            var availableBookIDs = context.Books.Where(x => x.ownerID != user.userID && (x.bookState == ResponseConstant.STATE_OPENED_TO_SHARE || x.bookState == ResponseConstant.STATE_READING)).Select(x => x.bookID).ToList();
            List<int> randomIDs = new List<int>();
            if (returnBooks.Count > 0)
            {
                foreach (int id in availableBookIDs)
                {
                    tempUser = BookUtils.GetBookOwner(context, id);
                    if (!returnBooks.Any(x => x.ID == id) && UserUtils.GetDistanceBetweenTwoUsers(user.latitude, tempUser.latitude, user.longitude, tempUser.longitude) < ResponseConstant.MIN_DISTANCE && !bookIDs.Contains(id))
                    {
                        randomIDs.Add(id);
                    }
                }
            }

            BookModel randomBook = new BookModel();
            Random random = new Random();
            if (randomIDs.Count > 0)
            {
                int index = random.Next(0, randomIDs.Count - 1);
                randomBook = BookUtils.GetBookModel(context, randomIDs.ElementAtOrDefault(index));
            }
            else
            {
                return null;
            }
            return randomBook;
        }
        //Konuma bağlı olarak bookmodel döndürür
        private static BookModel GetRandomBookModelBasedOnLocation(Context context, User user, List<BookModel> returnBooks)
        {
            User tempUser;
            var availableBooks = context.Books.Where(x => x.ownerID != user.userID && (x.bookState == ResponseConstant.STATE_OPENED_TO_SHARE || x.bookState == ResponseConstant.STATE_READING)).Select(x => x.bookID).ToList();
            if (returnBooks.Count > 0)
            {
                foreach (var book in returnBooks)
                {
                    tempUser = BookUtils.GetBookOwner(context, book.ID);
                    if (availableBooks.Any(x => x == book.ID) || UserUtils.GetDistanceBetweenTwoUsers(user.latitude, tempUser.latitude, user.longitude, tempUser.longitude) > ResponseConstant.MIN_DISTANCE)
                    {
                        availableBooks.Remove(book.ID);
                    }
                }
            }

            BookModel randomBook = new BookModel();
            Random random = new Random();
            if (availableBooks.Count > 0)
            {
                int index = random.Next(0, availableBooks.Count - 1);
                randomBook = BookUtils.GetBookModel(context, availableBooks.ElementAtOrDefault(index));
            }
            else
            {
                return null;
            }
            return randomBook;
        }
        //Konumdan bağımsız bookmodel döndürür
        private static BookModel GetRandomBookModelNotBasedOnLocation(Context context, User user, List<BookModel> returnBooks)
        {
            User tempUser;
            var availableBooks = context.Books.Where(x => x.ownerID != user.userID && (x.bookState == ResponseConstant.STATE_OPENED_TO_SHARE || x.bookState == ResponseConstant.STATE_READING)).Select(x => x.bookID).ToList();
            if (returnBooks.Count > 0)
            {
                foreach (var book in returnBooks)
                {
                    tempUser = BookUtils.GetBookOwner(context, book.ID);
                    if (availableBooks.Any(x => x == book.ID))
                    {
                        availableBooks.Remove(book.ID);
                    }
                }
            }

            BookModel randomBook = new BookModel();
            Random random = new Random();
            if (availableBooks.Count > 0)
            {
                int index = random.Next(0, availableBooks.Count - 1);
                randomBook = BookUtils.GetBookModel(context, availableBooks.ElementAtOrDefault(index));
            }            
            else
            {
                return null;
            }
            return randomBook;
        }
    }
}