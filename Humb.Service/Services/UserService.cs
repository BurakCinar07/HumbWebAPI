using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humb.Core.DTOs;
using Humb.Core.Entities;
using Humb.Core.Interfaces.ServiceInterfaces;
using Humb.Core.Interfaces.RepositoryInterfaces;

namespace Humb.Service.Services
{
    public class UserService : IUserService
    {
        private IRepository<User> _userRepository;
        private IRepository<BlockUser> _blockUserRepository;
        public UserService(IRepository<User> userRepo, IRepository<BlockUser> blockUserRepo)
        {
            this._userRepository = userRepo;
            this._blockUserRepository = blockUserRepo;
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
                VerificationHash = Helper.CalculateMD5Hash(new Random().Next(0, 1000).ToString()),
            };
            _userRepository.Insert(user);
        }
        public void BlockUser(int fromUserId, int toUserId)
        {
            BlockUser blockedUsers = new BlockUser()
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                CreatedAt = DateTime.Now
            };
            _blockUserRepository.Insert(blockedUsers);
        }

        public string ChangeUserPassword(string email, string password)
        {
            throw new NotImplementedException();
        }

        public void ConfirmPasswordChange(string email, string token)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void UserForgotPassword(string email)
        {
            throw new NotImplementedException();
        }
    }
}
