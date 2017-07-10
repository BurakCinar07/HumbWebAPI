using BookieAPI.Constants;
using BookieAPI.Controllers.Utils.ModelUtils;
using BookieAPI.Models.Context;
using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace BookieAPI.Controllers.Utils
{
    public static class UserUtils
    {
        private const string imageURL = "http://82.165.97.141:4000/Images/";
        private const int SEARCH_RETURN_COUNT_KEY_NOT_PRESSED = 5;
        private const int SEARCH_RETURN_COUNT_KEY_PRESSED = 20;

        internal static bool EmailExist(Context context, string email)
        {
            return context.Users.Any(x => x.email == email);
        }

        internal static bool UserExist(Context context, int userID)
        {
            return context.Users.Any(x => x.userID == userID);
        }

        public static bool UserExist(Context context, string email, string password)
        {
            return context.Users.Any(x => x.email == email && x.password == password);
        }

        public static int GetUserID(Context context, string email)
        {
            return context.Users.Where(x => x.email == email).FirstOrDefault().userID;
        }

        public static User GetUser(Context context, string email)
        {
            return context.Users.Where(x => x.email == email).FirstOrDefault();
        }
        public static User GetUser(Context context, int userID)
        {
            return context.Users.Where(x => x.userID == userID).FirstOrDefault();
        }

        public static UserModel GetUserModel(Context context, int userID)
        {
            UserModel um = new UserModel();
            User user = new User();
            user = GetUser(context, userID);
            um.ID = user.userID;
            um.latitude = user.latitude;
            um.longitude = user.longitude;
            um.nameSurname = user.nameSurname;
            um.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
            um.profilePictureURL = user.profilePictureURL;
            return um;
        }

        public static UserLoginModel GetUserLoginModel(Context context, string email)
        {
            User user = GetUser(context, email);

            UserLoginModel um = new UserLoginModel();
            um.ID = user.userID;
            um.nameSurname = user.nameSurname;
            um.profilePictureURL = user.profilePictureURL;
            um.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
            um.latitude = user.latitude;
            um.longitude = user.longitude;
            um.email = user.email;
            um.password = user.password;
            um.bio = user.bio;
            um.emailVerified = user.emailVerified;
            um.counter = UserUtils.GetUserBookCounter(context, user.userID);
            um.point = UserUtils.GetUserPoint(context, user.userID);

            return um;
        }

        public static string GetUserFcmToken(Context context, int userID)
        {
            return context.Users.Where(x => x.userID == userID).FirstOrDefault().fcmToken;
        }

        public static int GetUserBookCounter(Context context, int userID)
        {
            int counter = 100;
            counter += context.BookTransactions.Where(x => x.transactionType == ResponseConstant.TRANSACTION_DISPATCH && x.giverUserID == userID).Count();
            counter -= context.BookTransactions.Where(x => x.transactionType == ResponseConstant.TRANSACTION_DISPATCH && x.takerUserID == userID).Count();

            //Dispatch olduğunda puan direk artırıldığı için kitabı kaybeden giverdan önce verilen puan alınır ardından bir puan daha düşürülür, takerdan puan dispatchte düşürüldüğü için puanına dokunulmaz.
            counter -= 2 * context.BookTransactions.Where(x => x.transactionType == ResponseConstant.TRANSACTION_LOST && x.giverUserID == userID).Count();
            return counter;
        }

        public static int GetUserPoint(Context context, int userID)
        {
            int point = 0;
            point += context.BookInteractions.Where(x => x.interactionType == ResponseConstant.INTERACTION_ADD && x.userID == userID).Count() * 2;
            point += context.BookInteractions.Where(x => x.interactionType == ResponseConstant.INTERACTION_READ_STOP && x.userID == userID).GroupBy(x => x.bookID).Select(x => x.FirstOrDefault()).Count();
            point += context.BookTransactions.Where(x => x.transactionType == ResponseConstant.TRANSACTION_COME_TO_HAND && x.giverUserID == userID).Count() * 5;
            point += context.BookTransactions.Where(x => x.transactionType == ResponseConstant.TRANSACTION_COME_TO_HAND && x.takerUserID == userID).Count() * 3;

            point -= context.BookTransactions.Where(x => x.transactionType == ResponseConstant.TRANSACTION_LOST && (x.giverUserID == userID || x.takerUserID == userID)).Count() * 20;
            return point;
        }

        public static string GetUserProfilePictureUrl(Context context, string email)
        {
            string url = context.Users.Where(x => x.email == email).FirstOrDefault().profilePictureURL;
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            else
            {
                return url.Substring(url.LastIndexOf('/') + 1);
            }
        }
        public static string GetUserProfilePictureThumbnailUrl(Context context, string email)
        {
            string url = context.Users.Where(x => x.email == email).FirstOrDefault().profilePictureThumbnailURL;
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            else
            {
                return url.Substring(url.LastIndexOf('/') + 1);
            }
        }

        public static bool IsUserVerified(Context context, string email)
        {
            return context.Users.Any(x => x.email == email && x.emailVerified == true);
        }

        internal static bool IsUserLocationExist(Context context, string email)
        {
            return context.Users.Any(x => x.email == email && (x.latitude != null && x.longitude != null));
        }

        internal static bool IsUserBlocked(Context context, int fromUserID, int toUserID)
        {
            return context.Blocks.Any(x => (x.fromUserID == fromUserID && x.toUserID == toUserID) || (x.fromUserID == toUserID && x.toUserID == fromUserID));
        }


        internal static void BlockUser(Context context, int fromUserID, int toUserID)
        {
            Block b = new Block();
            b.createdAt = DateTime.Now;
            b.fromUserID = fromUserID;
            b.toUserID = toUserID;
            context.Blocks.Add(b);
            context.SaveChanges();
        }

        internal static void ConfirmPasswordChange(Context context, string email, string token)
        {
            ForgottenPassword fp = context.ForgottenPasswords.FirstOrDefault(x => x.email == email && x.token == token);
            if (fp != null)
            {
                User u = new User();
                u = GetUser(context, email);
                u.password = TextUtils.CalculateMD5Hash(fp.newPassword);
                if (!u.emailVerified)
                {
                    u.emailVerified = true;
                }
                context.ForgottenPasswords.Remove(fp);
                context.SaveChanges();
            }
        }

        public static string[] SaveUserProfilePicture(Context context, string email, string path, string thumbnailPath)
        {
            User user = context.Users.Where(x => x.email == email).FirstOrDefault();
            user.profilePictureURL = imageURL + "/ProfilePictures/" + path;
            user.profilePictureThumbnailURL = imageURL + "/ProfilePicturesThumbnails/" + thumbnailPath;
            context.SaveChanges();
            string[] returnURLs = { user.profilePictureURL, user.profilePictureThumbnailURL };
            return returnURLs;
        }

        internal static void ReportUser(Context context, int fromUserID, int toUserID, int reportCode, string reportInfo)
        {
            Report report = new Report();
            if (string.IsNullOrEmpty(reportInfo))
            {
                report.reportInfo = reportInfo;
            }
            report.reportInfo = reportInfo;
            report.createdAt = DateTime.Now;
            report.fromUserID = fromUserID;
            report.toUserID = toUserID;
            report.reportCode = reportCode;
            context.Reports.Add(report);
            context.SaveChanges();
        }

        internal static void ResendEmailVerification(Context context, string email)
        {
            User user = new User();
            user = UserUtils.GetUser(context, email);
            user.verificationHash = TextUtils.CalculateMD5Hash(new Random().Next(0, 1000).ToString());
            SendEmail(context, email, user.nameSurname, user.verificationHash);
            context.SaveChanges();
        }

        public static void SendEmail(Context context, string email, string nameSurname, string verificationHash)
        {
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = false;
            client.Host = "mail.mantreads.com";

            client.Port = 587;
            // setup Smtp authentication
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential("noreply@mantreads.com", "Burakasdf");
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;
            client.Timeout = 1000;
            //can be obtained from your model
            MailMessage msg = new MailMessage();

            msg.From = new MailAddress("noreply@mantreads.com");
            msg.To.Add(new MailAddress(email));

            msg.Subject = "From:noreply@mantreads.com";
            msg.IsBodyHtml = true;
            msg.Body = string.Format("<html><head></head><body>Thanks for signing up " + nameSurname + "!<br>Your account has been created, you can login with the following credentials after you have activated your account by pressing the url below. <br> 82.165.97.141:4000/api/EmailVerification?email=" + email + "&verificationHash=" + verificationHash + "</body></html>");

            client.Send(msg);

        }
        
        public static double GetDistanceBetweenTwoUsers(double? lat1, double? lat2, double? lon1, double? lon2)
        {

            if (!lat2.HasValue && !lon2.HasValue)
            {
                return 999;
            }
            var p = 0.017453292519943295;    // Math.PI / 180
            var a = 0.5 - Math.Cos((lat2.Value - lat1.Value) * p) / 2 +
                    Math.Cos(lat1.Value * p) * Math.Cos(lat2.Value * p) *
                    (1 - Math.Cos((lon2.Value - lon1.Value) * p)) / 2;

            return 12742 * Math.Asin(Math.Sqrt(a)); // 2 * R; R = 6371 km
        }
        public static List<UserModel> GetUsersBySearchStringNotPressed(Context context, string searchString, string email)
        {
            List<UserModel> returnUsers = new List<UserModel>();
            int userID = UserUtils.GetUserID(context, email);
            List<int> returnUserIDs = new List<int>();
            foreach (int id in context.Users.Where(x => x.userID != userID && x.nameSurname.StartsWith(searchString)).Select(x=>x.userID))
            {
                returnUserIDs.Add(id);
            }
            if (returnUserIDs.Count < SEARCH_RETURN_COUNT_KEY_NOT_PRESSED)
            {
                foreach (int id in context.Users.Where(x => x.userID != userID && x.nameSurname.Contains(searchString)).Select(x => x.userID))
                {
                    if (!returnUserIDs.Contains(id))
                        returnUserIDs.Add(id);
                }
            }
            foreach (int id in returnUserIDs)
            {
                returnUsers.Add(GetUserModel(context, id));
            }
            return returnUsers.Take(SEARCH_RETURN_COUNT_KEY_NOT_PRESSED).ToList();
        }

        internal static List<UserModel> GetUsersBySearchString(Context context, string searchString, string email)
        {
            List<UserModel> returnUsers = new List<UserModel>();
            int userID = UserUtils.GetUserID(context, email);
            List<int> returnUserIDs = new List<int>();
            foreach (int id in context.Users.Where(x => x.userID != userID && x.nameSurname.StartsWith(searchString)).Select(x => x.userID))
            {
                returnUserIDs.Add(id);
            }
            if (returnUserIDs.Count < SEARCH_RETURN_COUNT_KEY_PRESSED)
            {
                foreach (int id in context.Users.Where(x => x.userID != userID && x.nameSurname.Contains(searchString)).Select(x => x.userID))
                {
                    if (!returnUserIDs.Contains(id))
                        returnUserIDs.Add(id);
                }
            }
            if(returnUserIDs.Count < SEARCH_RETURN_COUNT_KEY_PRESSED)
            {
                Dictionary<int, double> levensteinRatios = new Dictionary<int, double>();
                foreach (var user in context.Users.Where(x=>x.userID != userID).Select(x => new { x.userID, x.nameSurname }))
                {
                    if (!returnUserIDs.Contains(user.userID))
                    {
                        double commonCharacters = TextUtils.GetCommonCharacters(searchString, user.nameSurname);
                        double levDis = SearchUtils.DamerauLevenshteinDistance(searchString, user.nameSurname, 10);
                        double ratio = commonCharacters / levDis;
                        if (ratio == 0)
                        {
                            ratio = 1 / levDis;
                        }
                        if (ratio > 0.55)
                            levensteinRatios.Add(user.userID, ratio);
                    }
                }
                var sortedRatios = levensteinRatios.OrderByDescending(x => x.Value);
                foreach(var entry in sortedRatios)
                {
                    returnUserIDs.Add(entry.Key);
                }
            }
            foreach (int id in returnUserIDs)
            {
                returnUsers.Add(GetUserModel(context, id));
            }
            return returnUsers.Take(SEARCH_RETURN_COUNT_KEY_PRESSED).ToList();
        }

        internal static void UpdateUserLocation(Context context, string email, double latidue, double longitude)
        {
            User user = new User();
            user = GetUser(context, email);
            user.latitude = latidue;
            user.longitude = longitude;
            context.SaveChanges();
        }

        internal static void UpdateUserName(Context context, string email, string name)
        {
            User user = new User();
            user = GetUser(context, email);
            user.nameSurname = name;
            context.SaveChanges();
        }

        internal static void UpdateUserBio(Context context, string email, string bio)
        {
            User user = new User();
            user = GetUser(context, email);
            user.bio = bio;
            context.SaveChanges();
        }

        internal static string UserChangePassword(Context context, string email, string password)
        {
            User user = new User();
            user = GetUser(context, email);
            user.password = TextUtils.CalculateMD5Hash(password);
            context.SaveChanges();
            return user.password;
        }
        internal static void UserForgotPassword(Context context, string email)
        {
            User user = new User();
            user = GetUser(context, email);
            string password = TextUtils.GenerateRandomPassword();

            ForgottenPassword fp = new ForgottenPassword();
            fp = context.ForgottenPasswords.FirstOrDefault(x => x.email == email);
            if (fp != null)
            {
                context.ForgottenPasswords.Remove(fp);
                context.SaveChanges();
            }
            fp = new ForgottenPassword();
            fp.createdAt = DateTime.Now;
            fp.email = email;
            fp.newPassword = password;
            fp.token = TextUtils.CalculateMD5Hash(new Random().Next(0, 1000).ToString());
            context.ForgottenPasswords.Add(fp);
            context.SaveChanges();

            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = false;
            client.Host = "mail.mantreads.com";

            client.Port = 587;
            // setup Smtp authentication
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential("noreply@mantreads.com", "Burakasdf");
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;
            client.Timeout = 1000;
            //can be obtained from your model
            MailMessage msg = new MailMessage();

            msg.From = new MailAddress("noreply@mantreads.com");
            msg.To.Add(new MailAddress(email));

            msg.Subject = "From:noreply@mantreads.com";
            msg.IsBodyHtml = true;
            msg.Body = string.Format("<html><head></head><body>We recieved you forgot your password " + user.nameSurname + "!<br>Your new password is: " + password + "<br>If you didn't request a new password please ignore this mail. If not please click the link below to set your password. 82.165.97.141:4000/api/ConfirmPasswordHash?email=" + user.email + "&token=" + fp.token + "</body></html>");

            client.Send(msg);
        }

        public static List<BookModel> GetUserCurrentlyReadingBooks(Context context, int userID)
        {
            List<BookModel> books = new List<BookModel>();
            BookModel bm;
            UserModel um = new UserModel();
            um = UserUtils.GetUserModel(context, userID);


            var booksCurrentlyReading = context.Books.Where(x => x.bookState == ResponseConstant.STATE_READING && x.ownerID == userID).
                Select(i => new { i.bookID, i.bookName, i.bookPictureURL, i.bookPictureThumbnailURL, i.author, i.bookState, i.genreCode });
            foreach (var book in booksCurrentlyReading)
            {
                bm = new BookModel();

                bm.ID = book.bookID;
                bm.bookName = book.bookName;
                bm.bookPictureURL = book.bookPictureURL;
                bm.bookPictureThumbnailURL = book.bookPictureThumbnailURL;
                bm.author = book.author;
                bm.bookState = book.bookState;
                bm.genreCode = book.genreCode;
                bm.owner = um;
                books.Add(bm);
            }
            return books;
        }
        internal static int GetUserSharedCount(Context context, int uID)
        {
            return context.BookTransactions.Where(x => x.giverUserID == uID && x.transactionType == ResponseConstant.TRANSACTION_COME_TO_HAND).Count();
        }

        public static List<BookModel> GetUserBooksOnHand(Context context, int userID)
        {
            List<BookModel> books = new List<BookModel>();
            UserModel um = new UserModel();
            um = UserUtils.GetUserModel(context, userID);
            BookModel bm;
           
            var booksOnHand = context.BookInteractions.Where(x => (x.Book.bookState == ResponseConstant.STATE_OPENED_TO_SHARE || x.Book.bookState == ResponseConstant.STATE_CLOSED_TO_SHARE ||
                x.Book.bookState == ResponseConstant.STATE_ON_ROAD || x.Book.bookState == ResponseConstant.STATE_LOST) && x.Book.ownerID == userID).GroupBy(x => x.bookID).Select(x => x.OrderByDescending(j => j.createdAt)).Select(x => x.FirstOrDefault()).OrderByDescending(x => x.createdAt).
                Select(i => new { i.Book.bookID, i.Book.bookName, i.Book.bookPictureURL, i.Book.bookPictureThumbnailURL, i.Book.author, i.Book.bookState, i.Book.genreCode });

            foreach (var book in booksOnHand)
            {
                bm = new BookModel();

                bm.ID = book.bookID;
                bm.bookName = book.bookName;
                bm.bookPictureURL = book.bookPictureURL;
                bm.bookPictureThumbnailURL = book.bookPictureThumbnailURL;
                bm.author = book.author;
                bm.bookState = book.bookState;
                bm.genreCode = book.genreCode;
                bm.owner = um;
                books.Add(bm);
            }

            return books;


        }
        public static List<BookModel> GetUserReadBooks(Context context, int userID)
        {
            List<BookModel> books = new List<BookModel>();
            BookModel bookModel;
            User user = GetUser(context, userID);
            UserModel userModel = new UserModel();
            userModel = UserUtils.GetUserModel(context, userID);
            var bookInteractions = context.BookInteractions.Where(x => x.interactionType == ResponseConstant.INTERACTION_READ_STOP && x.userID == userID).
                GroupBy(x => x.bookID).Select(x => x.OrderByDescending(j => j.createdAt)).Select(x => x.FirstOrDefault()).OrderByDescending(x => x.createdAt).
                Select(i => new { i.bookID, i.Book.bookName, i.Book.bookPictureThumbnailURL, i.Book.bookPictureURL, i.Book.author, i.Book.bookState, i.Book.genreCode });

            foreach (var bi in bookInteractions)
            {
                bookModel = new BookModel();
                bookModel.ID = bi.bookID;
                bookModel.bookName = bi.bookName;
                bookModel.bookPictureURL = bi.bookPictureURL;
                bookModel.bookPictureThumbnailURL = bi.bookPictureThumbnailURL;
                bookModel.author = bi.author;
                bookModel.bookState = bi.bookState;
                bookModel.genreCode = bi.genreCode;
                bookModel.owner = userModel;
                books.Add(bookModel);
            }
            return books;
        }
        public static List<BookModel> GetUserOnRoadBooks(Context context, int uID)
        {
            List<BookModel> bms = new List<BookModel>();
            BookModel book;

            var lastTransactions = context.BookTransactions
                .Where(x => x.takerUserID == uID)
                .OrderByDescending(x => x.createdAt)
                .GroupBy(y => y.bookID).Select(i => i.FirstOrDefault()).OrderByDescending(x => x.createdAt).
                Select(i => new { i.bookID, i.giverUserID, i.takerUserID, i.createdAt })
                .ToList();

            foreach (var lastTransaction in lastTransactions)
            {
                BookTransaction transaction = TransactionUtils.GetLastTransaction(context, lastTransaction.bookID);
                if (transaction.takerUserID == uID && transaction.transactionType == ResponseConstant.TRANSACTION_DISPATCH)
                {
                    book = new BookModel();
                    book = BookUtils.GetBookModel(context, lastTransaction.bookID);
                    if (book.owner.ID == lastTransaction.giverUserID && book.owner.ID != uID && book.bookState == ResponseConstant.STATE_ON_ROAD)
                    {
                        bms.Add(book);
                    }
                }
            }
            return bms;
        }

    }
}