﻿using Humb.Core.DTOs;
using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//TO DO : User utilsde olup burda olmayan metodları search servisine ekle.
namespace Humb.Core.Interfaces.ServiceInterfaces
{
    public interface IUserService
    {
        void CreateUser(string email, string password, string nameSurname);
        int GetTotalUserCount();
        bool UserEmailExist(string email);
        bool UserExist(int userId);
        bool UserExist(string email, string password);
        int GetUserId(string email);
        User GetUser(string email);
        User GetUser(int userId);
        string GetUserFcmToken(int userId);
        int GetUserBookCount(int userId);
        int GetUserProfilePoint(int userId);
        string GetUserProfilePictureUrl(string email);
        string GetUserProfilePictureThumbnailUrl(string email);
        bool IsUserVerified(string email);
        bool DoesUserLocationExist(string email);
        bool IsUserBlocked(int fromUserId, int toUserId);
        void BlockUser(int fromUserId, int toUserId);
        void ConfirmPasswordChange(string email, string token);
        string[] SaveUserProfilePicture(string email, string path, string thumbnailPath);
        void ReportUser(int fromUserID, int toUserID, int reportCode, string reportInfo);
        void ResendEmailVerification(string email);
        void SendEmail(string email, string nameSurname, string verificationHash);
        double GetDistanceBetweenTwoUsers(double lat1, double lat2, double lon1, double lon2);
        void UpdateUserLocation(string email, double latidue, double longitude);
        void UpdateUserName(string email, string name);
        void UpdateUserBio(string email, string bio);
        string ChangeUserPassword(string email, string password);
        void UserForgotPassword(string email);
        int GetUserGivenBookCount(int userId);
        IList<BookDTO> GetUserCurrentlyReadingBooks(int userId);
        IList<BookDTO> GetUserBooksOnHand(int userId);
        IList<BookDTO> GetUserReadBooks(int userId);
        IList<BookDTO> GetUserOnRoadBooks(int userId);
    }
}
