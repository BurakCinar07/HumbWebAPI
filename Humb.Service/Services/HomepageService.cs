using Humb.Core.Constants;
using Humb.Core.Entities;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Humb.Core.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Service.Services
{
    public class HomepageService : IHomepageService
    {
        private readonly IUserService _userService;
        private readonly IRepository<BookInteraction> _bookInteractionRepository;
        private readonly IBookService _bookService;


        public HomepageService(IUserService userService, IBookService bookService, IRepository<BookInteraction> bookInteractionRepository)
        {
            _userService = userService;
            _bookService = bookService;
            _bookInteractionRepository = bookInteractionRepository;
        }
        
        public int GetBookPopularity(string bookName, DateTime dateTime)
        {
            int popularity = 0;
            popularity += _bookInteractionRepository.FindBy(x => (x.InteractionType == ResponseConstant.INTERACTION_READ_START ||
            x.InteractionType == ResponseConstant.INTERACTION_READ_STOP) && x.Book.BookName == bookName && x.CreatedAt > dateTime).GroupBy(x => x.UserId).Select(x => x.FirstOrDefault()).Count();

            return popularity;
        }
        public IEnumerable<Book> GetHomepagePopularBooks(int userId)
        {
            List<Book> returnBooks = new List<Book>();
            User user = _userService.GetUser(userId);
            Dictionary<string, int> bookPopularities = new Dictionary<string, int>();
            int days = -14;
            DateTime dateTime = DateTime.Now.AddDays(days);
            var bookInteractions = _bookInteractionRepository.FindBy(x => x.CreatedAt > dateTime).GroupBy(x => x.Book.BookName).
                    Select(y => y.FirstOrDefault()).Select(i => new { i.Book.BookName });

            //Son 2 hafta içinde 5 tane kitap bulamazsa 2 hafta daha geriden bakar
            while (bookInteractions.Count() < ResponseConstant.POPULAR_BOOKS_COUNT && days > -42)
            {
                days -= 14;
                dateTime = dateTime.AddDays(days);
                bookInteractions = _bookInteractionRepository.FindBy(x => x.CreatedAt > dateTime && x.UserId != user.Id).GroupBy(x => x.Book.BookName).
                    Select(y => y.FirstOrDefault()).Select(i => new { i.Book.BookName });
            }

            //Kitapları isimlerine göre gruplar populerliklerine göre sıralar ilk 5 i döndürür.
            foreach (var interaction in bookInteractions)
            {
                bookPopularities.Add(interaction.BookName, GetBookPopularity(interaction.BookName, dateTime));
            }
            var sortedBookPopularities = bookPopularities.OrderByDescending(x => x.Value).Take(5);
            foreach (var entry in sortedBookPopularities)
            {
                returnBooks.Add(_bookService.GetRandomBookByBookName(entry.Key));
            }

            return returnBooks;
        }

        public IEnumerable<Book> GetFirstViewedBookList(int userId)
        {
            User user = _userService.GetUser(userId);
            List<Book> returnBooks = new List<Book>();
            IEnumerable<Book> lovedGenreBooks = new List<Book>();
            Book randomBook;
            #region If user's location is null
            if (user.Latitude == null || user.Longitude == null)
            {
                if (user.LovedGenres.Count > 0)
                {
                    lovedGenreBooks = _bookService.GetBooksByLovedGenres(user.LovedGenres);
                    if (lovedGenreBooks.Count() > 20)
                    {
                        foreach (var book in lovedGenreBooks.OrderBy(x => Guid.NewGuid()).Take(20))
                        {
                            returnBooks.Add(book);
                        }
                        if (returnBooks.Count < 20)
                        {
                            for (int i = returnBooks.Count; i < 20; i++)
                            {
                                randomBook = GetRandomBook(user, returnBooks.Select(x=>x.Id));
                                if (randomBook != null)
                                    returnBooks.Add(randomBook);
                            }
                        }
                    }
                    else
                    {
                        foreach (var book in lovedGenreBooks.OrderBy(x => Guid.NewGuid()))
                        {
                            returnBooks.Add(book);
                        }
                        if (returnBooks.Count < 20)
                        {
                            int count = returnBooks.Count;
                            for (int i = count; i < 20; i++)
                            {
                                randomBook = GetRandomBook(user, returnBooks.Select(x=>x.Id));
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
                        randomBook = GetRandomBook(user, returnBooks.Select(x => x.Id));
                        if (randomBook != null)
                        {
                            returnBooks.Add(randomBook);
                        }
                    }
                }
                return returnBooks.OrderBy(x => Guid.NewGuid());
            }
            #endregion
            #region If user's location isnt null
            else
            {
                //Userın loved genre sayısı 0 dan büyükse 
                if (user.LovedGenres.Count > 0)
                {
                    //Userın loved genrelarındaki kitapları çeker
                    lovedGenreBooks = _bookService.GetBooksByLovedGenres(user.LovedGenres);
                    User tempUser;
                    //Kitap sayısı 20 den fazlaysa kitabın ownerı ile user arasındaki mesafeye bakar rastgele 20 kitap döndürür.
                    if (lovedGenreBooks.Count() > 20)
                    {
                        foreach (var book in lovedGenreBooks.OrderBy(x => Guid.NewGuid()).Take(20))
                        {
                            tempUser = _userService.GetUser(book.OwnerId);
                            if (_userService.GetDistanceBetweenTwoUsers(user.Latitude, tempUser.Latitude, user.Longitude, tempUser.Longitude) < ResponseConstant.MAX_DISTANCE)
                            {
                                returnBooks.Add(book);
                            }
                        }
                        //Konum kıyaslamasından dolayı 20 den az kitap filtrelenmişse kalanları konuma göre random atar.
                        if (returnBooks.Count < 20)
                        {
                            for (int i = returnBooks.Count; i < 20; i++)
                            {
                                randomBook = GetRandomBook(user, returnBooks.Select(x=>x.Id));
                                if (randomBook != null)
                                    returnBooks.Add(randomBook);
                            }
                            //Hala 20 den azsa konuma bakmadan random kitap atar.
                            if (returnBooks.Count < 20)
                            {
                                for (int i = returnBooks.Count; i < 20; i++)
                                {
                                    randomBook = GetRandomBook(user, returnBooks.Select(x=>x.Id));
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
                            tempUser = _userService.GetUser(book.OwnerId);
                            if (_userService.GetDistanceBetweenTwoUsers(user.Latitude, tempUser.Latitude, user.Longitude, tempUser.Longitude) < ResponseConstant.MAX_DISTANCE)
                            {
                                returnBooks.Add(book);
                            }
                        }
                        //Dönecek kitaplar 20 den azsa kalanları konuma bakarak random atar.
                        int count = returnBooks.Count;
                        for (int i = count; i < 20; i++)
                        {
                            randomBook = GetRandomBook(user, returnBooks.Select(x=>x.Id));
                            if (randomBook != null)
                                returnBooks.Add(randomBook);
                        }
                        //Hala azsa konumdan bağımsız random atar.
                        if (returnBooks.Count < 20)
                        {
                            for (int i = returnBooks.Count; i < 20; i++)
                            {
                                randomBook = GetRandomBook(user, returnBooks.Select(x=>x.Id));
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
                        randomBook = GetRandomBook(user, returnBooks.Select(x=>x.Id));
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
                            randomBook = GetRandomBook(user, returnBooks.Select(x=>x.Id));
                            if (randomBook != null)
                                returnBooks.Add(randomBook);
                        }
                    }
                }
                //Dönecek kitapların sırasını değiştirir sürekli aynı kitaplar dönmesin diye.
            }
            #endregion
            return returnBooks.OrderBy(x => Guid.NewGuid()).ToList();
        }

        public IEnumerable<Book> GetScrolledBookList(int userId, int[] bookIds)
        {
            throw new NotImplementedException();
        }

        public Book GetRandomBook(User user, IEnumerable<int> bookIds)
        {
            throw new NotImplementedException();
        }
    }
}
