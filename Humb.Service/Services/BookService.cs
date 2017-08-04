using Humb.Core.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Humb.Core.DTOs;
using Humb.Core.Entities;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Humb.Core.Constants;
using Humb.Core.Events;
using Humb.Service.Helpers;
using Humb.Core.Interfaces.ServiceInterfaces.InformClient;

namespace Humb.Service.Services
{
    public class BookService : IBookService
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<ReportBook> _reportedBookRepository;
        private readonly IUserService _userService;
        private readonly IBookTransactionService _bookTransactionService;
        private readonly IInformClientService _informClientService;
        public BookService(IRepository<Book> bookRepo, IRepository<ReportBook> reportedBookRepo, IUserService userService, IInformClientService informClientService, IBookTransactionService bookTransactionService)
        {
            _bookRepository = bookRepo;
            _reportedBookRepository = reportedBookRepo;
            _userService = userService;
            _bookTransactionService = bookTransactionService;
            _informClientService = informClientService;
        }
        public bool IsBookAddedByUser(int bookId, int userId)
        {
            return _bookRepository.Any(x => x.Id == bookId && x.AddedById == userId);
        }

        public int CreateBook(int userId, string path, string thumbnailPath, string bookName, string author, int bookState, int genreCode)
        {
            Book book = new Book()
            {
                BookName = bookName.Replace('_', ' '),
                Author = author.Replace('_', ' '),
                OwnerId = userId,
                AddedById = userId,
                CreatedAt = DateTime.Now,
                GenreCode = genreCode,
                BookState = bookState,
                BookPictureUrl = ResponseConstant.IMAGE_URL + "BookPictures/" + path,
                BookPictureThumbnailUrl = ResponseConstant.IMAGE_URL + "BookPicturesThumbnails/" + thumbnailPath,
            };
            _bookRepository.Insert(book);

            EventHub.Publish(new BookAdded(book.Id, userId, TypeConverter.BookStateToInteractionType(bookState)));
            return book.Id;
        }

        public bool IsUserBookOwner(int bookId, int userId)
        {
            return _bookRepository.Any(x => x.Id == bookId && x.OwnerId == userId);
        }

        public Book GetBook(int bookId)
        {
            return _bookRepository.FindSingleBy(x => x.Id == bookId);
        }

        public IList<BookDTO> GetBookDTOByGenre(int genreCode, string email, bool searchPressed)
        {
            throw new NotImplementedException();
        }

        public int GetBookOwnerId(int bookId)
        {
            return _bookRepository.FindSingleBy(x => x.Id == bookId).OwnerId;
        }

        public string GetBookPictureThumbnailURL(int bookId)
        {
            return _bookRepository.FindSingleBy(x => x.Id == bookId).BookPictureThumbnailUrl;
        }

        public string GetBookPictureUrl(int bookId)
        {
            return _bookRepository.FindSingleBy(x => x.Id == bookId).BookPictureUrl;
        }

        public IEnumerable<Book> GetBooksByLovedGenres(ICollection<LovedGenre> lovedGenres)
        {
            List<Book> returnBooks = new List<Book>();
            foreach (var genre in lovedGenres)
            {
                returnBooks.AddRange(_bookRepository.FindBy(x => x.OwnerId != genre.UserId && x.GenreCode == genre.GenreCode &&
                (x.BookState == ResponseConstant.STATE_OPENED_TO_SHARE || x.BookState == ResponseConstant.STATE_READING)));
            }
            return returnBooks;
        }

        public int GetBookState(int bookId)
        {
            return _bookRepository.FindSingleBy(x => x.Id == bookId).BookState;
        }

        public void ReportBook(int userId, int bookId, int reportCode, string reportInfo)
        {
            ReportBook rb = new ReportBook()
            {
                BookId = bookId,
                UserId = userId,
                ReportCode = reportCode,
                ReportInfo = reportInfo,
                CreatedAt = DateTime.Now
            };
            _reportedBookRepository.Insert(rb);
        }

        public bool SetBookStateLost(string email, int bookId)
        {
            Book book = GetBook(bookId);
            User user = _userService.GetUser(email);
            if (book.BookState != ResponseConstant.STATE_ON_ROAD || book.OwnerId != user.Id)
                return false;
            book.BookState = ResponseConstant.STATE_LOST;
            _bookRepository.Update(book, bookId);
            EventHub.Publish(new BookStateSetLost(book.Id, user.Id));
            //TODO : Let external services to get triggered by events.
            //_informClientService.InformClient(InformClientEnums.NotificationRequest, user.FcmToken, _userService.GetFcmToken(bt.TakerUserId), user, book, ResponseConstant.FCM_DATA_TYPE_TRANSACTION_LOST);
            return true;
        }

        public void UpdateBookDetails(int bookId, string bookName, string author, int genreCode)
        {
            Book book = GetBook(bookId);
            book.BookName = bookName;
            book.Author = author;
            book.GenreCode = genreCode;
            _bookRepository.Update(book, bookId);
        }

        public void UpdateBookOwner(int bookId, int userId)
        {
            Book book = GetBook(bookId);
            book.OwnerId = userId;
            _bookRepository.Update(book, bookId);
        }

        public string[] UpdateBookPicture(int bookId, string picturePath, string thumbnailPath)
        {
            Book book = GetBook(bookId);
            book.BookPictureUrl = ResponseConstant.IMAGE_URL + "BookPictures/" + picturePath;
            book.BookPictureThumbnailUrl = ResponseConstant.IMAGE_URL + "BookPicturesThumbnails/" + thumbnailPath;
            _bookRepository.Update(book, bookId);
            return new[] { book.BookPictureUrl, book.BookPictureThumbnailUrl };
        }

        public void UpdateBookState(int bookId, int bookState)
        {
            Book book = GetBook(bookId);
            book.BookState = bookState;
            _bookRepository.Update(book, bookId);
        }
        public IEnumerable<Book> GetUserCurrentlyReadingBooks(int userId)
        {
            return _bookRepository.FindBy(x => x.BookState == ResponseConstant.STATE_READING && x.OwnerId == userId);
        }

        #region Homepage methods
        public IEnumerable<Book> GetFirstViewedBookList(User user)
        {
            List<Book> returnBooks = new List<Book>();
            IEnumerable<Book> lovedGenreBooks;

            #region If user's location is null
            if (user.Latitude == null || user.Longitude == null)
            {
                if (user.LovedGenres.Count > 0)
                {
                    lovedGenreBooks = GetBooksByLovedGenres(user.LovedGenres);
                    foreach (var book in lovedGenreBooks.OrderBy(x => Guid.NewGuid()).Take(ResponseConstant.HOMEPAGE_BOOK_LIST_COUNT))
                    {
                        returnBooks.Add(book);
                    }
                }
            }
            #endregion
            #region If user's location isnt null
            else
            {
                //Userın loved genre sayısı 0 dan büyükse 
                if (user.LovedGenres.Count > 0)
                {
                    //Userın loved genrelarındaki kitapları çeker
                    lovedGenreBooks = GetBooksByLovedGenres(user.LovedGenres);
                    //Kitap sayısı 20 den fazlaysa kitabın ownerı ile user arasındaki mesafeye bakar rastgele 20 kitap döndürür.
                    foreach (var book in lovedGenreBooks.OrderBy(x => Guid.NewGuid()).Take(ResponseConstant.HOMEPAGE_BOOK_LIST_COUNT))
                    {
                        var tempUser = _userService.GetUser(book.OwnerId);
                        if (MathHelper.GetDistanceBetweenTwoUsers(user.Latitude, tempUser.Latitude, user.Longitude, tempUser.Longitude) < ResponseConstant.MAX_DISTANCE)
                        {
                            returnBooks.Add(book);
                        }
                    }
                }
                //Konum kısıtlamasından dolayı 20 den az kitap filtrelenmişse kalanları konuma göre random atar.                
                if (returnBooks.Count < ResponseConstant.HOMEPAGE_BOOK_LIST_COUNT)
                {
                    FillListWithRandomBooksByUsersDistances(user.Id, user.Latitude.Value, user.Longitude.Value, returnBooks);
                }

            }
            #endregion

            //20 i doldurmazsa konumdan bağımsız rasgele kitap atar.
            if (returnBooks.Count < ResponseConstant.HOMEPAGE_BOOK_LIST_COUNT)
            {
                FillListWithRandomBooks(user.Id, returnBooks);
            }
            //Dönecek kitapların sırasını değiştirir sürekli aynı kitaplar dönmesin diye.
            return returnBooks.OrderBy(x => Guid.NewGuid());
        }
        public IEnumerable<Book> GetScrolledBookList(User user, int[] bookIds)
        {
            List<Book> returnBooks = new List<Book>();            

            IEnumerable<Book> lovedGenreBooks;
            var availableBooksIds = _bookRepository.FindBy(x => x.OwnerId != user.Id &&
            (x.BookState == ResponseConstant.STATE_OPENED_TO_SHARE || x.BookState == ResponseConstant.STATE_READING)).
            Select(y => y.Id).Except(bookIds);
            #region If user's location is null
            if (user.Latitude == null || user.Longitude == null)
            {
                if (user.LovedGenres.Count > 0)
                {
                    returnBooks.AddRange(GetBooksByLovedGenres(user.LovedGenres).TakeWhile(x => !bookIds.Contains(x.Id))
                        .OrderBy(x => Guid.NewGuid()).Take(ResponseConstant.HOMEPAGE_BOOK_LIST_COUNT));
                }
            }
            #endregion
            #region If user's location isnt null
            else
            {
                //Userın loved genre sayısı 0 dan büyükse                 
                if (user.LovedGenres.Count > 0)
                {
                    //Userın loved genrelarındaki kitapları çeker
                    lovedGenreBooks = GetBooksByLovedGenres(user.LovedGenres);
                    User tempUser;
                    //Kitap sayısı 20 den fazlaysa kitabın ownerı ile user arasındaki mesafeye bakar rastgele 20 kitap döndürür.
                    foreach (var book in lovedGenreBooks.TakeWhile(x => !bookIds.Contains(x.Id)).OrderBy(x => Guid.NewGuid()))
                    {
                        tempUser = _userService.GetUser(book.OwnerId);
                        if (MathHelper.GetDistanceBetweenTwoUsers(user.Latitude, tempUser.Latitude, user.Longitude, tempUser.Longitude) < ResponseConstant.MAX_DISTANCE)
                        {
                            returnBooks.Add(book);
                        }
                    }
                }
                //Konum kısıtlamasından dolayı 20 den az kitap filtrelenmişse kalanları konuma göre random atar.                
                if (returnBooks.Count < ResponseConstant.HOMEPAGE_BOOK_LIST_COUNT)
                {
                    FillScrolledListWithRandomBooksByUserDistances(user.Id, user.Latitude.Value, user.Longitude.Value, returnBooks, availableBooksIds);
                }

            }
            #endregion
            //20 i doldurmazsa konumdan bağımsız rasgele kitap atar.
            if (returnBooks.Count < ResponseConstant.HOMEPAGE_BOOK_LIST_COUNT)
            {
                FillListWithRandomBooks(user.Id, returnBooks);
            }
            //Dönecek kitapların sırasını değiştirir sürekli aynı kitaplar dönmesin diye.
            return returnBooks.OrderBy(x => Guid.NewGuid()).Take(ResponseConstant.HOMEPAGE_BOOK_LIST_COUNT);
        }

        public Book GetRandomBookByBookName(string bookName)
        {
            return _bookRepository.FindBy(x => x.BookName == bookName && x.BookState != ResponseConstant.STATE_LOST && x.BookState != ResponseConstant.STATE_ON_ROAD).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }
        private void FillListWithRandomBooks(int userId, List<Book> returnBooks)
        {
            var availableBookIds = _bookRepository.FindBy(x => x.OwnerId != userId &&
            (x.BookState == ResponseConstant.STATE_OPENED_TO_SHARE || x.BookState == ResponseConstant.STATE_READING)).
            Select(x => x.Id);
            if (returnBooks.Count > 0)
            {
                var existingBookIds = returnBooks.Select(x => x.Id);
                availableBookIds = availableBookIds.Except(existingBookIds);
            }
            int availableBookCount = availableBookIds.Count();
            if (availableBookCount > 0)
            {
                availableBookIds = availableBookIds.OrderBy(x => Guid.NewGuid());
                while (returnBooks.Count < ResponseConstant.HOMEPAGE_BOOK_LIST_COUNT && availableBookCount > 0)
                {
                    returnBooks.Add(_bookRepository.GetById(availableBookIds.ElementAt(availableBookCount)));
                    availableBookCount--;
                }
            }
        }
        private void FillListWithRandomBooksByUsersDistances(int userId, double latitude, double longitude, List<Book> returnBooks)
        {
            var bookIds = _bookRepository.FindBy(x => x.OwnerId != userId &&
            (x.BookState == ResponseConstant.STATE_OPENED_TO_SHARE || x.BookState == ResponseConstant.STATE_READING)).
            Select(x => x.Id);
            List<int> availableBookIds;
            if (returnBooks.Count > 0)
            {
                var existingBookIds = returnBooks.Select(x => x.Id);
                availableBookIds = bookIds.Except(existingBookIds).ToList();
                int i = availableBookIds.Count() - 1;
                while (i >= 0)
                {
                    double[] tempUserLatLong = _userService.GetUserLocation(GetBookOwnerId(availableBookIds.ElementAt(i)));
                    if (MathHelper.GetDistanceBetweenTwoUsers(latitude, tempUserLatLong[0], longitude, tempUserLatLong[1]) > ResponseConstant.MAX_DISTANCE)
                    {
                        availableBookIds.RemoveAt(i);
                    }
                    i--;
                }
                int availableBookCount = availableBookIds.Count;
                if (availableBookCount > 0)
                {
                    availableBookIds = availableBookIds.OrderBy(x => Guid.NewGuid()).ToList();
                    while (returnBooks.Count < ResponseConstant.HOMEPAGE_BOOK_LIST_COUNT && availableBookCount >= 0)
                    {
                        returnBooks.Add(_bookRepository.GetById(availableBookIds.ElementAt(availableBookCount)));
                        availableBookCount--;
                    }
                }
            }
        }
        private void FillScrolledListWithRandomBooksByUserDistances(int userId, double latitude, double longitude, List<Book> returnBooks, IQueryable<int> bookIds)
        {
            List<int> availableBookIds;
            if (returnBooks.Count > 0)
            {
                availableBookIds = bookIds.Except(returnBooks.Select(x => x.Id)).ToList();
                int i = availableBookIds.Count();
                while (i >= 0)
                {
                    double[] tempUserLatLong = _userService.GetUserLocation(GetBookOwnerId(availableBookIds.ElementAt(i)));
                    if (MathHelper.GetDistanceBetweenTwoUsers(latitude, tempUserLatLong[0], longitude, tempUserLatLong[1]) > ResponseConstant.MAX_DISTANCE)
                    {
                        availableBookIds.RemoveAt(i);
                    }
                    i--;
                }
                Random random = new Random();
                int availableBookCount = availableBookIds.Count;
                if (availableBookCount > 0)
                {
                    availableBookIds = availableBookIds.OrderBy(x => Guid.NewGuid()).ToList();
                    while (returnBooks.Count < ResponseConstant.HOMEPAGE_BOOK_LIST_COUNT && availableBookCount >= 0)
                    {
                        returnBooks.Add(_bookRepository.GetById(availableBookIds.ElementAt(availableBookCount)));
                        availableBookCount--;
                    }
                }
            }
        }
        #endregion
    }
}
