using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Constants
{
    public class ResponseConstant
    {
        public const string APPLICATION_ID = "AAAACNMFGAM:APA91bHfBhJdOCFQ04xcPsQ3H-Ez9_HTckKj5Fj7tX8xru0LaIdgZB3znm04nxEDMaDRazuaf2ZDqTWg1sEygsAbG8ly_7JVgeYjt3WKaHa130IAPNMyK_9ZvZis1SgLrspEM9DU2Rno";
        // Book interaction types
        public const int INTERACTION_ADD = 0;
        public const int INTERACTION_READ_START = 1;
        public const int INTERACTION_READ_STOP = 2;
        public const int INTERACTION_OPEN_TO_SHARE = 3;
        public const int INTERACTION_CLOSE_TO_SHARE = 4;

        // Book request types
        public const int REQUEST_SENT = 5;
        public const int REQUEST_ACCEPT = 6;
        public const int REQUEST_REJECT = 7;

        // Book transaction types
        public const int TRANSACTION_DISPATCH = 8;
        public const int TRANSACTION_COME_TO_HAND = 9;
        public const int TRANSACTION_LOST = 10;

        

        // Book situation types for convert process types to useable information for application
        public const int STATE_READING = 0;
        public const int STATE_OPENED_TO_SHARE = 1;
        public const int STATE_CLOSED_TO_SHARE = 2;
        public const int STATE_ON_ROAD = 3;
        public const int STATE_LOST = 4;

        // URLS
        public const string IMAGES_FOLDER_URL = "~/Images";

        // Input limitations
        public const int PASSWORD_LENGTH_MIN = 6;
        public const int PASSWORD_LENGTH_MAX = 128;

        // Error codes
        public const int ERROR_EMPTY_POST = 0;
        public const int ERROR_MISSING_POST_ELEMENT = 1;
        public const int ERROR_INVALID_EMAIL = 2;
        public const int ERROR_INVALID_NAME_SURNAME = 3;
        public const int ERROR_LOCATION_NOT_FOUND = 4;
        public const int ERROR_EMAIL_TAKEN = 5;
        public const int ERROR_SHORT_PASSWORD = 6;
        public const int ERROR_LONG_PASSWORD = 7;
        public const int ERROR_FALSE_COMBINATION = 8;
        public const int ERROR_QUERY = 9;
        public const int ERROR_EMAIL_NOT_FOUND = 10;
        public const int ERROR_INVALID_REQUEST = 11;
        public const int ERROR_VERIFICATION_HASH_NOT_FOUND = 12;
        public const int ERROR_BOOK_COUNT_INSUFFICIENT = 13;
        public const int ERROR_USER_NOT_VERIFIED = 14;
        public const int ERROR_USER_BLOCKED = 15;
        public const int ERROR_USER_ALREADY_VERIFIED = 16;
        public const int ERROR_UNKNOWN = -1;
        public const int ERROR_NONE = -99;

        //Fcm types
        public const int FCM_DATA_TYPE_SENT_MESSAGE = 0;
        public const int FCM_DATA_TYPE_DELIVERED_MESSAGE = 1;
        public const int FCM_DATA_TYPE_SEEN_MESSAGE = 2;
        public const int FCM_DATA_TYPE_REQUEST_SENT = 3;
        public const int FCM_DATA_TYPE_REQUEST_REJECTED = 4;
        public const int FCM_DATA_TYPE_REQUEST_ACCEPTED = 5;
        public const int FCM_DATA_TYPE_TRANSACTION_COME_TO_HAND = 6;
        public const int FCM_DATA_TYPE_TRANSACTION_LOST = 7;
        public const int FCM_DATA_TYPE_USER_VERIFIED = 8;

        //Message types
        public const int MESSAGE_TYPE_PENDING = 0;
        public const int MESSAGE_TYPE_SENT = 1;
        public const int MESSAGE_TYPE_DELIVERED = 2;
        public const int MESSAGE_TYPE_SEEN = 3;
        public const int MESSAGE_TYPE_ERROR = 4;

        //From User Message states
        public const int MESSAGE_FROM_USER_STATE_SENT = 0;
        public const int MESSAGE_FROM_USER_STATE_DELIVERED = 1;
        public const int MESSAGE_FROM_USER_STATE_DELETED = 2;
        public const int MESSAGE_FROM_USER_STATE_SEEN = 3;

        //To User Message states
        public const int MESSAGE_TO_USER_STATE_NONE = 0;
        public const int MESSAGE_TO_USER_STATE_RECIEVED = 1;
        public const int MESSAGE_TO_USER_STATE_DELETED = 2;        

        //Sizes
        public const int GENRE_COUNT = 32;
        public const int TOTAL_TIMELINE_LIST_SIZE = 20;
        public const int MIN_DISTANCE = 50;
    }
}