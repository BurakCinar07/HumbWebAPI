//using BookieAPI.Models.Context;
//using BookieAPI.Models.DAL;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using BookieAPI.Constants;
//using BookieAPI.Models.ResponseModels.Models;
//using System.Text;
//using System.Security.Cryptography;
//using BookieAPI.Models.ResponseModels.Models.BookDetails;
//using System.Net;
//using System.Web.Script.Serialization;
//using System.IO;
//using System.Net.Mail;

//namespace BookieAPI.Controllers
//{

//    public class Helpers
//    {
//        private const string imageURL = "http://82.165.97.141:4000/Images/";
//        private Context context = new Context();
//        public string CalculateMD5Hash(string input)
//        {
//            // step 1, calculate MD5 hash from input

//            MD5 md5 = System.Security.Cryptography.MD5.Create();

//            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

//            byte[] hash = md5.ComputeHash(inputBytes);

//            // step 2, convert byte array to hex string

//            StringBuilder sb = new StringBuilder();

//            for (int i = 0; i < hash.Length; i++)
//            {
//                sb.Append(hash[i].ToString("X2"));
//            }
//            return sb.ToString();
//        }

//        internal void ConfirmPasswordChange(string email, string token)
//        {
//            ForgottenPassword fp = context.ForgottenPasswords.FirstOrDefault(x => x.email == email && x.token == token);
//            if (fp != null)
//            {
//                User u = new User();
//                u = GetUser(email);
//                u.password = CalculateMD5Hash(fp.newPassword);
//                if (!u.emailVerified)
//                {
//                    u.emailVerified = true;
//                }
//                context.ForgottenPasswords.Remove(fp);
//                context.SaveChanges();
//            }
//        }

//        string GenerateRandomPassword()
//        {
//            Random rd = new Random();

//            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
//            char[] chars = new char[6];

//            for (int i = 0; i < 6; i++)
//            {
//                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
//            }
//            return new string(chars);

//        }
//        internal void UserForgotPassword(string email)
//        {
//            User user = new User();
//            user = GetUser(email);
//            string password = GenerateRandomPassword();
            
//            ForgottenPassword fp = new ForgottenPassword();
//            fp = context.ForgottenPasswords.FirstOrDefault(x => x.email == email);
//            if(fp != null)
//            {
//                context.ForgottenPasswords.Remove(fp);
//                context.SaveChanges();
//            }
//            fp = new ForgottenPassword();
//            fp.createdAt = DateTime.Now;
//            fp.email = email;
//            fp.newPassword = password;
//            fp.token = CalculateMD5Hash(new Random().Next(0, 1000).ToString());
//            context.ForgottenPasswords.Add(fp);
//            context.SaveChanges();

//            SmtpClient client = new SmtpClient();
//            client.DeliveryMethod = SmtpDeliveryMethod.Network;
//            client.EnableSsl = false;
//            client.Host = "mail.mantreads.com";

//            client.Port = 587;
//            // setup Smtp authentication
//            System.Net.NetworkCredential credentials =
//                new System.Net.NetworkCredential("noreply@mantreads.com", "Burakasdf");
//            client.UseDefaultCredentials = false;
//            client.Credentials = credentials;
//            client.Timeout = 1000;
//            //can be obtained from your model
//            MailMessage msg = new MailMessage();

//            msg.From = new MailAddress("noreply@mantreads.com");
//            msg.To.Add(new MailAddress(email));

//            msg.Subject = "From:noreply@mantreads.com";
//            msg.IsBodyHtml = true;
//            msg.Body = string.Format("<html><head></head><body>We recieved you forgot your password " + user.nameSurname + "!<br>Your new password is: "+ password +"<br>If you didn't request a new password please ignore this mail. If not please click the link below to set your password. 82.165.97.141:4000/api/ConfirmPasswordHash?email=" + user.email + "&token=" + fp.token + "</body></html>");

//            client.Send(msg);

//        }

//        internal void ResendEmailVerification(string email)
//        {
//            User user = new User();
//            user = GetUser(email);
//            user.verificationHash = CalculateMD5Hash(new Random().Next(0, 1000).ToString());
//            SendEmail(email, user.nameSurname, user.verificationHash);
//            context.SaveChanges();
//        }

//        internal bool SetBookStateLost(string email, int bookID)
//        {
//            Book b = new Book();
//            b = GetBook(bookID);
//            int userID = GetUserID(email);
//            if (b.bookState == ResponseConstant.STATE_ON_ROAD)
//            {
//                if (b.ownerID == userID)
//                {
//                    b.bookState = ResponseConstant.STATE_LOST;
//                    BookTransaction bt = context.BookTransactions.Where(x => x.bookID == bookID && x.fromUserID == userID).OrderByDescending(x => x.createdAt).First();
//                    bt.transactionType = ResponseConstant.TRANSACTION_LOST;
//                    context.SaveChanges();
//                    SendNotification(bookID, bt.toUserID, userID, ResponseConstant.FCM_DATA_TYPE_TRANSACTION_LOST);
//                    return true;
//                }
//            }
//            return false;
//        }

//        public void SendEmail(string email, string nameSurname, string verificationHash)
//        {
//            SmtpClient client = new SmtpClient();
//            client.DeliveryMethod = SmtpDeliveryMethod.Network;
//            client.EnableSsl = false;
//            client.Host = "mail.mantreads.com";

//            client.Port = 587;
//            // setup Smtp authentication
//            System.Net.NetworkCredential credentials =
//                new System.Net.NetworkCredential("noreply@mantreads.com", "Burakasdf");
//            client.UseDefaultCredentials = false;
//            client.Credentials = credentials;
//            client.Timeout = 1000;
//            //can be obtained from your model
//            MailMessage msg = new MailMessage();

//            msg.From = new MailAddress("noreply@mantreads.com");
//            msg.To.Add(new MailAddress(email));

//            msg.Subject = "From:noreply@mantreads.com";
//            msg.IsBodyHtml = true;
//            msg.Body = string.Format("<html><head></head><body>Thanks for signing up " + nameSurname + "!<br>Your account has been created, you can login with the following credentials after you have activated your account by pressing the url below. <br> 82.165.97.141:4000/api/EmailVerification?email=" + email + "&verificationHash=" + verificationHash + "</body></html>");

//            client.Send(msg);

//        }
//        public void SendEmailVerifiedFCM(string email)
//        {
//            try
//            {
//                string applicationID = ResponseConstant.APPLICATION_ID;

//                string receiverId = GetUserFcmToken(GetUserID(email));
//                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
//                tRequest.Method = "post";
//                tRequest.ContentType = "application/json";

//                var data = new
//                {

//                    to = receiverId,

//                    data = new
//                    {
//                        fcmDataType = ResponseConstant.FCM_DATA_TYPE_USER_VERIFIED,
//                    }
//                };

//                var serializer = new JavaScriptSerializer();

//                var json = serializer.Serialize(data);

//                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

//                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
//                tRequest.ContentLength = byteArray.Length;


//                using (Stream dataStream = tRequest.GetRequestStream())
//                {

//                    dataStream.Write(byteArray, 0, byteArray.Length);


//                    using (WebResponse tResponse = tRequest.GetResponse())
//                    {

//                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
//                        {

//                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
//                            {

//                                String sResponseFromServer = tReader.ReadToEnd();

//                                string str = sResponseFromServer;

//                            }
//                        }
//                    }
//                }
//            }

//            catch (Exception ex)
//            {

//            }
//        }

//        internal List<FetchMessageModel> GetFetchedMessages(string email, int[] userIDs)
//        {
//            List<FetchMessageModel> fm = new List<FetchMessageModel>();
//            FetchMessageModel fmm = new FetchMessageModel();
//            int userID = GetUserID(email);
//            var messages = context.Messages.Where(x => (x.fromUserID == userID && x.fromUserMessageState != ResponseConstant.MESSAGE_FROM_USER_STATE_DELETED) || (x.toUserID == userID && x.toUserMessageState != ResponseConstant.MESSAGE_TO_USER_STATE_DELETED)).ToList();
//            foreach (var id in userIDs)
//            {
//                var m = messages.Where(x => (x.fromUserID == userID && x.toUserID == id) || (x.toUserID == userID && x.fromUserID == id)).ToList();
//                foreach (var mm in m)
//                {
//                    if ((mm.fromUserID == userID && mm.toUserID == id && mm.fromUserMessageState == ResponseConstant.MESSAGE_FROM_USER_STATE_SENT) || (mm.fromUserID == id && mm.toUserID == userID && mm.toUserMessageState == ResponseConstant.MESSAGE_TO_USER_STATE_NONE))
//                    {
//                        fmm = new FetchMessageModel();
//                        fmm.createdAt = mm.createdAt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
//                        fmm.fromUser = GetUserModel(mm.fromUserID);
//                        fmm.messageID = mm.messageID;
//                        fmm.messageText = mm.messageText;
//                        fmm.toUser = GetUserModel(mm.toUserID);
//                        fmm.messageState = ResponseConstant.MESSAGE_TYPE_SENT;
//                        fm.Add(fmm);
//                    }
//                    messages.Remove(mm);
//                }
//            }
//            foreach (var mes in messages)
//            {
//                fmm = new FetchMessageModel();
//                fmm.createdAt = mes.createdAt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
//                fmm.fromUser = GetUserModel(mes.fromUserID);
//                fmm.messageID = mes.messageID;
//                fmm.messageText = mes.messageText;
//                fmm.toUser = GetUserModel(mes.toUserID);
//                switch (mes.fromUserMessageState)
//                {
//                    case ResponseConstant.MESSAGE_FROM_USER_STATE_SENT:
//                        fmm.messageState = ResponseConstant.MESSAGE_TYPE_SENT;
//                        break;

//                    case ResponseConstant.MESSAGE_FROM_USER_STATE_DELIVERED:
//                        fmm.messageState = ResponseConstant.MESSAGE_TYPE_DELIVERED;
//                        break;

//                    case ResponseConstant.MESSAGE_FROM_USER_STATE_SEEN:
//                        fmm.messageState = ResponseConstant.MESSAGE_TYPE_SEEN;
//                        break;

//                    default:
//                        fmm.messageState = ResponseConstant.MESSAGE_TYPE_SENT;
//                        break;
//                }

//                fm.Add(fmm);
//            }
//            return fm;
//        }

//        internal void DeleteConversation(string email, int toUserID)
//        {
//            int fromUserID = GetUserID(email);
//            IList<Message> messages = context.Messages.Where(x => (x.fromUserID == fromUserID && x.toUserID == toUserID) || (x.fromUserID == toUserID && x.toUserID == fromUserID)).ToList();
//            foreach (var m in messages)
//            {
//                if (m.fromUserID == fromUserID)
//                {
//                    m.fromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_DELETED;
//                }
//                else if (m.toUserID == fromUserID)
//                {
//                    m.toUserMessageState = ResponseConstant.MESSAGE_TO_USER_STATE_DELETED;
//                }
//                context.SaveChanges();
//            }
//        }

//        internal void DeleteMessages(string email, int[] messageIDs)
//        {
//            int userID = GetUserID(email);
//            Message m;
//            foreach (var id in messageIDs)
//            {
//                m = new Message();
//                m = context.Messages.FirstOrDefault(x => x.messageID == id);
//                if (m.fromUserID == userID)
//                {
//                    if (m.fromUserMessageState != ResponseConstant.MESSAGE_FROM_USER_STATE_DELETED)
//                    {
//                        m.fromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_DELETED;
//                    }

//                }
//                else if (m.toUserID == userID)
//                {
//                    if (m.toUserMessageState != ResponseConstant.MESSAGE_TO_USER_STATE_DELETED)
//                    {
//                        m.toUserMessageState = ResponseConstant.MESSAGE_TO_USER_STATE_DELETED;
//                    }
//                }
//                context.SaveChanges();
//            }

//        }

//        internal bool UserAddedBook(string email, int bookID)
//        {
//            int userID = GetUserID(email);
//            return context.Books.Any(x => x.addedByID == userID && x.bookID == bookID);
//        }

//        public bool IsUserVerified(string email)
//        {
//            return context.Users.Any(x => x.email == email && x.emailVerified == true);
//        }
//        public List<BookTransaction> GetUserTransactions(int userID)
//        {
//            return context.BookTransactions.Where(x => x.fromUserID == userID || x.toUserID == userID).ToList();
//        }

//        internal void UpdateMessageState(int messageID, string email, int messageType)
//        {
//            if (messageType == ResponseConstant.MESSAGE_TYPE_DELIVERED)
//            {
//                Message m = new Message();
//                m = context.Messages.FirstOrDefault(x => x.messageID == messageID);
//                int userID = GetUserID(email);
//                if (m.fromUserID == userID || m.toUserID == userID)
//                {
//                    m.toUserMessageState = ResponseConstant.MESSAGE_TO_USER_STATE_RECIEVED;
//                    m.fromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_DELIVERED;
//                    context.SaveChanges();
//                }
//            }
//            else if (messageType == ResponseConstant.MESSAGE_TYPE_SEEN)
//            {
//                Message m = new Message();
//                m = context.Messages.FirstOrDefault(x => x.messageID == messageID);
//                int userID = GetUserID(email);
//                if (m.fromUserID == userID || m.toUserID == userID)
//                {
//                    m.fromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_SEEN;
//                    context.SaveChanges();
//                }
//            }
//        }

//        internal string UserChangePassword(string email, string password)
//        {
//            User user = new User();
//            user = GetUser(email);
//            user.password = CalculateMD5Hash(password);
//            context.SaveChanges();
//            return user.password;
//        }

//        internal bool IsUserLocationExist(string email)
//        {
//            return context.Users.Any(x => x.email == email && (x.latitude != null && x.longitude != null));
//        }

//        internal void AddInfiltrator(string s)
//        {
//            Infiltrator i = new Infiltrator();
//            i.createdAt = DateTime.Now;
//            i.IPAdress = s;
//            context.Infiltrators.Add(i);
//            context.SaveChanges();
//        }

//        internal void UpdateBookDetails(int bookID, string bookName, string author, int genreCode)
//        {
//            Book b = new Book();
//            b = GetBook(bookID);
//            b.author = author;
//            b.bookName = bookName;
//            b.genreCode = genreCode;
//            context.SaveChanges();
//        }

//        internal void ReportBook(int userID, int bookID, int reportCode, string reportInfo)
//        {
//            ReportBook rb = new ReportBook();
//            rb.createdAt = DateTime.Now;
//            rb.bookID = bookID;
//            rb.reportCode = reportCode;
//            rb.reportInfo = reportInfo;
//            rb.userID = userID;
//            context.ReportBooks.Add(rb);
//            context.SaveChanges();
//        }

//        internal void BlockUser(int fromUserID, int toUserID)
//        {
//            Block b = new Block();
//            b.createdAt = DateTime.Now;
//            b.fromUserID = fromUserID;
//            b.toUserID = toUserID;
//            context.Blocks.Add(b);
//            context.SaveChanges();
//        }

//        internal bool UsersBlocked(int fromUserID, int toUserID)
//        {
//            return context.Blocks.Any(x => (x.fromUserID == fromUserID && x.toUserID == toUserID) || (x.fromUserID == toUserID && x.toUserID == fromUserID));
//        }

//        internal bool UserExist(int userID)
//        {
//            return context.Users.Any(x => x.userID == userID);
//        }
//        internal bool UserExist(string email)
//        {
//            return context.Users.Any(x => x.email == email);
//        }
//        public bool IsUserValid(int userID, string password)
//        {
//            return context.Users.Any(x => x.userID == userID && x.password == password);
//        }
//        public bool IsUserValid(string email, string password)
//        {
//            return context.Users.Any(x => x.email == email && x.password == password);
//        }
//        internal void ReportUser(int fromUserID, int toUserID, int reportCode, string reportInfo)
//        {
//            Report report = new Report();
//            if (string.IsNullOrEmpty(reportInfo))
//            {
//                report.reportInfo = reportInfo;
//            }
//            report.reportInfo = reportInfo;
//            report.createdAt = DateTime.Now;
//            report.fromUserID = fromUserID;
//            report.toUserID = toUserID;
//            report.reportCode = reportCode;
//            context.Reports.Add(report);
//            context.SaveChanges();
//        }

//        internal void AddFeedback(string email, string feedback)
//        {
//            Feedback fb = new Feedback();
//            fb.createdAt = DateTime.Now;
//            fb.isChecked = false;
//            fb.text = feedback;
//            fb.userID = GetUserID(email);
//            context.Feedbacks.Add(fb);
//            context.SaveChanges();
//        }

//        internal string GetUserFcmTokenByEmail(string email)
//        {
//            return context.Users.Where(x => x.email == email).FirstOrDefault().fcmToken;
//        }



//        internal void UpdateUserName(string email, string name)
//        {
//            User user = new User();
//            user = GetUser(email);
//            user.nameSurname = name;
//            context.SaveChanges();
//        }

//        internal void UpdateUserBio(string email, string bio)
//        {
//            User user = new User();
//            user = GetUser(email);
//            user.bio = bio;
//            context.SaveChanges();
//        }

//        internal void UpdateUserLocation(string email, double latidue, double longitude)
//        {
//            User user = new User();
//            user = GetUser(email);
//            user.latitude = latidue;
//            user.longitude = longitude;
//            context.SaveChanges();
//        }



//        public string GetUserFcmToken(int userID)
//        {
//            return context.Users.Where(x => x.userID == userID).FirstOrDefault().fcmToken;
//        }

//        public UserModel GetUserModel(int userID)
//        {
//            UserModel um = new UserModel();
//            User user = new User();
//            user = GetUser(userID);
//            um.ID = user.userID;
//            um.latitude = user.latitude;
//            um.longitude = user.longitude;
//            um.nameSurname = user.nameSurname;
//            um.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//            um.profilePictureURL = user.profilePictureURL;
//            return um;
//        }

//        internal int GetFromUserIDByMessageID(int messageID)
//        {
//            return context.Messages.Where(x => x.messageID == messageID).FirstOrDefault().fromUserID;
//        }

//        public string SanitizeInput(string input)
//        {
//            const string removeChars = "?&^$#@!()+-,:;<>’\'-_*";
//            // specify capacity of StringBuilder to avoid resizing
//            StringBuilder sb = new StringBuilder(input.Length);
//            foreach (char x in input.Where(c => !removeChars.Contains(c)))
//            {
//                sb.Append(x);
//            }
//            return sb.ToString();
//        }

//        public bool IsEmailValid(string email)
//        {
//            try
//            {
//                var addr = new System.Net.Mail.MailAddress(email);
//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        internal int GetUserSharedCount(int uID)
//        {
//            return context.BookTransactions.Where(x => x.fromUserID == uID && x.transactionType == ResponseConstant.TRANSACTION_COME_TO_HAND).Count();
//        }

//        //ÇALIŞMIYOR GEREKLİ Mİ?
//        public bool IsNameSurnameValid(string nameSurname)
//        {
//            return true;
//            //if (Regex.Match(nameSurname, "^[a-zA-ZàáâäãåąčćçęèéêëėğıįìíîïłńòóôöõøşùúûüųūÿýżźñšžÀÁÂÄÃÅĄĆČÇĖĘÈÉÊËĞİÌÍÎÏĮŁŃÒÓÔÖÕØŞÙÚÛÜŲŪŸÝŻŹÑßŒÆŠŽ∂ð ,.'-]{3,70}+$").Success)
//            //{
//            //    return true;
//            //}
//            //return false;
//        }



//        internal List<UserModel> GetUsersBySearchString(string searchString, string email)
//        {
//            List<UserModel> userModels = new List<UserModel>();
//            UserModel um;
//            User user = GetUser(email);
//            var users = context.Users.Where(x => x.userID != user.userID && x.nameSurname.StartsWith(searchString)).Select(i => new
//            {
//                i.userID,
//                i.nameSurname,
//                i.profilePictureThumbnailURL,
//                i.profilePictureURL,
//                i.latitude,
//                i.longitude
//            }).ToList();
//            if (users.Count > 20)
//            {
//                users = users.OrderBy(x => Guid.NewGuid()).Take(20).ToList();
//                foreach (var u in users)
//                {
//                    if (userModels.Any(x => x.ID == u.userID))
//                    {
//                        continue;
//                    }
//                    um = new UserModel();
//                    um.ID = u.userID;
//                    um.latitude = u.latitude;
//                    um.longitude = u.latitude;
//                    um.nameSurname = u.nameSurname;
//                    um.profilePictureThumbnailURL = u.profilePictureThumbnailURL;
//                    um.profilePictureURL = u.profilePictureURL;
//                    userModels.Add(um);
//                }
//            }
//            else
//            {

//                var us = context.Users.Where(x => x.userID != user.userID && x.nameSurname.Contains(searchString)).Select(i => new
//                {
//                    i.userID,
//                    i.nameSurname,
//                    i.profilePictureThumbnailURL,
//                    i.profilePictureURL,
//                    i.latitude,
//                    i.longitude
//                }).ToList();
//                foreach (var u in us)
//                {
//                    if (users.Any(x => x.userID == u.userID))
//                    {
//                        continue;
//                    }
//                    users.Add(u);
//                }
//                if (users.Count > 20)
//                {
//                    users = users.OrderBy(x => Guid.NewGuid()).Take(20).ToList();
//                    foreach (var u in users)
//                    {

//                        um = new UserModel();
//                        um.ID = u.userID;
//                        um.latitude = u.latitude;
//                        um.longitude = u.latitude;
//                        um.nameSurname = u.nameSurname;
//                        um.profilePictureThumbnailURL = u.profilePictureThumbnailURL;
//                        um.profilePictureURL = u.profilePictureURL;
//                        userModels.Add(um);
//                    }
//                }
//                else
//                {
//                    foreach (var u in users)
//                    {
//                        um = new UserModel();
//                        um.ID = u.userID;
//                        um.latitude = u.latitude;
//                        um.longitude = u.latitude;
//                        um.nameSurname = u.nameSurname;
//                        um.profilePictureThumbnailURL = u.profilePictureThumbnailURL;
//                        um.profilePictureURL = u.profilePictureURL;
//                        userModels.Add(um);
//                    }
//                    var tempusers = context.Users.Where(x => x.userID != user.userID).Select(i => new
//                    {
//                        i.userID,
//                        i.nameSurname,
//                        i.profilePictureThumbnailURL,
//                        i.profilePictureURL,
//                        i.latitude,
//                        i.longitude
//                    }).ToList();

//                    Dictionary<int, double> levensteinRatios = new Dictionary<int, double>();
//                    foreach (var u in tempusers)
//                    {
//                        if (!users.Any(x => x.userID == u.userID))
//                        {
//                            double commonCharacters = GetCommonCharacters(searchString, u.nameSurname);
//                            double levDis = DamerauLevenshteinDistance(searchString, u.nameSurname, 10);
//                            double ratio = commonCharacters / levDis;
//                            if (ratio == 0)
//                            {
//                                ratio = 1 / levDis;
//                            }
//                            if (ratio > 0.55)
//                                levensteinRatios.Add(u.userID, ratio);
//                        }

//                    }
//                    var sortedRatios = levensteinRatios.OrderByDescending(x => x.Value);
//                    User tempUser;
//                    foreach (var u in sortedRatios)
//                    {
//                        um = new UserModel();
//                        tempUser = new User();
//                        tempUser = GetUser(u.Key);
//                        um.ID = tempUser.userID;
//                        um.latitude = tempUser.latitude;
//                        um.longitude = tempUser.latitude;
//                        um.nameSurname = tempUser.nameSurname;
//                        um.profilePictureThumbnailURL = tempUser.profilePictureThumbnailURL;
//                        um.profilePictureURL = tempUser.profilePictureURL;
//                        userModels.Add(um);
//                    }
//                }
//            }
//            return userModels;
//        }

//        internal void SendNotification(int bookID, int toUserID, int fromUserID, int notificationType)
//        {

//            string applicationID = ResponseConstant.APPLICATION_ID;

//            string senderId = GetUserFcmToken(fromUserID);

//            string receiverId = GetUserFcmToken(toUserID);
//            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId))
//            {
//                return;
//            }

//            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

//            tRequest.Method = "post";

//            tRequest.ContentType = "application/json";
//            UserModel um = new UserModel();
//            um = GetUserModel(fromUserID);
//            BookModel bm = new BookModel();
//            bm = GetBookModel(bookID);
//            var data = new
//            {

//                to = receiverId,
//                data = new
//                {
//                    fcmDataType = notificationType,
//                    sender = um,
//                    book = bm
//                }

//            };

//            var serializer = new JavaScriptSerializer();

//            var json = serializer.Serialize(data);

//            Byte[] byteArray = Encoding.UTF8.GetBytes(json);

//            tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

//            tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));

//            tRequest.ContentLength = byteArray.Length;


//            using (Stream dataStream = tRequest.GetRequestStream())
//            {

//                dataStream.Write(byteArray, 0, byteArray.Length);


//                using (WebResponse tResponse = tRequest.GetResponse())
//                {

//                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
//                    {

//                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
//                        {

//                            String sResponseFromServer = tReader.ReadToEnd();

//                            string str = sResponseFromServer;

//                        }
//                    }
//                }
//            }

//        }

//        internal int AddMessage(int fromUserID, int toUserID, string message)
//        {
//            Message m = new Message();
//            m.createdAt = DateTime.Now;
//            m.fromUserID = fromUserID;
//            m.toUserID = toUserID;
//            m.fromUserMessageState = ResponseConstant.MESSAGE_FROM_USER_STATE_SENT;
//            m.toUserMessageState = ResponseConstant.MESSAGE_TO_USER_STATE_NONE;
//            m.messageText = message;
//            context.Messages.Add(m);
//            context.SaveChanges();
//            return m.messageID;
//        }

//        internal void UpdateBookOwner(int bookID, int userID)
//        {
//            Book book = new Book();
//            book = GetBook(bookID);
//            book.ownerID = userID;
//            context.SaveChanges();
//        }

//        internal void UpdateBookState(int bookID, int state)
//        {
//            Book book = new Book();
//            book = GetBook(bookID);
//            book.bookState = state;
//            context.SaveChanges();
//        }

//        public List<BookModel> GetUserOnRoadBooks(int uID)
//        {
//            List<BookModel> bms = new List<BookModel>();
//            BookModel bm;
//            Book tempBook;
//            User user;
//            var bookIDs = context.BookTransactions.Where(x => x.toUserID == uID && x.transactionType == ResponseConstant.TRANSACTION_DISPATCH).OrderByDescending(x => x.createdAt).GroupBy(y => y.bookID).Select(i => i.FirstOrDefault()).Select(i => new { i.bookID, i.fromUserID, i.toUserID });
//            foreach (var id in bookIDs)
//            {
//                tempBook = new Book();
//                tempBook = GetBook(id.bookID);
//                if (tempBook.ownerID == id.fromUserID && tempBook.ownerID != uID && tempBook.bookState == ResponseConstant.STATE_ON_ROAD)
//                {
//                    bm = new BookModel();
//                    bm.author = tempBook.author;
//                    bm.bookName = tempBook.bookName;
//                    bm.bookPictureThumbnailURL = tempBook.bookPictureThumbnailURL;
//                    bm.bookPictureURL = tempBook.bookPictureURL;
//                    bm.bookState = tempBook.bookState;
//                    bm.genreCode = tempBook.genreCode;
//                    bm.ID = tempBook.bookID;
//                    user = new User();
//                    user = GetUser(tempBook.ownerID);
//                    bm.owner.ID = user.userID;
//                    bm.owner.latitude = user.latitude;
//                    bm.owner.longitude = user.longitude;
//                    bm.owner.nameSurname = user.nameSurname;
//                    bm.owner.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//                    bm.owner.profilePictureURL = user.profilePictureURL;
//                    bms.Add(bm);
//                }
//            }
//            return bms;
//        }



//        public void UpdateBookPicture(string picturePath, string thumbnailPath, int bookID)
//        {
//            Book b = context.Books.Where(x => x.bookID == bookID).FirstOrDefault();
//            b.bookPictureURL = imageURL + "/BookPictures/" + picturePath;
//            b.bookPictureThumbnailURL = imageURL + "/BookPicturesThumbnails/" + thumbnailPath;
//            context.SaveChanges();
//        }

//        public bool UserExist(string email, string password)
//        {
//            return context.Users.Any(x => x.email == email && x.password == password);
//        }

//        public List<BookModel> GetBooksBySearchString(string searchString, string email)
//        {
//            List<BookModel> bookModels = new List<BookModel>();
//            BookModel bm;
//            User user = GetUser(email);
//            Dictionary<int, double> userDistances = new Dictionary<int, double>();

//            if (context.Books.Any(x => x.ownerID != user.userID && x.author == searchString))
//            {
//                var books = context.Books.Where(x => x.ownerID != user.userID && x.author == searchString).Select(i => new
//                {
//                    i.bookID,
//                    i.bookName,
//                    i.bookPictureThumbnailURL,
//                    i.bookPictureURL,
//                    i.bookState,
//                    i.genreCode,
//                    i.author,
//                    i.ownerID
//                }).OrderBy(x => Guid.NewGuid()).ToList();
//                if (books.Count() > 20)
//                {
//                    books = books.Take(20).ToList();
//                }
//                foreach (var book in books)
//                {
//                    bm = new BookModel();
//                    user = new User();
//                    user = GetUser(book.ownerID);
//                    bm.author = book.author;
//                    bm.bookName = book.bookName;
//                    bm.bookPictureThumbnailURL = book.bookPictureThumbnailURL;
//                    bm.bookPictureURL = book.bookPictureURL;
//                    bm.bookState = book.bookState;
//                    bm.genreCode = book.genreCode;
//                    bm.owner.ID = user.userID;
//                    bm.owner.latitude = user.latitude;
//                    bm.owner.longitude = user.longitude;
//                    bm.owner.nameSurname = user.nameSurname;
//                    bm.owner.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//                    bm.owner.profilePictureURL = user.profilePictureURL;
//                    bookModels.Add(bm);
//                }
//                return bookModels;
//            }
//            else {
//                User tempUser;

//                var books = context.Books.Where(x => x.ownerID != user.userID && (x.bookName.StartsWith(searchString) || x.author.StartsWith(searchString))).Select(i => new
//                {
//                    i.bookID,
//                    i.bookName,
//                    i.bookPictureThumbnailURL,
//                    i.bookPictureURL,
//                    i.bookState,
//                    i.genreCode,
//                    i.author,
//                    i.ownerID
//                }).ToList();
//                if (books.Count >= 20)
//                {
//                    //Konumu null değil ve aranan kelimeyi içeren kitap sayısı 20 ten fazla
//                    if (user.latitude != null)
//                    {
//                        foreach (var book in books)
//                        {
//                            tempUser = new User();
//                            tempUser = GetUser(book.ownerID);
//                            userDistances.Add(book.bookID, GetDistanceBetweenTwoUsers(user.latitude, tempUser.latitude, user.longitude, tempUser.longitude));
//                        }
//                        var sortedUserDistances = userDistances.OrderBy(x => x.Value).Take(20);
//                        Book tempBook;
//                        foreach (var a in sortedUserDistances)
//                        {
//                            tempBook = new Book();
//                            tempBook = GetBook(a.Key);
//                            bm = new BookModel();
//                            bm.author = tempBook.author;
//                            bm.bookName = tempBook.bookName;
//                            bm.bookPictureThumbnailURL = tempBook.bookPictureThumbnailURL;
//                            bm.bookPictureURL = tempBook.bookPictureURL;
//                            bm.bookState = tempBook.bookState;
//                            bm.genreCode = tempBook.genreCode;
//                            bm.ID = tempBook.bookID;
//                            tempUser = new User();
//                            tempUser = GetUser(tempBook.ownerID);
//                            bm.owner.ID = tempUser.userID;
//                            bm.owner.latitude = tempUser.latitude;
//                            bm.owner.longitude = tempUser.longitude;
//                            bm.owner.nameSurname = tempUser.nameSurname;
//                            bm.owner.profilePictureThumbnailURL = tempUser.profilePictureThumbnailURL;
//                            bm.owner.profilePictureURL = tempUser.profilePictureURL;
//                            bookModels.Add(bm);
//                        }
//                        return bookModels;
//                    }
//                    //Aranan kelimeyi içeren kitap sayısı 20 ten fazla ve userın konumu nullsa 
//                    else
//                    {
//                        var tempBooks = books.OrderBy(x => Guid.NewGuid()).Take(20);
//                        Book tempBook;

//                        foreach (var i in tempBooks)
//                        {
//                            tempBook = new Book();
//                            tempBook = GetBook(i.bookID);
//                            bm = new BookModel();
//                            bm.author = tempBook.author;
//                            bm.bookName = tempBook.bookName;
//                            bm.bookPictureThumbnailURL = tempBook.bookPictureThumbnailURL;
//                            bm.bookPictureURL = tempBook.bookPictureURL;
//                            bm.bookState = tempBook.bookState;
//                            bm.genreCode = tempBook.genreCode;
//                            bm.ID = tempBook.bookID;
//                            tempUser = new User();
//                            tempUser = GetUser(tempBook.ownerID);
//                            bm.owner.ID = tempUser.userID;
//                            bm.owner.latitude = tempUser.latitude;
//                            bm.owner.longitude = tempUser.longitude;
//                            bm.owner.nameSurname = tempUser.nameSurname;
//                            bm.owner.profilePictureThumbnailURL = tempUser.profilePictureThumbnailURL;
//                            bm.owner.profilePictureURL = tempUser.profilePictureURL;
//                            bookModels.Add(bm);
//                        }
//                        return bookModels;
//                    }
//                }
//                //aranan stringi içeren kitap sayısı 20 ten fazla değilse containsle ara listeye ekle
//                else
//                {
//                    foreach (var k in context.Books.Where(x => x.ownerID != user.userID && (x.bookName.Contains(searchString) || x.author.Contains(searchString))).Select(i => new
//                    { i.bookID, i.bookName, i.bookPictureThumbnailURL, i.bookPictureURL, i.bookState, i.genreCode, i.author, i.ownerID }))
//                    {
//                        if (!books.Any(x => x.bookID == k.bookID))
//                        {
//                            books.Add(k);
//                        }
//                    }
//                    if (books.Count >= 20)
//                    {
//                        if (user.latitude != null)
//                        {
//                            foreach (var book in books)
//                            {
//                                tempUser = new User();
//                                tempUser = GetUser(book.ownerID);
//                                userDistances.Add(book.bookID, GetDistanceBetweenTwoUsers(user.latitude, tempUser.latitude, user.longitude, tempUser.longitude));

//                            }
//                            var sortedUserDistances = userDistances.OrderBy(x => x.Value).Take(20);
//                            Book tempBook;
//                            foreach (var a in sortedUserDistances)
//                            {
//                                tempBook = new Book();
//                                tempBook = GetBook(a.Key);
//                                bm = new BookModel();
//                                bm.author = tempBook.author;
//                                bm.bookName = tempBook.bookName;
//                                bm.bookPictureThumbnailURL = tempBook.bookPictureThumbnailURL;
//                                bm.bookPictureURL = tempBook.bookPictureURL;
//                                bm.bookState = tempBook.bookState;
//                                bm.genreCode = tempBook.genreCode;
//                                bm.ID = tempBook.bookID;
//                                tempUser = new User();
//                                tempUser = GetUser(tempBook.ownerID);
//                                bm.owner.ID = tempUser.userID;
//                                bm.owner.latitude = tempUser.latitude;
//                                bm.owner.longitude = tempUser.longitude;
//                                bm.owner.nameSurname = tempUser.nameSurname;
//                                bm.owner.profilePictureThumbnailURL = tempUser.profilePictureThumbnailURL;
//                                bm.owner.profilePictureURL = tempUser.profilePictureURL;
//                                bookModels.Add(bm);
//                            }
//                            return bookModels;
//                        }

//                        else
//                        {
//                            var tempBooks = books.OrderBy(x => Guid.NewGuid()).Take(20);
//                            Book tempBook;

//                            foreach (var i in tempBooks)
//                            {
//                                tempBook = new Book();
//                                tempBook = GetBook(i.bookID);
//                                bm = new BookModel();
//                                bm.author = tempBook.author;
//                                bm.bookName = tempBook.bookName;
//                                bm.bookPictureThumbnailURL = tempBook.bookPictureThumbnailURL;
//                                bm.bookPictureURL = tempBook.bookPictureURL;
//                                bm.bookState = tempBook.bookState;
//                                bm.genreCode = tempBook.genreCode;
//                                bm.ID = tempBook.bookID;
//                                tempUser = new User();
//                                tempUser = GetUser(tempBook.ownerID);
//                                bm.owner.ID = tempUser.userID;
//                                bm.owner.latitude = tempUser.latitude;
//                                bm.owner.longitude = tempUser.longitude;
//                                bm.owner.nameSurname = tempUser.nameSurname;
//                                bm.owner.profilePictureThumbnailURL = tempUser.profilePictureThumbnailURL;
//                                bm.owner.profilePictureURL = tempUser.profilePictureURL;
//                                bookModels.Add(bm);
//                            }
//                            return bookModels;
//                        }
//                    }
//                    else
//                    {
//                        Book tempBook;
//                        foreach (var i in books)
//                        {
//                            tempBook = new Book();
//                            tempBook = GetBook(i.bookID);
//                            bm = new BookModel();
//                            bm.author = tempBook.author;
//                            bm.bookName = tempBook.bookName;
//                            bm.bookPictureThumbnailURL = tempBook.bookPictureThumbnailURL;
//                            bm.bookPictureURL = tempBook.bookPictureURL;
//                            bm.bookState = tempBook.bookState;
//                            bm.genreCode = tempBook.genreCode;
//                            bm.ID = tempBook.bookID;
//                            tempUser = new User();
//                            tempUser = GetUser(tempBook.ownerID);
//                            bm.owner.ID = tempUser.userID;
//                            bm.owner.latitude = tempUser.latitude;
//                            bm.owner.longitude = tempUser.longitude;
//                            bm.owner.nameSurname = tempUser.nameSurname;
//                            bm.owner.profilePictureThumbnailURL = tempUser.profilePictureThumbnailURL;
//                            bm.owner.profilePictureURL = tempUser.profilePictureURL;
//                            bookModels.Add(bm);
//                        }
//                        var tempBooks = context.Books.Where(x => x.ownerID != user.userID).Select(i => new
//                        {
//                            i.bookID,
//                            i.bookName,
//                            i.bookPictureThumbnailURL,
//                            i.bookPictureURL,
//                            i.bookState,
//                            i.genreCode,
//                            i.author,
//                            i.ownerID
//                        }).ToList();


//                        Dictionary<int, double> levensteinRatios = new Dictionary<int, double>();
//                        foreach (var book in tempBooks)
//                        {
//                            if (!books.Any(x => x.bookName == book.bookName))
//                            {
//                                double commonCharacters = GetCommonCharacters(searchString, book.bookName);
//                                double levDis = DamerauLevenshteinDistance(searchString, book.bookName, 10);
//                                double ratio = commonCharacters / levDis;
//                                if (ratio == 0)
//                                {
//                                    ratio = 1 / levDis;
//                                }
//                                if (ratio > 0.55)
//                                    levensteinRatios.Add(book.bookID, ratio);
//                            }

//                        }
//                        var sortedRatios = levensteinRatios.OrderByDescending(x => x.Value);


//                        foreach (var i in sortedRatios)
//                        {
//                            tempBook = new Book();
//                            tempBook = GetBook(i.Key);
//                            bm = new BookModel();
//                            bm.author = tempBook.author;
//                            bm.bookName = tempBook.bookName;
//                            bm.bookPictureThumbnailURL = tempBook.bookPictureThumbnailURL;
//                            bm.bookPictureURL = tempBook.bookPictureURL;
//                            bm.bookState = tempBook.bookState;
//                            bm.genreCode = tempBook.genreCode;
//                            bm.ID = tempBook.bookID;
//                            tempUser = new User();
//                            tempUser = GetUser(tempBook.ownerID);
//                            bm.owner.ID = tempUser.userID;
//                            bm.owner.latitude = tempUser.latitude;
//                            bm.owner.longitude = tempUser.longitude;
//                            bm.owner.nameSurname = tempUser.nameSurname;
//                            bm.owner.profilePictureThumbnailURL = tempUser.profilePictureThumbnailURL;
//                            bm.owner.profilePictureURL = tempUser.profilePictureURL;
//                            bookModels.Add(bm);
//                        }
//                        return bookModels;
//                    }

//                }
//            }
//        }
//        public List<BookModel> GetBooksBySearchStringNotPressed(string searchString, string email)
//        {
//            List<BookModel> bookModels = new List<BookModel>();
//            BookModel bm;
//            User user = GetUser(email);
//            Dictionary<int, double> userDistances = new Dictionary<int, double>();

//            if (context.Books.Any(x => x.ownerID != user.userID && x.author == searchString))
//            {
//                var books = context.Books.Where(x => x.ownerID != user.userID && x.author == searchString).Select(i => new
//                {
//                    i.bookID,
//                    i.bookName,
//                    i.bookPictureThumbnailURL,
//                    i.bookPictureURL,
//                    i.bookState,
//                    i.genreCode,
//                    i.author,
//                    i.ownerID
//                }).OrderBy(x => Guid.NewGuid()).ToList();
//                if (books.Count() > 5)
//                {
//                    books = books.Take(5).ToList();
//                }
//                foreach (var book in books)
//                {
//                    bm = new BookModel();
//                    user = new User();
//                    user = GetUser(book.ownerID);
//                    bm.author = book.author;
//                    bm.bookName = book.bookName;
//                    bm.bookPictureThumbnailURL = book.bookPictureThumbnailURL;
//                    bm.bookPictureURL = book.bookPictureURL;
//                    bm.bookState = book.bookState;
//                    bm.genreCode = book.genreCode;
//                    bm.owner.ID = user.userID;
//                    bm.owner.latitude = user.latitude;
//                    bm.owner.longitude = user.longitude;
//                    bm.owner.nameSurname = user.nameSurname;
//                    bm.owner.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//                    bm.owner.profilePictureURL = user.profilePictureURL;
//                    bookModels.Add(bm);
//                }
//                return bookModels;
//            }
//            else {
//                User tempUser;

//                var books = context.Books.Where(x => x.ownerID != user.userID && (x.bookName.StartsWith(searchString) || x.author.StartsWith(searchString))).Select(i => new
//                {
//                    i.bookID,
//                    i.bookName,
//                    i.bookPictureThumbnailURL,
//                    i.bookPictureURL,
//                    i.bookState,
//                    i.genreCode,
//                    i.author,
//                    i.ownerID
//                }).ToList();
//                if (books.Count > 5)
//                {
//                    //Konumu null değil ve aranan kelimeyi içeren kitap sayısı 5 ten fazla
//                    if (user.latitude != null)
//                    {
//                        foreach (var book in books)
//                        {
//                            tempUser = new User();
//                            tempUser = GetUser(book.ownerID);
//                            userDistances.Add(book.bookID, GetDistanceBetweenTwoUsers(user.latitude, tempUser.latitude, user.longitude, tempUser.longitude));
//                        }
//                        var sortedUserDistances = userDistances.OrderBy(x => x.Value).Take(5);
//                        Book tempBook;
//                        foreach (var a in sortedUserDistances)
//                        {
//                            tempBook = new Book();
//                            tempBook = GetBook(a.Key);
//                            bm = new BookModel();
//                            bm.author = tempBook.author;
//                            bm.bookName = tempBook.bookName;
//                            bm.bookPictureThumbnailURL = tempBook.bookPictureThumbnailURL;
//                            bm.bookPictureURL = tempBook.bookPictureURL;
//                            bm.bookState = tempBook.bookState;
//                            bm.genreCode = tempBook.genreCode;
//                            bm.ID = tempBook.bookID;
//                            tempUser = new User();
//                            tempUser = GetUser(tempBook.ownerID);
//                            bm.owner.ID = tempUser.userID;
//                            bm.owner.latitude = tempUser.latitude;
//                            bm.owner.longitude = tempUser.longitude;
//                            bm.owner.nameSurname = tempUser.nameSurname;
//                            bm.owner.profilePictureThumbnailURL = tempUser.profilePictureThumbnailURL;
//                            bm.owner.profilePictureURL = tempUser.profilePictureURL;
//                            bookModels.Add(bm);
//                        }
//                        return bookModels;
//                    }
//                    //Aranan kelimeyi içeren kitap sayısı 5 ten fazla ve userın konumu nullsa 
//                    else
//                    {
//                        var tempBooks = books.OrderBy(x => Guid.NewGuid()).Take(5);
//                        Book tempBook;

//                        foreach (var i in tempBooks)
//                        {
//                            tempBook = new Book();
//                            tempBook = GetBook(i.bookID);
//                            bm = new BookModel();
//                            bm.author = tempBook.author;
//                            bm.bookName = tempBook.bookName;
//                            bm.bookPictureThumbnailURL = tempBook.bookPictureThumbnailURL;
//                            bm.bookPictureURL = tempBook.bookPictureURL;
//                            bm.bookState = tempBook.bookState;
//                            bm.genreCode = tempBook.genreCode;
//                            bm.ID = tempBook.bookID;
//                            tempUser = new User();
//                            tempUser = GetUser(tempBook.ownerID);
//                            bm.owner.ID = tempUser.userID;
//                            bm.owner.latitude = tempUser.latitude;
//                            bm.owner.longitude = tempUser.longitude;
//                            bm.owner.nameSurname = tempUser.nameSurname;
//                            bm.owner.profilePictureThumbnailURL = tempUser.profilePictureThumbnailURL;
//                            bm.owner.profilePictureURL = tempUser.profilePictureURL;
//                            bookModels.Add(bm);
//                        }
//                        return bookModels;
//                    }
//                }
//                //aranan stringi içeren kitap sayısı 5 ten fazla değilse containsle ara listeye ekle
//                else
//                {
//                    foreach (var k in context.Books.Where(x => x.ownerID != user.userID && (x.bookName.Contains(searchString) || x.author.Contains(searchString))).Select(i => new
//                    { i.bookID, i.bookName, i.bookPictureThumbnailURL, i.bookPictureURL, i.bookState, i.genreCode, i.author, i.ownerID }))
//                    {
//                        if (!books.Any(x => x.bookID == k.bookID))
//                        {
//                            books.Add(k);
//                        }
//                    }
//                    if (books.Count > 5)
//                    {
//                        if (user.latitude != null)
//                        {
//                            foreach (var book in books)
//                            {
//                                tempUser = new User();
//                                tempUser = GetUser(book.ownerID);
//                                userDistances.Add(book.bookID, GetDistanceBetweenTwoUsers(user.latitude, tempUser.latitude, user.longitude, tempUser.longitude));

//                            }
//                            var sortedUserDistances = userDistances.OrderBy(x => x.Value).Take(5);
//                            Book tempBook;
//                            foreach (var a in sortedUserDistances)
//                            {
//                                tempBook = new Book();
//                                tempBook = GetBook(a.Key);
//                                bm = new BookModel();
//                                bm.author = tempBook.author;
//                                bm.bookName = tempBook.bookName;
//                                bm.bookPictureThumbnailURL = tempBook.bookPictureThumbnailURL;
//                                bm.bookPictureURL = tempBook.bookPictureURL;
//                                bm.bookState = tempBook.bookState;
//                                bm.genreCode = tempBook.genreCode;
//                                bm.ID = tempBook.bookID;
//                                tempUser = new User();
//                                tempUser = GetUser(tempBook.ownerID);
//                                bm.owner.ID = tempUser.userID;
//                                bm.owner.latitude = tempUser.latitude;
//                                bm.owner.longitude = tempUser.longitude;
//                                bm.owner.nameSurname = tempUser.nameSurname;
//                                bm.owner.profilePictureThumbnailURL = tempUser.profilePictureThumbnailURL;
//                                bm.owner.profilePictureURL = tempUser.profilePictureURL;
//                                bookModels.Add(bm);
//                            }
//                            return bookModels;
//                        }

//                        else
//                        {
//                            var tempBooks = books.OrderBy(x => Guid.NewGuid()).Take(5);
//                            Book tempBook;

//                            foreach (var i in tempBooks)
//                            {
//                                tempBook = new Book();
//                                tempBook = GetBook(i.bookID);
//                                bm = new BookModel();
//                                bm.author = tempBook.author;
//                                bm.bookName = tempBook.bookName;
//                                bm.bookPictureThumbnailURL = tempBook.bookPictureThumbnailURL;
//                                bm.bookPictureURL = tempBook.bookPictureURL;
//                                bm.bookState = tempBook.bookState;
//                                bm.genreCode = tempBook.genreCode;
//                                bm.ID = tempBook.bookID;
//                                tempUser = new User();
//                                tempUser = GetUser(tempBook.ownerID);
//                                bm.owner.ID = tempUser.userID;
//                                bm.owner.latitude = tempUser.latitude;
//                                bm.owner.longitude = tempUser.longitude;
//                                bm.owner.nameSurname = tempUser.nameSurname;
//                                bm.owner.profilePictureThumbnailURL = tempUser.profilePictureThumbnailURL;
//                                bm.owner.profilePictureURL = tempUser.profilePictureURL;
//                                bookModels.Add(bm);
//                            }
//                            return bookModels;
//                        }
//                    }
//                    else
//                    {
//                        Book tempBook;

//                        foreach (var i in books)
//                        {
//                            tempBook = new Book();
//                            tempBook = GetBook(i.bookID);
//                            bm = new BookModel();
//                            bm.author = tempBook.author;
//                            bm.bookName = tempBook.bookName;
//                            bm.bookPictureThumbnailURL = tempBook.bookPictureThumbnailURL;
//                            bm.bookPictureURL = tempBook.bookPictureURL;
//                            bm.bookState = tempBook.bookState;
//                            bm.genreCode = tempBook.genreCode;
//                            bm.ID = tempBook.bookID;
//                            tempUser = new User();
//                            tempUser = GetUser(tempBook.ownerID);
//                            bm.owner.ID = tempUser.userID;
//                            bm.owner.latitude = tempUser.latitude;
//                            bm.owner.longitude = tempUser.longitude;
//                            bm.owner.nameSurname = tempUser.nameSurname;
//                            bm.owner.profilePictureThumbnailURL = tempUser.profilePictureThumbnailURL;
//                            bm.owner.profilePictureURL = tempUser.profilePictureURL;
//                            bookModels.Add(bm);
//                        }
//                        return bookModels;
//                    }

//                }
//            }
//        }
//        public int GetCommonCharacters(string searchString, string bookName)
//        {
//            return bookName.ToLower().Intersect(searchString.ToLower()).Count();
//        }
//        public List<UserModel> GetUsersBySearchStringNotPressed(string searchString, string email)
//        {
//            List<UserModel> userModels = new List<UserModel>();
//            UserModel um;
//            User user = GetUser(email);
//            var users = context.Users.Where(x => x.userID != user.userID && x.nameSurname.StartsWith(searchString)).Select(i => new
//            {
//                i.userID,
//                i.nameSurname,
//                i.profilePictureThumbnailURL,
//                i.profilePictureURL,
//                i.latitude,
//                i.longitude
//            }).ToList();
//            if (users.Count > 5)
//            {
//                users = users.OrderBy(x => Guid.NewGuid()).Take(5).ToList();
//                foreach (var u in users)
//                {
//                    if (userModels.Any(x => x.ID == u.userID))
//                    {
//                        continue;
//                    }
//                    um = new UserModel();
//                    um.ID = u.userID;
//                    um.latitude = u.latitude;
//                    um.longitude = u.latitude;
//                    um.nameSurname = u.nameSurname;
//                    um.profilePictureThumbnailURL = u.profilePictureThumbnailURL;
//                    um.profilePictureURL = u.profilePictureURL;
//                    userModels.Add(um);
//                }
//            }
//            else
//            {
//                var us = context.Users.Where(x => x.userID != user.userID && x.nameSurname.Contains(searchString)).Select(i => new
//                {
//                    i.userID,
//                    i.nameSurname,
//                    i.profilePictureThumbnailURL,
//                    i.profilePictureURL,
//                    i.latitude,
//                    i.longitude
//                }).ToList();
//                foreach (var u in us)
//                {
//                    if (users.Any(x => x.userID == u.userID))
//                    {
//                        continue;
//                    }
//                    users.Add(u);
//                }
//                if (users.Count > 5)
//                {
//                    users = users.OrderBy(x => Guid.NewGuid()).Take(5).ToList();
//                    foreach (var u in users)
//                    {

//                        um = new UserModel();
//                        um.ID = u.userID;
//                        um.latitude = u.latitude;
//                        um.longitude = u.latitude;
//                        um.nameSurname = u.nameSurname;
//                        um.profilePictureThumbnailURL = u.profilePictureThumbnailURL;
//                        um.profilePictureURL = u.profilePictureURL;
//                        userModels.Add(um);
//                    }
//                }
//                else
//                {
//                    foreach (var u in users)
//                    {

//                        um = new UserModel();
//                        um.ID = u.userID;
//                        um.latitude = u.latitude;
//                        um.longitude = u.latitude;
//                        um.nameSurname = u.nameSurname;
//                        um.profilePictureThumbnailURL = u.profilePictureThumbnailURL;
//                        um.profilePictureURL = u.profilePictureURL;
//                        userModels.Add(um);
//                    }
//                }
//            }
//            return userModels;
//        }

//        public User GetUser(string email)
//        {
//            return context.Users.Where(x => x.email == email).FirstOrDefault();
//        }
//        public User GetUser(int userID)
//        {
//            return context.Users.Where(x => x.userID == userID).FirstOrDefault();
//        }
//        public Book GetBook(int ID)
//        {
//            return context.Books.Where(x => x.bookID == ID).FirstOrDefault();
//        }
//        public BookModel GetBookModel(int ID)
//        {
//            BookModel bm = new BookModel();
//            Book book = new Book();
//            book = context.Books.Where(x => x.bookID == ID).FirstOrDefault();
//            bm.author = book.author;
//            bm.bookName = book.bookName;
//            bm.bookPictureThumbnailURL = book.bookPictureThumbnailURL;
//            bm.bookPictureURL = book.bookPictureURL;
//            bm.bookState = book.bookState;
//            bm.genreCode = book.genreCode;
//            bm.ID = book.bookID;
//            User user = new User();
//            user = GetUser(book.ownerID);
//            bm.owner.ID = user.userID;
//            bm.owner.latitude = user.latitude;
//            bm.owner.longitude = user.longitude;
//            bm.owner.nameSurname = user.nameSurname;
//            bm.owner.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//            bm.owner.profilePictureURL = user.profilePictureURL;
//            return bm;
//        }
//        public int GetBookState(int bookID)
//        {
//            return context.Books.Where(x => x.bookID == bookID).FirstOrDefault().bookState;
//        }
//        public void SaveUserProfilePicture(string email, string path, string thumbnailPath)
//        {
//            User user = context.Users.Where(x => x.email == email).FirstOrDefault();
//            user.profilePictureURL = imageURL + "/ProfilePictures/" + path;
//            user.profilePictureThumbnailURL = imageURL + "/ProfilePicturesThumbnails/" + thumbnailPath;
//            context.SaveChanges();
//        }

        
//        public int GetUserID(string email)
//        {
//            return context.Users.Where(x => x.email == email).FirstOrDefault().userID;
//        }

//        internal string GetBookPictureThumbnailURL(int bookID)
//        {
//            string url = context.Books.Where(x => x.bookID == bookID).FirstOrDefault().bookPictureThumbnailURL;
//            if (string.IsNullOrEmpty(url))
//            {
//                return null;
//            }
//            else
//            {
//                return url.Substring(url.LastIndexOf('/') + 1);
//            }
//        }

//        internal string GetBookPictureURL(int bookID)
//        {
//            string url = context.Books.Where(x => x.bookID == bookID).FirstOrDefault().bookPictureURL;
//            if (string.IsNullOrEmpty(url))
//            {
//                return null;
//            }
//            else
//            {
//                return url.Substring(url.LastIndexOf('/') + 1);
//            }
//        }
//        public string GetUserProfilePictureUrl(string email)
//        {
//            string url = context.Users.Where(x => x.email == email).FirstOrDefault().profilePictureURL;
//            if (string.IsNullOrEmpty(url))
//            {
//                return null;
//            }
//            else
//            {
//                return url.Substring(url.LastIndexOf('/') + 1);
//            }
//        }
//        public string GetUserProfilePictureThumbnailUrl(string email)
//        {
//            string url = context.Users.Where(x => x.email == email).FirstOrDefault().profilePictureThumbnailURL;
//            if (string.IsNullOrEmpty(url))
//            {
//                return null;
//            }
//            else
//            {
//                return url.Substring(url.LastIndexOf('/') + 1);
//            }
//        }


//        public void CreateBook(string email, string path, string thumbnailPath, string bookName, string author, int bookState, int genreCode)
//        {
//            bookName = bookName.Replace('_', ' ');
//            author = author.Replace('_', ' ');
//            Book book = new Book();
//            int userID = GetUserID(email);
//            book.ownerID = userID;
//            book.createdAt = DateTime.Now;
//            book.addedByID = userID;
//            book.author = author;
//            book.bookName = bookName;
//            book.genreCode = genreCode;
//            book.bookState = bookState;
//            book.bookPictureURL = imageURL + "/BookPictures/" + path;
//            book.bookPictureThumbnailURL = imageURL + "/BookPicturesThumbnails/" + thumbnailPath;
//            context.Books.Add(book);
//            context.SaveChanges();
//            BookInteraction bi = new BookInteraction();
//            bi.User = GetUser(email);
//            bi.Book = book;
//            bi.interactionType = ResponseConstant.INTERACTION_ADD;
//            bi.createdAt = DateTime.Now;
//            context.BookInteractions.Add(bi);
//            context.SaveChanges();

//            BookInteraction bi2 = new BookInteraction();
//            bi2.Book = book;
//            bi2.User = GetUser(email);
//            bi2.createdAt = DateTime.Now;
//            if (bookState == ResponseConstant.STATE_OPENED_TO_SHARE)
//            {
//                bi2.interactionType = ResponseConstant.INTERACTION_OPEN_TO_SHARE;
//            }
//            else if (bookState == ResponseConstant.STATE_READING)
//            {
//                bi2.interactionType = ResponseConstant.INTERACTION_READ_START;
//            }
//            else
//            {
//                bi2.interactionType = ResponseConstant.INTERACTION_CLOSE_TO_SHARE;
//            }
//            context.BookInteractions.Add(bi2);
//            context.SaveChanges();

//        }



//        public void AddBookInteraction(int bookID, string email, int interactionType)
//        {
//            Book book = GetBook(bookID);
//            BookInteraction bookInteraction = new BookInteraction();
//            if (book.bookState == ResponseConstant.STATE_READING && interactionType != ResponseConstant.INTERACTION_READ_STOP)
//            {
//                bookInteraction.User = GetUser(email);
//                bookInteraction.Book = book;
//                bookInteraction.interactionType = ResponseConstant.INTERACTION_READ_STOP;
//                bookInteraction.createdAt = DateTime.Now;
//                context.BookInteractions.Add(bookInteraction);
//                context.SaveChanges();
//            }
//            bookInteraction = new BookInteraction();
//            bookInteraction.User = GetUser(email);
//            bookInteraction.Book = book;
//            bookInteraction.interactionType = interactionType;
//            bookInteraction.createdAt = DateTime.Now;
//            context.BookInteractions.Add(bookInteraction);

//            context.SaveChanges();

//            if (interactionType == ResponseConstant.INTERACTION_READ_START)
//            {
//                book.bookState = ResponseConstant.STATE_READING;
//            }
//            else if (interactionType == ResponseConstant.INTERACTION_OPEN_TO_SHARE)
//            {
//                book.bookState = ResponseConstant.STATE_OPENED_TO_SHARE;
//            }
//            else if (interactionType == ResponseConstant.INTERACTION_CLOSE_TO_SHARE)
//            {
//                book.bookState = ResponseConstant.STATE_CLOSED_TO_SHARE;
//            }
//            context.Books.Attach(book);
//            var entry = context.Entry(book);
//            entry.Property(e => e.bookState).IsModified = true;
//            context.SaveChanges();

//        }
//        public BookRequest AddBookRequest(int bookID, int fromUserID, int toUserID, int requestType)
//        {
//            BookRequest bookRequest = new BookRequest();
//            bookRequest.fromUserID = fromUserID;
//            bookRequest.toUserID = toUserID;
//            bookRequest.Book = GetBook(bookID);
//            bookRequest.requestType = requestType;
//            bookRequest.createdAt = DateTime.Now;
//            context.BookRequests.Add(bookRequest);
//            context.SaveChanges();
//            return bookRequest;
//        }
//        public void AddBookTransaction(int bookID, int fromUserID, int toUserID, int transactiontype)
//        {
//            BookTransaction bookTransaction = new BookTransaction();
//            bookTransaction.fromUserID = fromUserID;
//            bookTransaction.toUserID = toUserID;
//            bookTransaction.Book = GetBook(bookID);
//            bookTransaction.transactionType = transactiontype;
//            bookTransaction.createdAt = DateTime.Now.AddMilliseconds(10);
//            context.BookTransactions.Add(bookTransaction);
//            context.SaveChanges();
//            if (transactiontype == ResponseConstant.TRANSACTION_COME_TO_HAND)
//            {
//                SendNotification(bookID, fromUserID, toUserID, ResponseConstant.FCM_DATA_TYPE_TRANSACTION_COME_TO_HAND);
//            }
//        }

//        public void AddBookTransaction(int bookID, int fromUserID, int toUserID, int transactiontype, int bookState)
//        {
//            BookTransaction bookTransaction = new BookTransaction();
//            bookTransaction.fromUserID = fromUserID;
//            bookTransaction.toUserID = toUserID;
//            bookTransaction.Book = GetBook(bookID);
//            bookTransaction.Book.bookState = bookState;
//            bookTransaction.transactionType = transactiontype;
//            bookTransaction.createdAt = DateTime.Now.AddMilliseconds(10);
//            context.BookTransactions.Add(bookTransaction);
//            context.SaveChanges();
//            if (transactiontype == ResponseConstant.TRANSACTION_COME_TO_HAND)
//            {
//                SendNotification(bookID, fromUserID, toUserID, ResponseConstant.FCM_DATA_TYPE_TRANSACTION_COME_TO_HAND);
//            }
//        }

//        public int GetUserBookCounter(int userID)
//        {
//            int counter = 1;
//            counter += context.BookTransactions.Where(x => x.transactionType == ResponseConstant.TRANSACTION_COME_TO_HAND && x.fromUserID == userID).Count();
//            counter -= context.BookTransactions.Where(x => x.transactionType == ResponseConstant.TRANSACTION_COME_TO_HAND && x.toUserID == userID).Count();

//            counter -= context.BookTransactions.Where(x => x.transactionType == ResponseConstant.TRANSACTION_LOST && (x.fromUserID == userID || x.toUserID == userID)).Count();
//            return counter;
//        }
//        public int GetUserPoint(int userID)
//        {
//            int point = 0;
//            point += context.BookInteractions.Where(x => x.interactionType == ResponseConstant.INTERACTION_ADD && x.userID == userID).Count() * 2;
//            point += context.BookInteractions.Where(x => x.interactionType == ResponseConstant.INTERACTION_READ_STOP && x.userID == userID).GroupBy(x => x.bookID).Select(x => x.FirstOrDefault()).Count();
//            point += context.BookTransactions.Where(x => x.transactionType == ResponseConstant.TRANSACTION_COME_TO_HAND && x.fromUserID == userID).Count() * 5;
//            point += context.BookTransactions.Where(x => x.transactionType == ResponseConstant.TRANSACTION_COME_TO_HAND && x.toUserID == userID).Count() * 3;

//            point -= context.BookTransactions.Where(x => x.transactionType == ResponseConstant.TRANSACTION_LOST && (x.fromUserID == userID || x.toUserID == userID)).Count() * 20;
//            return point;
//        }
//        public List<BookModel> GetUserCurrentlyReadingBooks(int userID)
//        {
//            List<BookModel> books = new List<BookModel>();
//            BookModel bm;
//            User user = GetUser(userID);
//            UserModel um = new UserModel();
//            um.ID = user.userID;
//            um.nameSurname = user.nameSurname;
//            um.profilePictureURL = user.profilePictureURL;
//            um.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//            um.latitude = user.latitude;
//            um.longitude = user.longitude;

//            var booksCurrentlyReading = context.Books.Where(x => x.bookState == ResponseConstant.STATE_READING && x.ownerID == userID).
//                Select(i => new { i.bookID, i.bookName, i.bookPictureURL, i.bookPictureThumbnailURL, i.author, i.bookState, i.genreCode });
//            foreach (var book in booksCurrentlyReading)
//            {
//                bm = new BookModel();

//                bm.ID = book.bookID;
//                bm.bookName = book.bookName;
//                bm.bookPictureURL = book.bookPictureURL;
//                bm.bookPictureThumbnailURL = book.bookPictureThumbnailURL;
//                bm.author = book.author;
//                bm.bookState = book.bookState;
//                bm.genreCode = book.genreCode;
//                bm.owner = um;
//                books.Add(bm);
//            }
//            return books;
//        }
//        public List<BookModel> GetUserBooksOnHand(int userID)
//        {
//            List<BookModel> books = new List<BookModel>();
//            BookModel bm;
//            User user = GetUser(userID);
//            UserModel um = new UserModel();
//            um.ID = user.userID;
//            um.nameSurname = user.nameSurname;
//            um.profilePictureURL = user.profilePictureURL;
//            um.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//            um.latitude = user.latitude;
//            um.longitude = user.longitude;
//            var booksOnHand = context.Books.Where(x => x.ownerID == userID && x.bookState != ResponseConstant.STATE_READING && x.bookState != ResponseConstant.STATE_LOST).
//                Select(i => new { i.bookID, i.bookName, i.bookPictureURL, i.bookPictureThumbnailURL, i.author, i.bookState, i.genreCode });
//            foreach (var book in booksOnHand)
//            {
//                bm = new BookModel();
//                bm.ID = book.bookID;
//                bm.bookName = book.bookName;
//                bm.bookPictureURL = book.bookPictureURL;
//                bm.bookPictureThumbnailURL = book.bookPictureThumbnailURL;
//                bm.author = book.author;
//                bm.bookState = book.bookState;
//                bm.genreCode = book.genreCode;
//                bm.owner = um;
//                books.Add(bm);
//            }
//            return books;
//        }

//        public List<BookModel> GetUserReadBooks(int userID)
//        {
//            List<BookModel> books = new List<BookModel>();
//            BookModel bm;
//            User user = GetUser(userID);
//            UserModel um = new UserModel();
//            um.ID = user.userID;
//            um.nameSurname = user.nameSurname;
//            um.profilePictureURL = user.profilePictureURL;
//            um.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//            um.latitude = user.latitude;
//            um.longitude = user.longitude;
//            var bookInteractions = context.BookInteractions.Where(x => x.interactionType == ResponseConstant.INTERACTION_READ_STOP && x.userID == userID).
//                Select(i => new { i.bookID, i.Book.bookName, i.Book.bookPictureThumbnailURL, i.Book.bookPictureURL, i.Book.author, i.Book.bookState, i.Book.genreCode }).
//                GroupBy(x => x.bookID).Select(x => x.FirstOrDefault());

//            foreach (var bi in bookInteractions)
//            {
//                bm = new BookModel();
//                bm.ID = bi.bookID;
//                bm.bookName = bi.bookName;
//                bm.bookPictureURL = bi.bookPictureURL;
//                bm.bookPictureThumbnailURL = bi.bookPictureThumbnailURL;
//                bm.author = bi.author;
//                bm.bookState = bi.bookState;
//                bm.genreCode = bi.genreCode;
//                bm.owner = um;
//                books.Add(bm);
//            }
//            return books;
//        }

//        public int GetBookPopularity(string bookName, DateTime dateTime)
//        {
//            int popularity = 0;
//            popularity += context.BookInteractions.Where(x => (x.interactionType == ResponseConstant.INTERACTION_READ_START ||
//            x.interactionType == ResponseConstant.INTERACTION_READ_STOP) && x.Book.bookName == bookName && x.createdAt > dateTime).GroupBy(x => x.userID).Select(x => x.FirstOrDefault()).Count();

//            return popularity;
//        }
//        public Book GetBookByBookName(string name)
//        {
//            return context.Books.Where(x => x.bookName == name).FirstOrDefault();
//        }

//        public double GetDistanceBetweenTwoUsers(double? lat1, double? lat2, double? lon1, double? lon2)
//        {
//            if (!lat2.HasValue && !lon2.HasValue)
//            {
//                return 999;
//            }
//            var p = 0.017453292519943295;    // Math.PI / 180
//            var a = 0.5 - Math.Cos((lat2.Value - lat1.Value) * p) / 2 +
//                    Math.Cos(lat1.Value * p) * Math.Cos(lat2.Value * p) *
//                    (1 - Math.Cos((lon2.Value - lon1.Value) * p)) / 2;

//            return 12742 * Math.Asin(Math.Sqrt(a)); // 2 * R; R = 6371 km
//        }
//        public List<Book> GetBooksByGenre(int genreCode, int userID)
//        {
//            List<Book> books = context.Books.Where(x => x.genreCode == genreCode && x.ownerID != userID).ToList();
//            return books;
//        }
//        public List<BookModel> GetBookModelsByGenre(int genreCode, string email)
//        {
//            List<BookModel> bookModels = new List<BookModel>();
//            User user = new User();
//            user = GetUser(email);
//            BookModel bm;
//            var books = context.Books.Where(x => x.ownerID != user.userID && x.genreCode == genreCode).Select(i => new
//            {
//                i.bookID,
//                i.bookName,
//                i.bookPictureThumbnailURL,
//                i.bookPictureURL,
//                i.bookState,
//                i.genreCode,
//                i.author,
//                i.ownerID
//            });
//            if (books.Count() > 20)
//            {
//                books = books.OrderBy(x => Guid.NewGuid()).Take(5);
//            }
//            foreach (var book in books)
//            {
//                user = new User();
//                user = GetUser(book.ownerID);
//                bm = new BookModel();
//                bm.author = book.author;
//                bm.bookName = book.bookName;
//                bm.bookPictureThumbnailURL = book.bookPictureThumbnailURL;
//                bm.bookPictureURL = book.bookPictureURL;
//                bm.bookState = book.bookState;
//                bm.genreCode = book.genreCode;
//                bm.ID = book.bookID;
//                bm.owner.ID = user.userID;
//                bm.owner.latitude = user.latitude;
//                bm.owner.longitude = user.longitude;
//                bm.owner.nameSurname = user.nameSurname;
//                bm.owner.profilePictureURL = user.profilePictureURL;
//                bm.owner.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//                bookModels.Add(bm);
//            }
//            return bookModels;
//        }
//        public Book GetRandomBook()
//        {
//            Random random = new Random();
//            int index = random.Next(0, context.Books.Count() - 1);
//            var a = context.Books.AsEnumerable().ElementAtOrDefault(index);

//            return a;
//        }
//        public List<BookInteractionModel> GetBookInteractions(int bookID)
//        {
//            var bookInteractions = context.BookInteractions.Where(x => x.bookID == bookID).Select(i => new { i.createdAt, i.userID, i.interactionType }).ToList();
//            List<BookInteractionModel> bims = new List<BookInteractionModel>();
//            BookInteractionModel bim;
//            User user;
//            foreach (var bi in bookInteractions)
//            {
//                bim = new BookInteractionModel();
//                bim.createdAt = bi.createdAt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
//                bim.interactionType = bi.interactionType;
//                user = new User();
//                user = GetUser(bi.userID);
//                bim.user.ID = user.userID;
//                bim.user.latitude = user.latitude;
//                bim.user.longitude = user.longitude;
//                bim.user.nameSurname = user.nameSurname;
//                bim.user.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//                bim.user.profilePictureURL = user.profilePictureURL;
//                bims.Add(bim);
//            }
//            return bims;
//        }
//        public List<BookRequestModel> GetBookRequests(int bookID)
//        {
//            var bookRequests = context.BookRequests.Where(x => x.bookID == bookID).Select(i => new { i.requestType, i.createdAt, i.fromUserID, i.toUserID }).ToList();
//            List<BookRequestModel> brms = new List<BookRequestModel>();
//            BookRequestModel brm;
//            User user;
//            foreach (var br in bookRequests)
//            {
//                brm = new BookRequestModel();
//                brm.createdAt = br.createdAt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
//                brm.requestType = br.requestType;

//                user = new User();
//                user = GetUser(br.fromUserID);
//                brm.fromUser.ID = user.userID;
//                brm.fromUser.latitude = user.latitude;
//                brm.fromUser.longitude = user.longitude;
//                brm.fromUser.nameSurname = user.nameSurname;
//                brm.fromUser.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//                brm.fromUser.profilePictureURL = user.profilePictureURL;

//                user = new User();
//                user = GetUser(br.toUserID);
//                brm.toUser.ID = user.userID;
//                brm.toUser.latitude = user.latitude;
//                brm.toUser.longitude = user.longitude;
//                brm.toUser.nameSurname = user.nameSurname;
//                brm.toUser.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//                brm.toUser.profilePictureURL = user.profilePictureURL;
//                brms.Add(brm);
//            }
//            return brms;
//        }
//        public List<BookRequestModel> GetBookRequests(int bookID, string email)
//        {
//            int userID = GetUserID(email);
//            var request = context.BookRequests.Where(x => x.bookID == bookID && x.toUserID == userID && x.requestType == ResponseConstant.REQUEST_ACCEPT).OrderByDescending(x => x.createdAt).FirstOrDefault();

//            if (request == null)
//            {
//                request = new BookRequest();
//                request.createdAt = new DateTime(2017, 01, 01);
//            }

//            var bookSentRequests = context.BookRequests.Where(x => x.bookID == bookID && x.toUserID == userID &&
//            x.requestType == ResponseConstant.REQUEST_SENT && x.createdAt > request.createdAt).Select(i => new
//            {
//                i.requestType,
//                i.createdAt,
//                i.fromUserID,
//                i.toUserID
//            }).ToList();

//            var bookAnsweredRequests = context.BookRequests.Where(x => x.bookID == bookID && x.fromUserID == userID &&
//                x.requestType != ResponseConstant.REQUEST_SENT && x.createdAt > request.createdAt).Select(i => new
//                {
//                    i.requestType,
//                    i.createdAt,
//                    i.fromUserID,
//                    i.toUserID
//                }).ToList();

//            List<int> userIDs = new List<int>();
//            foreach (var ar in bookAnsweredRequests)
//            {
//                if (bookSentRequests.Any(x => x.fromUserID == ar.toUserID))
//                {
//                    var a = bookSentRequests.Where(x => x.fromUserID == ar.toUserID).FirstOrDefault();
//                    bookSentRequests.Remove(a);
//                    bookSentRequests.Add(ar);
//                }
//            }

//            List<BookRequestModel> brms = new List<BookRequestModel>();
//            BookRequestModel brm;
//            User user;
//            foreach (var br in bookSentRequests)
//            {
//                if(br.requestType == ResponseConstant.REQUEST_SENT)
//                {
//                    if (GetUserBookCounter(br.fromUserID) <= 0)
//                    {
//                        continue;
//                    }
//                }
//                brm = new BookRequestModel();
//                brm.createdAt = br.createdAt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
//                brm.requestType = br.requestType;
               
//                user = new User();
//                user = GetUser(br.fromUserID);
//                brm.fromUser.ID = user.userID;
//                brm.fromUser.latitude = user.latitude;
//                brm.fromUser.longitude = user.longitude;
//                brm.fromUser.nameSurname = user.nameSurname;
//                brm.fromUser.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//                brm.fromUser.profilePictureURL = user.profilePictureURL;

//                user = new User();
//                user = GetUser(br.toUserID);
//                brm.toUser.ID = user.userID;
//                brm.toUser.latitude = user.latitude;
//                brm.toUser.longitude = user.longitude;
//                brm.toUser.nameSurname = user.nameSurname;
//                brm.toUser.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//                brm.toUser.profilePictureURL = user.profilePictureURL;
//                brms.Add(brm);
//            }
//            return brms;
//        }

//        public List<BookTransactionModel> GetBookTransactions(int bookID)
//        {
//            var bookTransactions = context.BookTransactions.Where(x => x.bookID == bookID).Select(i => new { i.createdAt, i.fromUserID, i.toUserID, i.transactionType }).ToList();
//            List<BookTransactionModel> btms = new List<BookTransactionModel>();
//            BookTransactionModel btm;
//            User user;
//            foreach (var bi in bookTransactions)
//            {
//                btm = new BookTransactionModel();
//                btm.createdAt = bi.createdAt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
//                btm.transactionType = bi.transactionType;

//                user = new User();
//                user = GetUser(bi.fromUserID);
//                btm.fromUser.ID = user.userID;
//                btm.fromUser.latitude = user.latitude;
//                btm.fromUser.longitude = user.longitude;
//                btm.fromUser.nameSurname = user.nameSurname;
//                btm.fromUser.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//                btm.fromUser.profilePictureURL = user.profilePictureURL;

//                user = new User();
//                user = GetUser(bi.toUserID);
//                btm.toUser.ID = user.userID;
//                btm.toUser.latitude = user.latitude;
//                btm.toUser.longitude = user.longitude;
//                btm.toUser.nameSurname = user.nameSurname;
//                btm.toUser.profilePictureThumbnailURL = user.profilePictureThumbnailURL;
//                btm.toUser.profilePictureURL = user.profilePictureURL;
//                btms.Add(btm);
//            }
//            return btms;
//        }
//        public int DamerauLevenshteinDistance(string source, string target, int threshold)
//        {

//            int length1 = source.Length;
//            int length2 = target.Length;

//            // Return trivial case - difference in string lengths exceeds threshhold
//            if (Math.Abs(length1 - length2) > threshold) { return int.MaxValue; }

//            // Ensure arrays [i] / length1 use shorter length 
//            if (length1 > length2)
//            {
//                Swap(ref target, ref source);
//                Swap(ref length1, ref length2);
//            }

//            int maxi = length1;
//            int maxj = length2;

//            int[] dCurrent = new int[maxi + 1];
//            int[] dMinus1 = new int[maxi + 1];
//            int[] dMinus2 = new int[maxi + 1];
//            int[] dSwap;

//            for (int i = 0; i <= maxi; i++) { dCurrent[i] = i; }

//            int jm1 = 0, im1 = 0, im2 = -1;

//            for (int j = 1; j <= maxj; j++)
//            {

//                // Rotate
//                dSwap = dMinus2;
//                dMinus2 = dMinus1;
//                dMinus1 = dCurrent;
//                dCurrent = dSwap;

//                // Initialize
//                int minDistance = int.MaxValue;
//                dCurrent[0] = j;
//                im1 = 0;
//                im2 = -1;

//                for (int i = 1; i <= maxi; i++)
//                {

//                    int cost = source[im1] == target[jm1] ? 0 : 1;

//                    int del = dCurrent[im1] + 1;
//                    int ins = dMinus1[i] + 1;
//                    int sub = dMinus1[im1] + cost;

//                    //Fastest execution for min value of 3 integers
//                    int min = (del > ins) ? (ins > sub ? sub : ins) : (del > sub ? sub : del);

//                    if (i > 1 && j > 1 && source[im2] == target[jm1] && source[im1] == target[j - 2])
//                        min = Math.Min(min, dMinus2[im2] + cost);

//                    dCurrent[i] = min;
//                    if (min < minDistance) { minDistance = min; }
//                    im1++;
//                    im2++;
//                }
//                jm1++;
//                if (minDistance > threshold) { return int.MaxValue; }
//            }

//            int result = dCurrent[maxi];
//            return (result > threshold) ? int.MaxValue : result;
//        }
//        void Swap<T>(ref T arg1, ref T arg2)
//        {
//            T temp = arg1;
//            arg1 = arg2;
//            arg2 = temp;
//        }
//        public UserDetailsModel GetUserDetailsModel(int userID)
//        {
//            UserDetailsModel ud = new UserDetailsModel();
//            User u = new User();
//            u = GetUser(userID);
//            ud.bio = u.bio;
//            ud.counter = GetUserBookCounter(userID);
//            ud.email = u.email;
//            ud.emailVerified = u.emailVerified;
//            ud.ID = u.userID;
//            ud.latitude = u.latitude;
//            ud.longitude = u.longitude;
//            ud.nameSurname = u.nameSurname;
//            ud.password = u.password;
//            ud.point = GetUserPoint(userID);
//            ud.profilePictureThumbnailURL = u.profilePictureThumbnailURL;
//            ud.profilePictureURL = u.profilePictureURL;
//            ud.shared = GetUserSharedCount(userID);
//            return ud;
//        }
//        public UserDetailsModel GetUserDetailsModel(User u)
//        {
//            UserDetailsModel ud = new UserDetailsModel();
//            ud.bio = u.bio;
//            ud.counter = GetUserBookCounter(u.userID);
//            ud.email = u.email;
//            ud.emailVerified = u.emailVerified;
//            ud.ID = u.userID;
//            ud.latitude = u.latitude;
//            ud.longitude = u.longitude;
//            ud.nameSurname = u.nameSurname;
//            ud.password = u.password;
//            ud.point = GetUserPoint(u.userID);
//            ud.profilePictureThumbnailURL = u.profilePictureThumbnailURL;
//            ud.profilePictureURL = u.profilePictureURL;
//            ud.shared = GetUserSharedCount(u.userID);
//            ud.createdAt = u.createdAt;
//            return ud;
//        }
//        internal List<UserDetailsModel> GetAllUserDetails()
//        {
//            List<UserDetailsModel> uds = new List<UserDetailsModel>();
//            foreach (var u in context.Users)
//            {
//                uds.Add(GetUserDetailsModel(u));
//            }
//            return uds;
//        }

//        public List<BookRequest> GetUserRequests(int userID)
//        {
//            List<BookRequest> br = new List<BookRequest>();
//            br.AddRange(context.BookRequests.Where(x => x.fromUserID == userID && x.requestType == ResponseConstant.REQUEST_SENT));
//            br.AddRange(context.BookRequests.Where(x => x.toUserID == userID && x.requestType != ResponseConstant.REQUEST_SENT));
//            return br;
//        }

//    }
//    public class TupleList<T1, T2, T3> : List<Tuple<T1, T2, T3>>
//    {
//        public void Add(T1 item, T2 item2, T3 item3)
//        {
//            Add(new Tuple<T1, T2, T3>(item, item2, item3));
//        }
//    }
//}
