using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.DTOs;
using Humb.Core.Entities;
using Humb.Core.Interfaces.ServiceInterfaces;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Humb.Service.Helpers;
using Humb.Core.Constants;
using Pelusoft.EasyMapper;
using Humb.Core.Interfaces.ServiceInterfaces.EmailInterfaces;

namespace Humb.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<BlockUser> _blockUserRepository;
        private readonly IRepository<ForgottenPassword> _forgottenPasswordsRepository;
        private readonly IRepository<BookTransaction> _bookTransactionRepository;
        private readonly IRepository<BookInteraction> _bookInteractionRepository;
        private readonly IBookTransactionService _bookTransactionService;
        private IEmailFactory _emailFactory;
        public UserService(IRepository<User> userRepo, IRepository<Book> bookRepo, IRepository<BlockUser> blockUserRepo, IRepository<ForgottenPassword> forgottenPasswordsRepo,
            IRepository<BookTransaction> bookTransactionRepo, IRepository<BookInteraction> bookInteractionRepo, IEmailFactory emailFactory)
        {
            this._userRepository = userRepo;
            this._bookRepository = bookRepo;
            this._blockUserRepository = blockUserRepo;
            this._forgottenPasswordsRepository = forgottenPasswordsRepo;
            this._bookTransactionRepository = bookTransactionRepo;
            this._bookInteractionRepository = bookInteractionRepo;
            this._emailFactory = emailFactory;
        }

        public void CreateUser(string email, string password, string nameSurname)
        {
            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(nameSurname) || String.IsNullOrEmpty(password))
                throw new ArgumentNullException("parameter is null");
            if (UserExist(email, password))
                throw new Exception();

            User user = new User()
            {
                Email = email,
                Password = password,
                NameSurname = nameSurname,
                CreatedAt = DateTime.Now,
                EmailVerified = false,
                VerificationHash = TextHelper.CalculateMD5Hash(new Random().Next(0, 1000).ToString()),
            };
            _userRepository.Insert(user);
        }
        public void BlockUser(int fromUserId, int toUserId)
        {
            if (!UserExist(fromUserId) || !UserExist(toUserId))
                throw new Exception();
            BlockUser blockedUsers = new BlockUser()
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                CreatedAt = DateTime.Now
            };
            _blockUserRepository.Insert(blockedUsers);
        }
        public int GetTotalUserCount()
        {
            return _userRepository.Count();
        }
        public string ChangePassword(string email, string newPassword)
        {
            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(newPassword))
                throw new ArgumentNullException("parameter is null");
            User user = GetUser(email);
            user.Password = TextHelper.CalculateMD5Hash(newPassword);
            _userRepository.Update(user, user.Id);
            return user.Password;
        }
        public void ForgotPasswordRequest(string email)
        {
            User user = GetUser(email);
            ForgottenPassword fp = _forgottenPasswordsRepository.FindSingleBy(x => x.Email == email);
            if (fp != null)
            {
                _forgottenPasswordsRepository.Delete(fp);
            }
            fp = new ForgottenPassword();
            fp.CreatedAt = DateTime.Now;
            fp.Email = email;
            fp.NewPassword = TextHelper.GenerateRandomPassword();
            fp.Token = TextHelper.CalculateMD5Hash(new Random().Next(0, 1000).ToString());
            object[] forgottenPasswordEmailObject = {user.Email, user.NameSurname, fp.NewPassword, fp.Token};
            _emailFactory.Initialize(EmailEnums.TurkishForgotPasswordEmail, forgottenPasswordEmailObject);
            _forgottenPasswordsRepository.Insert(fp);
        }
        public void ConfirmForgottenPasswordRequest(string email, string token)
        {
            ForgottenPassword fp = _forgottenPasswordsRepository.FindSingleBy(x => x.Email == email && x.Token == token);
            if (fp != null)
            {
                User user = GetUser(email);
                user.Password = TextHelper.CalculateMD5Hash(fp.NewPassword);
                if (!user.EmailVerified)
                {
                    user.EmailVerified = true;
                }
                _forgottenPasswordsRepository.Delete(fp);
            }
        }
        public bool IsUserLocationExist(string email)
        {
            return _userRepository.GetAll().Any(x => x.Email == email && (x.Latitude != null && x.Longitude != null));
        }

        public double GetDistanceBetweenTwoUsers(double lat1, double lat2, double lon1, double lon2)
        {
            var p = 0.017453292519943295;    // Math.PI / 180
            var a = 0.5 - Math.Cos((lat2 - lat1) * p) / 2 +
                    Math.Cos(lat1 * p) * Math.Cos(lat2 * p) *
                    (1 - Math.Cos((lon2 - lon1) * p)) / 2;

            return 12742 * Math.Asin(Math.Sqrt(a));
        }

        public User GetUser(string email)
        {
            return _userRepository.FindSingleBy(x => x.Email == email);
        }

        public User GetUser(int userId)
        {
            return _userRepository.FindSingleBy(x => x.Id == userId);
        }
        public UserDTO GetUserDTO(int userId)
        {
            return EasyMapper.Map<UserDTO>(GetUser(userId));
        }
        public int GetUserBookCount(int userId)
        {
            int counter = 100;
            counter += _bookTransactionRepository.FindBy(x => x.TransactionType == ResponseConstant.TRANSACTION_DISPATCH && x.GiverUserId == userId).Count();
            counter -= _bookTransactionRepository.FindBy(x => x.TransactionType == ResponseConstant.TRANSACTION_DISPATCH && x.TakerUserId == userId).Count();

            //Dispatch olduğunda puan direk artırıldığı için kitabı kaybeden giverdan önce verilen puan alınır ardından bir puan daha düşürülür, takerdan puan dispatchte düşürüldüğü için puanına dokunulmaz.
            counter -= 2 * _bookTransactionRepository.FindBy(x => x.TransactionType == ResponseConstant.TRANSACTION_LOST && x.GiverUserId == userId).Count();
            return counter;
        }

        //Is mapping necessary?
        public IList<BookDTO> GetUserBooksOnHand(int userId)
        {
            List<BookDTO> books = new List<BookDTO>();

            var booksOnHand = _bookInteractionRepository.FindBy(x => (x.Book.BookState == ResponseConstant.STATE_OPENED_TO_SHARE || x.Book.BookState == ResponseConstant.STATE_CLOSED_TO_SHARE ||
                x.Book.BookState == ResponseConstant.STATE_ON_ROAD || x.Book.BookState == ResponseConstant.STATE_LOST) && x.Book.OwnerId == userId).GroupBy(x => x.BookId).Select(x => x.OrderByDescending(j => j.CreatedAt)).Select(x => x.FirstOrDefault()).OrderByDescending(x => x.CreatedAt).
                Select(i => new { i.Book.Id, i.Book.BookName, i.Book.BookPictureUrl, i.Book.BookPictureThumbnailUrl, i.Book.Author, i.Book.BookState, i.Book.GenreCode });

            foreach (var book in booksOnHand)
            {
                books.Add(EasyMapper.Map<BookDTO>(book));
            }
            return books;
        }

        public IList<BookDTO> GetUserCurrentlyReadingBooks(int userId)
        {
            List<BookDTO> books = new List<BookDTO>();

            var booksCurrentlyReading = _bookRepository.FindBy(x => x.BookState == ResponseConstant.STATE_READING && x.OwnerId == userId).
                Select(i => new { i.Id, i.BookName, i.BookPictureUrl, i.BookPictureThumbnailUrl, i.Author, i.BookState, i.GenreCode });

            foreach (var book in booksCurrentlyReading)
            {
                books.Add(EasyMapper.Map<BookDTO>(book));
            }
            return books;
        }
        public IList<BookDTO> GetUserOnRoadBooks(int userId)
        {
            List<BookDTO> books = new List<BookDTO>();

            var bookTransactions = _bookTransactionRepository.FindBy(x => x.TakerUserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .GroupBy(y => y.Id).Select(i => i.FirstOrDefault()).OrderByDescending(x => x.CreatedAt)
                .Select(i => new { i.BookId, i.GiverUserId, i.TakerUserId, i.CreatedAt })
                .ToList();

            foreach (var bookTransaction in bookTransactions)
            {
                BookTransaction transaction = _bookTransactionService.GetBookLastTransaction(bookTransaction.BookId);
                if (transaction.TakerUserId == userId && transaction.TransactionType == ResponseConstant.TRANSACTION_DISPATCH)
                {
                    //book = GetBookModel(lastTransaction.bookID);
                    //if (book.owner.ID == lastTransaction.giverUserID && book.owner.ID != uID && book.bookState == ResponseConstant.STATE_ON_ROAD)
                    //{
                    //    bms.Add(book);
                    //}
                }
            }
            return books;
        }
        public IList<BookDTO> GetUserReadBooks(int userId)
        {
            List<BookDTO> books = new List<BookDTO>();
            var bookInteractions = _bookInteractionRepository.FindBy(x => x.InteractionType == ResponseConstant.INTERACTION_READ_STOP && x.UserId == userId).
                GroupBy(x => x.BookId).Select(x => x.OrderByDescending(j => j.CreatedAt)).Select(x => x.FirstOrDefault()).OrderByDescending(x => x.CreatedAt).
                Select(i => new { i.BookId, i.Book.BookName, i.Book.BookPictureThumbnailUrl, i.Book.BookPictureUrl, i.Book.Author, i.Book.BookState, i.Book.GenreCode, i.User });

            foreach (var book in bookInteractions)
            {
                books.Add(EasyMapper.Map<BookDTO>(book));
            }
            return books;
        }

        public string GetFcmToken(int userId)
        {
            return _userRepository.GetById(userId).FcmToken;
        }

        public int GetUserGivenBookCount(int userId)
        {
            return _bookTransactionRepository.FindBy(x => x.GiverUserId == userId && x.TransactionType == ResponseConstant.TRANSACTION_COME_TO_HAND).Count();
        }

        public int GetUserId(string email)
        {
            return _userRepository.FindSingleBy(x => x.Email == email).Id;
        }

        public string GetUserProfilePictureThumbnailUrl(string email)
        {
            throw new NotImplementedException();
        }

        public string GetUserProfilePictureUrl(string email)
        {
            throw new NotImplementedException();
        }

        public int GetUserProfilePoint(int userId)
        {
            throw new NotImplementedException();
        }


        public bool IsUserBlocked(int fromUserId, int toUserId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserVerified(string email)
        {
            throw new NotImplementedException();
        }

        public void ReportUser(int fromUserID, int toUserID, int reportCode, string reportInfo)
        {
            throw new NotImplementedException();
        }

        public void ResendEmailVerification(string email)
        {
            throw new NotImplementedException();
        }

        public string[] SaveUserProfilePicture(string email, string path, string thumbnailPath)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(string email, string nameSurname, string verificationHash)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserBio(string email, string bio)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserLocation(string email, double latidue, double longitude)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserName(string email, string name)
        {
            throw new NotImplementedException();
        }

        public bool UserEmailExist(string email)
        {
            throw new NotImplementedException();
        }

        public bool UserExist(int userId)
        {
            throw new NotImplementedException();
        }

        public bool UserExist(string email, string password)
        {
            return _userRepository.GetAll().Any(x => x.Email == email && x.Password == password);
        }



    }
}
