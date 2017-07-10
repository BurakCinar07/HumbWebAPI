using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Humb.Core.Constants
{
    public class InfiltratorConstant
    {
        //Injection Warning
        public const int ERROR_INJECTION = -100;
        //All the post parameters are missing
        public const int ERROR_ALL_KEYS_MISSING = 0;
        //Given keys count not inconsistent
        public const int ERROR_GIVEN_KEYS_COUNT_INCONSISTENT = 1;
        //Given keys not exists in post object
        public const int ERROR_GIVEN_KEY_NOT_EXISTS = 2;
        //Given keys order not inconsistent
        public const int ERROR_GIVEN_KEYS_ORDER_INCONSISTENT = 3;
        //The value of given parameters is null or empty
        public const int ERROR_POST_VALUE_NULL_OR_EMPTY = 4;
        //The given email is not valid
        public const int ERROR_EMAIL_NOT_VALID = 5;
        //There is no user with given email and password
        public const int ERROR_EMAIL_PASSWORD_INCONSISTENT = 6;
        //User is not verified
        public const int WARNING_USER_NOT_VERIFIED = 7;
        //User have not any location info
        public const int WARNING_USER_NOT_HAVE_LOCATION = 8;
    }
}