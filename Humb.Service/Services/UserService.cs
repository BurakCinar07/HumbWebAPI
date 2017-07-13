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
using Humb.Core.Interfaces.ProviderInterfaces.EmailProviders;
using Humb.Service.Providers;

namespace Humb.Service.Services
{
    public class UserService : IUserService
    {
        private IRepository<User> _userRepository;
        private IRepository<BlockUser> _blockUserRepository;
        private IRepository<ForgottenPassword> _forgottenPasswordsRepository;
        private IEmailDispatcher _emailDispatcher;
        public UserService(IRepository<User> userRepo, IRepository<BlockUser> blockUserRepo, IRepository<ForgottenPassword> forgottenPasswordsRepo, IEmailDispatcher emailDispatcher)
        {
            this._userRepository = userRepo;
            this._blockUserRepository = blockUserRepo;
            this._forgottenPasswordsRepository = forgottenPasswordsRepo;
            this._emailDispatcher = emailDispatcher;
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
            string newPassword = TextHelper.GenerateRandomPassword();
            ForgottenPassword fp = _forgottenPasswordsRepository.FindSingleBy(x => x.Email == email);
            if(fp != null)
            {
                _forgottenPasswordsRepository.Delete(fp);
            }
            _emailDispatcher.Dispatch(new VerificationEmailGenerator().Generate(user));

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
        public bool DoesUserLocationExist(string email)
        {
            throw new NotImplementedException();
        }

        public double GetDistanceBetweenTwoUsers(double lat1, double lat2, double lon1, double lon2)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string email)
        {
            return _userRepository.FindSingleBy(x => x.Email == email);
        }

        public User GetUser(int userId)
        {
            throw new NotImplementedException();
        }

        public int GetUserBookCount(int userId)
        {
            throw new NotImplementedException();
        }

        public IList<BookDTO> GetUserBooksOnHand(int userId)
        {
            throw new NotImplementedException();
        }

        public IList<BookDTO> GetUserCurrentlyReadingBooks(int userId)
        {
            throw new NotImplementedException();
        }

        public string GetUserFcmToken(int userId)
        {
            throw new NotImplementedException();
        }

        public int GetUserGivenBookCount(int userId)
        {
            throw new NotImplementedException();
        }

        public int GetUserId(string email)
        {
            throw new NotImplementedException();
        }

        public IList<BookDTO> GetUserOnRoadBooks(int userId)
        {
            throw new NotImplementedException();
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

        public IList<BookDTO> GetUserReadBooks(int userId)
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
