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
using Humb.Core.Interfaces;

namespace Humb.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<BlockUser> _blockedUsersRepository;
        private readonly IRepository<ReportUser> _reportedUsersRepository;
        private readonly IRepository<ForgottenPassword> _forgottenPasswordsRepository;
        private readonly IBookTransactionService _bookTransactionService;
        private readonly IBookInteractionService _bookInteractionService;
        private readonly IBookService _bookService;
        private readonly IEmailService _emailService;
        public UserService(IBookTransactionService bookTransactionService, IBookInteractionService bookInteractionService, IBookService bookService, IRepository<User> userRepo, IRepository<Book> bookRepo, IRepository<BlockUser> blockUserRepo, IRepository<ForgottenPassword> forgottenPasswordsRepo
            , IRepository<ReportUser> reportedUsersRepo, IEmailService emailService)
        {
            _bookTransactionService = bookTransactionService;
            _bookInteractionService = bookInteractionService;
            _bookService = bookService;
            _userRepository = userRepo;
            _blockedUsersRepository = blockUserRepo;
            _reportedUsersRepository = reportedUsersRepo;
            _forgottenPasswordsRepository = forgottenPasswordsRepo;
            _emailService = emailService;
        }

        public void CreateUser(string email, string password, string nameSurname)
        {
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
            _emailService.SendEmail(EmailEnums.VerificationEmail, LanguageEnums.Turkish, user.Email, user.NameSurname, user.VerificationHash);            
        }
        public void BlockUser(int fromUserId, int toUserId)
        {
            if (!IsUserExist(fromUserId) || !IsUserExist(toUserId))
                throw new Exception();
            BlockUser blockedUsers = new BlockUser()
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                CreatedAt = DateTime.Now
            };
            _blockedUsersRepository.Insert(blockedUsers);
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
            fp = new ForgottenPassword()
            {
                Email = email,
                NewPassword = TextHelper.GenerateRandomPassword(),
                Token = TextHelper.CalculateMD5Hash(new Random().Next(0, 1000).ToString()),
                CreatedAt = DateTime.Now
            };
            _forgottenPasswordsRepository.Insert(fp);
            _emailService.SendEmail(EmailEnums.ForgottenPasswordEmail, LanguageEnums.Turkish, user.Email, user.NameSurname, fp.NewPassword, fp.Token);
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
                _userRepository.Update(user, user.Id);
            }
        }
        public bool IsUserLocationExist(string email)
        {
            return _userRepository.Any(x => x.Email == email && (x.Latitude != null && x.Longitude != null));
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
        
        public int GetUserBookCounter(int userId)
        {
            int counter = 100;
            counter += _bookTransactionService.GetGiverUserTransactionCount(userId, ResponseConstant.TRANSACTION_DISPATCH);
            counter -= _bookTransactionService.GetTakerUserTransactionCount(userId, ResponseConstant.TRANSACTION_DISPATCH);

            //Dispatch olduğunda puan direk artırıldığı için kitabı kaybeden giverdan önce verilen puan alınır ardından bir puan daha düşürülür, takerdan puan dispatchte düşürüldüğü için puanına dokunulmaz.
            counter -= 2 * _bookTransactionService.GetGiverUserTransactionCount(userId, ResponseConstant.TRANSACTION_LOST);
            return counter;
        }

        public string GetFcmToken(int userId)
        {
            return _userRepository.GetById(userId).FcmToken;
        }

        public int GetUserGivenBookCount(int userId)
        {
            return _bookTransactionService.GetGiverUserTransactionCount(userId, ResponseConstant.TRANSACTION_COME_TO_HAND);
        }

        public int GetUserId(string email)
        {
            return _userRepository.FindSingleBy(x => x.Email == email).Id;
        }
        public User GetBookOwner(int bookId)
        {
            int ownerId = _bookService.GetBookOwnerId(bookId);
            return _userRepository.FindSingleBy(x => x.Id == ownerId);
        }
        public string GetUserProfilePictureThumbnailUrl(string email)
        {
            return _userRepository.FindSingleBy(x => x.Email == email).ProfilePictureThumbnailUrl;
        }

        public string GetUserProfilePictureUrl(string email)
        {
            return _userRepository.FindSingleBy(x => x.Email == email).ProfilePictureUrl;
        }

        public int GetUserProfilePoint(int userId)
        {
            int point = 0;
            point += _bookInteractionService.GetUserInteractionCountWithType(userId, ResponseConstant.INTERACTION_ADD) * 2;
            point += _bookInteractionService.GetUserInteractionCountWithTypeDistinct(userId, ResponseConstant.INTERACTION_READ_STOP);
            point += _bookTransactionService.GetGiverUserTransactionCount(userId, ResponseConstant.TRANSACTION_COME_TO_HAND) * 5;
            point += _bookTransactionService.GetTakerUserTransactionCount(userId, ResponseConstant.TRANSACTION_COME_TO_HAND) * 3;

            point -= _bookTransactionService.GetGiverUserTransactionCount(userId, ResponseConstant.TRANSACTION_LOST) * 10;
            point -= _bookTransactionService.GetTakerUserTransactionCount(userId, ResponseConstant.TRANSACTION_LOST) * 10;
            return point;
        }

        public bool IsUserBlocked(int fromUserId, int toUserId)
        {
            return _blockedUsersRepository.Any(x => (x.FromUserId == fromUserId && x.ToUserId == toUserId) || (x.FromUserId == toUserId && x.ToUserId == fromUserId));
        }

        public bool IsUserVerified(string email)
        {
            return _userRepository.Any(x => x.Email == email && x.EmailVerified);
        }

        public void ReportUser(int fromUserID, int toUserID, int reportCode, string reportInfo)
        {
            ReportUser ru = new ReportUser()
            {
                FromUserId = fromUserID,
                ToUserId = toUserID,
                ReportCode = reportCode,
                ReportInfo = reportInfo,
                CreatedAt = DateTime.Now
            };
            _reportedUsersRepository.Insert(ru);
        }

        public void ResendEmailVerification(string email)
        {
            User user = GetUser(email);
            user.VerificationHash = TextHelper.CalculateMD5Hash(new Random().Next(0, 1000).ToString());
            _userRepository.Update(user, user.Id);
            _emailService.SendEmail(EmailEnums.VerificationEmail, LanguageEnums.Turkish, user.Email, user.NameSurname, user.VerificationHash);
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
            User user = GetUser(email);
            user.Bio = bio;
            _userRepository.Update(user, user.Id);
        }

        public void UpdateUserLocation(string email, double latidue, double longitude)
        {
            User user = GetUser(email);
            user.Latitude = latidue;
            user.Longitude = longitude;
            _userRepository.Update(user, user.Id);
        }

        public void UpdateUserName(string email, string name)
        {
            User user = GetUser(email);
            user.NameSurname = name;
            _userRepository.Update(user, user.Id);
        }

        public bool IsUserEmailExist(string email)
        {
            return _userRepository.Any(x => x.Email == email);
        }

        public bool IsUserExist(int userId)
        {
            return _userRepository.Any(x => x.Id == userId);
        }

        public bool IsUserExist(string email, string password)
        {
            return _userRepository.Any(x => x.Email == email && x.Password == password);
        }

        
    }
}
