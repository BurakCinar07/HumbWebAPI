using BookieAPI.Constants;
using BookieAPI.Models.Context;
using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels.Models;
using BookieAPI.Models.ResponseModels.Models.BookDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Controllers.Utils.ModelUtils
{
    public static class BookUtils
    {
        private const string imageURL = "http://82.165.97.141:4000/Images/";
        private const int SEARCH_RETURN_COUNT_KEY_NOT_PRESSED = 5;
        private const int SEARCH_RETURN_COUNT_KEY_PRESSED = 20;


        public static int CreateBook(Context context, string email, string path, string thumbnailPath, string bookName, string author, int bookState, int genreCode)
        {
            bookName = bookName.Replace('_', ' ');
            author = author.Replace('_', ' ');
            Book book = new Book();
            int userID = UserUtils.GetUserID(context, email);
            book.ownerID = userID;
            book.createdAt = DateTime.Now;
            book.addedByID = userID;
            book.author = author;
            book.bookName = bookName;
            book.genreCode = genreCode;
            book.bookState = bookState;
            book.bookPictureURL = imageURL + "/BookPictures/" + path;
            book.bookPictureThumbnailURL = imageURL + "/BookPicturesThumbnails/" + thumbnailPath;
            context.Books.Add(book);
            context.SaveChanges();

            int interactionType = interactionType = ResponseConstant.INTERACTION_CLOSE_TO_SHARE;

            if (bookState == ResponseConstant.STATE_OPENED_TO_SHARE)
            {
                interactionType = ResponseConstant.INTERACTION_OPEN_TO_SHARE;
            }
            else if (bookState == ResponseConstant.STATE_READING)
            {
                interactionType = ResponseConstant.INTERACTION_READ_START;
            }
            else if (bookState == ResponseConstant.STATE_CLOSED_TO_SHARE)
            {
                interactionType = ResponseConstant.INTERACTION_CLOSE_TO_SHARE;
            }

            InteractionUtils.AddInteraction(context, book, email, interactionType);

            context.SaveChanges();
            return book.bookID;
        }

        public static Book GetBook(Context context, int ID)
        {
            return context.Books.Where(x => x.bookID == ID).FirstOrDefault();
        }

        internal static bool IsUserBooksCreater(Context context, int bookID, int userID)
        {
            return context.Books.Any(x => x.bookID == bookID && x.addedByID == userID);
        }

        public static BookModel GetBookModel(Context context, int bookID)
        {
            BookModel bm = new BookModel();
            Book book = new Book();
            book = context.Books.Where(x => x.bookID == bookID).FirstOrDefault();
            bm.author = book.author;
            bm.bookName = book.bookName;
            bm.bookPictureThumbnailURL = book.bookPictureThumbnailURL;
            bm.bookPictureURL = book.bookPictureURL;
            bm.bookState = book.bookState;
            bm.genreCode = book.genreCode;
            bm.ID = book.bookID;
            bm.owner = UserUtils.GetUserModel(context, book.ownerID);
            return bm;
        }

        public static BookDetailsModel GetBookDetailsModel(Context context, int bookID)
        {
            BookDetailsModel bm = new BookDetailsModel();
            Book book = GetBook(context, bookID);
            bm.addedBy = UserUtils.GetUserModel(context, book.addedByID);
            bm.owner = UserUtils.GetUserModel(context, book.ownerID);

            bm.author = book.author;
            bm.bookName = book.bookName;
            bm.bookPictureThumbnailURL = book.bookPictureThumbnailURL;
            bm.bookPictureURL = book.bookPictureURL;
            bm.bookState = book.bookState;
            bm.genreCode = book.genreCode;
            bm.ID = book.bookID;

            bm.bookInteractions = InteractionUtils.GetInteractions(context, book.bookID);
            bm.bookRequests = RequestUtils.GetRequests(context, book.bookID);
            bm.bookTransactions = TransactionUtils.GetTransactions(context, book.bookID);

            return bm;
        }

        public static int GetBookState(Context context, int bookID)
        {
            return context.Books.Where(x => x.bookID == bookID).FirstOrDefault().bookState;
        }

        internal static string GetBookPictureThumbnailURL(Context context, int bookID)
        {
            string url = context.Books.Where(x => x.bookID == bookID).FirstOrDefault().bookPictureThumbnailURL;
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            else
            {
                return url.Substring(url.LastIndexOf('/') + 1);
            }
        }

        internal static string GetBookPictureURL(Context context, int bookID)
        {
            string url = context.Books.Where(x => x.bookID == bookID).FirstOrDefault().bookPictureURL;
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            else
            {
                return url.Substring(url.LastIndexOf('/') + 1);
            }
        }

        internal static void UpdateBookState(Context context, int bookID, int state)
        {
            Book book = new Book();
            book = BookUtils.GetBook(context, bookID);
            book.bookState = state;
            context.SaveChanges();
        }

        public static string[] UpdateBookPicture(Context context, string picturePath, string thumbnailPath, int bookID)
        {
            Book b = context.Books.Where(x => x.bookID == bookID).FirstOrDefault();
            b.bookPictureURL = imageURL + "/BookPictures/" + picturePath;
            b.bookPictureThumbnailURL = imageURL + "/BookPicturesThumbnails/" + thumbnailPath;
            context.SaveChanges();
            string[] urls = { b.bookPictureURL, b.bookPictureThumbnailURL };
            return urls;
        }

        internal static void UpdateBookOwner(Context context, int bookID, int userID)
        {
            Book book = new Book();
            book = GetBook(context, bookID);
            book.ownerID = userID;
            context.SaveChanges();
        }

        internal static void UpdateBookDetails(Context context, int bookID, string bookName, string author, int genreCode)
        {            
            Book b = new Book();
            b = BookUtils.GetBook(context, bookID);
            b.author = author;
            b.bookName = bookName;
            b.genreCode = genreCode;
            context.SaveChanges();
        }

        public static bool IsBookOwnerExist(Context context, int bookID, int userID)
        {
            return context.Books.Any(x => x.bookID == bookID && x.ownerID == userID);
        }

        internal static void ReportBook(Context context, int userID, int bookID, int reportCode, string reportInfo)
        {
            ReportBook rb = new ReportBook();
            rb.createdAt = DateTime.Now;
            rb.bookID = bookID;
            rb.reportCode = reportCode;
            rb.reportInfo = reportInfo;
            rb.userID = userID;
            context.ReportBooks.Add(rb);
            context.SaveChanges();
        }

        public static List<BookModel> GetBookModelsByGenre(Context context, int genreCode, string email, bool searchPressed)
        {
            List<BookModel> bookModels = new List<BookModel>();
            User user = UserUtils.GetUser(context, email);
            if (!searchPressed)
            {
                #region If searching for a genre and searchPressed is false
                if (user.latitude != null)
                {
                    var books = context.Books.Where(x => x.ownerID != user.userID && x.genreCode == genreCode).Select(i => new { i.bookID, i.ownerID });
                    Dictionary<int, double> userDistances = new Dictionary<int, double>();
                    foreach (var book in books)
                    {
                        User owner = UserUtils.GetUser(context, book.ownerID);
                        userDistances.Add(book.bookID, UserUtils.GetDistanceBetweenTwoUsers(user.latitude, owner.latitude, user.longitude, owner.longitude));
                    }
                    var sortedDistances = userDistances.OrderBy(x => x.Value).Take(SEARCH_RETURN_COUNT_KEY_NOT_PRESSED);
                    foreach (var entry in sortedDistances)
                    {
                        bookModels.Add(BookUtils.GetBookModel(context, entry.Key));
                    }
                }
                else
                {
                    var books = context.Books.Where(x => x.ownerID != user.userID && x.genreCode == genreCode).Select(i => i.bookID).Take(SEARCH_RETURN_COUNT_KEY_NOT_PRESSED);
                    foreach (var book in books)
                    {
                        bookModels.Add(BookUtils.GetBookModel(context, book));
                    }
                }
                #endregion
            }
            else
            {
                #region If searching for a genre and searchPressed is true
                if (user.latitude != null)
                {
                    var books = context.Books.Where(x => x.ownerID != user.userID && x.genreCode == genreCode).Select(i => new { i.bookID, i.ownerID });
                    Dictionary<int, double> userDistances = new Dictionary<int, double>();
                    foreach (var book in books)
                    {
                        User owner = UserUtils.GetUser(context, book.ownerID);
                        userDistances.Add(book.bookID, UserUtils.GetDistanceBetweenTwoUsers(user.latitude, owner.latitude, user.longitude, owner.longitude));
                    }
                    var sortedDistances = userDistances.OrderBy(x => x.Value).Take(SEARCH_RETURN_COUNT_KEY_PRESSED);
                    foreach (var entry in sortedDistances)
                    {
                        bookModels.Add(BookUtils.GetBookModel(context, entry.Key));
                    }
                }
                else
                {
                    var books = context.Books.Where(x => x.ownerID != user.userID && x.genreCode == genreCode).Select(i => i.bookID).Take(SEARCH_RETURN_COUNT_KEY_PRESSED);
                    foreach (var book in books)
                    {
                        bookModels.Add(BookUtils.GetBookModel(context, book));
                    }
                }
                #endregion
            }
            return bookModels;
        }

        internal static User GetBookOwner(Context context, int bookID)
        {
            return UserUtils.GetUser(context, context.Books.First(x => x.bookID == bookID).ownerID);
        }
        private static List<BookModel> SearchBooksByStateKeyNotPressed(Context context, int userID, string searchString, int bookState)
        {
            List<BookModel> returnBooks = new List<BookModel>();
            List<int> returnBookIDs = new List<int>();
            foreach (int bookID in context.Books.Where(x => x.ownerID != userID && (x.bookName.StartsWith(searchString) || x.author.StartsWith(searchString)) && x.bookState == bookState).Select(x => x.bookID))
            {
                returnBookIDs.Add(bookID);
            }
            if (returnBookIDs.Count < 5)
            {
                foreach (int bookID in context.Books.Where(x => x.ownerID != userID && (x.bookName.Contains(searchString) || x.author.Contains(searchString)) && x.bookState == bookState).Select(x => x.bookID))
                {
                    if (!returnBookIDs.Contains(bookID))
                        returnBookIDs.Add(bookID);
                }
            }
            foreach (int bookID in returnBookIDs)
            {
                returnBooks.Add(GetBookModel(context, bookID));
            }
            return returnBooks;
        }
        //lokasyona göre sıralansın mı?
        public static List<BookModel> GetBooksBySearchStringNotPressed(Context context, string searchString, string email)
        {
            List<BookModel> returnBooks = new List<BookModel>();
            User user = UserUtils.GetUser(context, email);
            Dictionary<int, double> userDistances = new Dictionary<int, double>();
            #region //If search string equals any author
            if (context.Books.Any(x => x.ownerID != user.userID && x.author == searchString))
            {
                if (user.latitude != null)
                {
                    //Konuma göre arama yapıldıktan sonra konumsuz da yapılsın mı?
                    var books = context.Books.Where(x => x.ownerID != user.userID && x.author == searchString).Select(i => new { i.bookID, i.ownerID }).OrderBy(x => Guid.NewGuid());
                    foreach (var book in books)
                    {
                        User owner = UserUtils.GetUser(context, book.ownerID);
                        userDistances.Add(book.bookID, UserUtils.GetDistanceBetweenTwoUsers(user.latitude, owner.latitude, user.longitude, owner.longitude));
                    }
                    var sortedDistances = userDistances.OrderBy(x => x.Value).Take(SEARCH_RETURN_COUNT_KEY_NOT_PRESSED);
                    foreach (var entry in sortedDistances)
                    {
                        returnBooks.Add(BookUtils.GetBookModel(context, entry.Key));
                    }
                }
                else
                {
                    var bookIDs = context.Books.Where(x => x.ownerID != user.userID && x.author == searchString).Select(i => i.bookID).OrderBy(x => Guid.NewGuid()).Take(SEARCH_RETURN_COUNT_KEY_NOT_PRESSED);
                    foreach (var bookID in bookIDs)
                    {
                        returnBooks.Add(BookUtils.GetBookModel(context, bookID));
                    }
                }
            }
            #endregion
            #region //If not
            else
            {
                if (user.latitude != null)
                {
                    #region //Search for books whose state is by order opened to share, reading, closed to share, on road, lost
                    for (int i = ResponseConstant.STATE_READING; i < ResponseConstant.STATE_LOST; i++)
                    {
                        if (returnBooks.Count < SEARCH_RETURN_COUNT_KEY_NOT_PRESSED)
                        {
                            var books = SearchBooksByStateKeyNotPressed(context, user.userID, searchString, i);
                            foreach (var book in books)
                            {
                                User owner = UserUtils.GetUser(context, book.owner.ID);
                                if (UserUtils.GetDistanceBetweenTwoUsers(user.latitude, owner.latitude, user.longitude, owner.longitude) < ResponseConstant.MIN_DISTANCE)
                                {
                                    returnBooks.Add(book);
                                }
                            }
                            //Bakılan statede 5 tane kitap bulamazsa aynı statede lokasyona bakmadan 5 e tamamlamaya çalıssın mı
                        }
                    }
                    for (int i = ResponseConstant.STATE_READING; i < ResponseConstant.STATE_LOST; i++)
                    {
                        if (returnBooks.Count < SEARCH_RETURN_COUNT_KEY_NOT_PRESSED)
                        {
                            var books = SearchBooksByStateKeyNotPressed(context, user.userID, searchString, i);
                            foreach (var book in books)
                            {
                                if (!returnBooks.Any(x=>x.ID == book.ID))
                                    returnBooks.Add(book);
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region //If user's location is null search for books whose state is by order opened to share, reading, closed to share, on road, lost
                    for (int i = 0; i < ResponseConstant.STATE_LOST; i++)
                    {
                        if (returnBooks.Count < SEARCH_RETURN_COUNT_KEY_NOT_PRESSED)
                        {
                            var books = SearchBooksByStateKeyNotPressed(context, user.userID, searchString, i);
                            foreach (var book in books)
                            {
                                returnBooks.Add(book);
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion
            return returnBooks.Take(SEARCH_RETURN_COUNT_KEY_NOT_PRESSED).ToList();
        }

        public static List<BookModel> GetBooksBySearchString(Context context, string searchString, string email)
        {
            List<BookModel> returnBooks = new List<BookModel>();
            User user = UserUtils.GetUser(context, email);
            Dictionary<int, double> userDistances = new Dictionary<int, double>();
            #region //If search string equals any author
            if (context.Books.Any(x => x.ownerID != user.userID && x.author == searchString))
            {
                if (user.latitude != null)
                {
                    //Konumsuz arama da yapılsın mı?
                    var books = context.Books.Where(x => x.ownerID != user.userID && x.author == searchString).Select(i => new { i.bookID, i.ownerID }).OrderBy(x => Guid.NewGuid());
                    foreach (var book in books)
                    {
                        User owner = UserUtils.GetUser(context, book.ownerID);
                        
                        userDistances.Add(book.bookID, UserUtils.GetDistanceBetweenTwoUsers(user.latitude, owner.latitude, user.longitude, owner.longitude));
                    }
                    var sortedDistances = userDistances.OrderBy(x => x.Value).Take(SEARCH_RETURN_COUNT_KEY_PRESSED);
                    foreach (var entry in sortedDistances)
                    {
                        returnBooks.Add(BookUtils.GetBookModel(context, entry.Key));
                    }
                }
                else
                {
                    var bookIDs = context.Books.Where(x => x.ownerID != user.userID && x.author == searchString).Select(i => i.bookID).OrderBy(x => Guid.NewGuid()).Take(SEARCH_RETURN_COUNT_KEY_NOT_PRESSED);
                    foreach (var bookID in bookIDs)
                    {
                        returnBooks.Add(BookUtils.GetBookModel(context, bookID));
                    }
                }
            }
            #endregion
            #region //If not
            else
            {
                if (user.latitude != null)
                {
                    #region //Search for books whose state is by order opened to share, reading, closed to share, on road, lost
                    for (int i = ResponseConstant.STATE_READING; i < ResponseConstant.STATE_LOST; i++)
                    {
                        if (returnBooks.Count < SEARCH_RETURN_COUNT_KEY_PRESSED)
                        {
                            var books = SearchBooksByStateKeyPressed(context, user.userID, searchString, i);
                            foreach (var book in books)
                            {
                                User owner = UserUtils.GetUser(context, book.owner.ID);
                                if (UserUtils.GetDistanceBetweenTwoUsers(user.latitude, owner.latitude, user.longitude, owner.longitude) < ResponseConstant.MIN_DISTANCE)
                                {
                                    returnBooks.Add(book);
                                }
                            }
                        }
                    }
                    if (returnBooks.Count < SEARCH_RETURN_COUNT_KEY_PRESSED)
                    {
                        for (int i = ResponseConstant.STATE_READING; i < ResponseConstant.STATE_LOST; i++)
                        {
                            if (returnBooks.Count < SEARCH_RETURN_COUNT_KEY_PRESSED)
                            {
                                var books = SearchBooksByStateKeyPressed(context, user.userID, searchString, i);
                                foreach (var book in books)
                                {
                                    if (!returnBooks.Any(x=>x.ID == book.ID))
                                        returnBooks.Add(book);
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region //If user's location is null search for books whose state is by order opened to share, reading, closed to share, on road, lost
                    for (int i = 0; i < ResponseConstant.STATE_LOST; i++)
                    {
                        if (returnBooks.Count < SEARCH_RETURN_COUNT_KEY_PRESSED)
                        {
                            var books = SearchBooksByStateKeyPressed(context, user.userID, searchString, i);
                            foreach (var book in books)
                            {
                                returnBooks.Add(book);
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion
            return returnBooks.Take(SEARCH_RETURN_COUNT_KEY_PRESSED).ToList();
        }
        private static List<BookModel> SearchBooksByStateKeyPressed(Context context, int userID, string searchString, int bookState)
        {
            List<BookModel> returnBooks = new List<BookModel>();
            List<int> returnBookIDs = new List<int>();
            foreach (int bookID in context.Books.Where(x => x.ownerID != userID && (x.bookName.StartsWith(searchString) || x.author.StartsWith(searchString)) && x.bookState == bookState).Select(x => x.bookID))
            {
                returnBookIDs.Add(bookID);
            }
            if (returnBookIDs.Count < SEARCH_RETURN_COUNT_KEY_PRESSED)
            {
                foreach (int bookID in context.Books.Where(x => x.ownerID != userID && (x.bookName.Contains(searchString) || x.author.Contains(searchString)) && x.bookState == bookState).Select(x => x.bookID))
                {
                    if (!returnBookIDs.Contains(bookID))
                        returnBookIDs.Add(bookID);
                }
            }
            if (returnBookIDs.Count < SEARCH_RETURN_COUNT_KEY_PRESSED)
            {
                Dictionary<int, double> levensteinRatios = new Dictionary<int, double>();
                foreach (var book in context.Books.Where(x => x.ownerID != userID && x.bookState == bookState).Select(x => new { x.bookID, x.bookName }))
                {
                    if (!returnBookIDs.Contains(book.bookID))
                    {
                        double commonCharacters = TextUtils.GetCommonCharacters(searchString, book.bookName);
                        double levDis = SearchUtils.DamerauLevenshteinDistance(searchString, book.bookName, 10);
                        double ratio = commonCharacters / levDis;
                        if (ratio == 0)
                        {
                            ratio = 1 / levDis;
                        }
                        if (ratio > 0.55)
                            levensteinRatios.Add(book.bookID, ratio);
                    }
                }
                var sortedRatios = levensteinRatios.OrderByDescending(x => x.Value);
                foreach (var entry in sortedRatios)
                {
                    returnBookIDs.Add(entry.Key);
                }
            }
            foreach (int bookID in returnBookIDs)
            {
                returnBooks.Add(GetBookModel(context, bookID));
            }
            return returnBooks;
        }
        internal static bool SetBookStateLost(Context context, string email, int bookID)
        {
            Book b = new Book();
            b = BookUtils.GetBook(context, bookID);
            int userID = UserUtils.GetUserID(context, email);
            if (b.bookState == ResponseConstant.STATE_ON_ROAD)
            {
                if (b.ownerID == userID)
                {
                    b.bookState = ResponseConstant.STATE_LOST;
                    BookTransaction bt = context.BookTransactions.Where(x => x.bookID == bookID && x.giverUserID == userID).OrderByDescending(x => x.createdAt).First();
                    bt.transactionType = ResponseConstant.TRANSACTION_LOST;
                    context.SaveChanges();
                    FcmUtils.SendRequestNotification(context, bookID, userID, bt.takerUserID, ResponseConstant.FCM_DATA_TYPE_TRANSACTION_LOST);
                    return true;
                }
            }
            return false;
        }
        //Verilen tarihten sonraki read stop ile read start olan interactionlara bakar user id lere göre gruplar sürekli read start read stop yapılarak kitabın populer olmasını engellemek için 
        public static int GetBookPopularity(Context context, string bookName, DateTime dateTime)
        {
            int popularity = 0;
            popularity += context.BookInteractions.Where(x => (x.interactionType == ResponseConstant.INTERACTION_READ_START ||
            x.interactionType == ResponseConstant.INTERACTION_READ_STOP) && x.Book.bookName == bookName && x.createdAt > dateTime).GroupBy(x => x.userID).Select(x => x.FirstOrDefault()).Count();

            return popularity;
        }
        public static BookModel GetBookModelByName(Context context, string name)
        {
            return GetBookModel(context, context.Books.Where(x => x.bookName == name && x.bookState != Constants.ResponseConstant.STATE_LOST && x.bookState != Constants.ResponseConstant.STATE_ON_ROAD).OrderBy(x => Guid.NewGuid()).FirstOrDefault().bookID);
        }
        public static List<Book> GetBooksByLovedGenres(Context context, ICollection<LovedGenre> lovedGenres)
        {
            List<Book> returnBooks = new List<Book>();
            foreach (var genre in lovedGenres)
            {
                returnBooks.AddRange(context.Books.Where(x => x.ownerID != genre.userID && x.genreCode == genre.genreCode &&
                (x.bookState == Constants.ResponseConstant.STATE_OPENED_TO_SHARE || x.bookState == Constants.ResponseConstant.STATE_READING)));
            }
            return returnBooks;
        }
    }
}