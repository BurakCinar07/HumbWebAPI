using BookieAPI.Controllers.Utils.ModelUtils;
using BookieAPI.Models.Context;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using BookieAPI.Constants;
using BookieAPI.Controllers.Utils;

namespace BookieAPI.Filters.ErrorHandlers
{
    public class ErrorHandler
    {

        public static PostErrorCase WithPost(Context context, JObject jObject, params string[] keys)
        {
            return new PostErrorCase(context, jObject, keys);
        }

        public static GetErrorCase WithGet(Context context, params string[] keys)
        {
            return new GetErrorCase(context, keys);
        }

        public class PostErrorCase
        {
            public Context context;
            public JObject postObject;
            public List<string> keys;

            public const int INDEX_EMAIL = 0;
            public const int INDEX_PASSWORD = 1;

            public int errorCode;

            public event EventHandler Error;
            public event EventHandler Success;

            public PostErrorCase(Context context, JObject jObject, params string[] keys)
            {
                this.context = context;
                this.postObject = jObject;
                this.keys = new List<string>(keys);

                errorCode = ResponseConstant.ERROR_NONE;
            }

            public PostErrorCase isKeysNull()
            {
                if (errorCode == ResponseConstant.ERROR_NONE)
                {
                    if(postObject.Count == 0)
                    {
                        InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_ALL_KEYS_MISSING, "All the post parameters are missing");
                        errorCode = InfiltratorConstant.ERROR_INJECTION;
                        return this;
                    }
                    else if (postObject.Count != keys.Count)
                    {
                        InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_GIVEN_KEYS_COUNT_INCONSISTENT, "Given keys count not inconsistent");
                        errorCode = InfiltratorConstant.ERROR_INJECTION;
                        return this;
                    }
                    else
                    {
                        int index = 0;
                        foreach(var p in postObject)
                        {
                            if (!keys.Contains(p.Key))
                            {
                                InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_GIVEN_KEY_NOT_EXISTS, "Given keys not exists in post object");
                                errorCode = InfiltratorConstant.ERROR_INJECTION;
                                return this;
                            }                            
                            else if (!keys[index].Equals(p.Key))
                            {
                                InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_GIVEN_KEYS_ORDER_INCONSISTENT, "Given keys order inconsistent");
                                errorCode = InfiltratorConstant.ERROR_INJECTION;
                                return this;
                            }
                            index++;
                        }                     

                    }
                }
                return this;
            }

            public PostErrorCase isValuesNullOrEmpty()
            {
                if (errorCode == ResponseConstant.ERROR_NONE)
                {
                    foreach (string key in keys)
                    {
                        if (string.IsNullOrEmpty(postObject[key].ToString()))
                        {
                            InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_POST_VALUE_NULL_OR_EMPTY, "The value of given parameters is null or empty");
                            errorCode = InfiltratorConstant.ERROR_INJECTION;
                        }
                    }
                }
                return this;
            }

            public PostErrorCase isValuesNullOrEmpty(params string[] exculudedKeys)
            {
                List<string> eKeys = new List<string>(exculudedKeys);
                if (errorCode == ResponseConstant.ERROR_NONE)
                {
                    foreach (string key in keys)
                    {
                        if (!eKeys.Contains(key) && string.IsNullOrEmpty(postObject[key].ToString()))
                        {
                            InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_POST_VALUE_NULL_OR_EMPTY, "The value of given parameters is null or empty");
                            errorCode = InfiltratorConstant.ERROR_INJECTION;
                        }
                    }
                }
                return this;
            }

            public PostErrorCase isEmailValid()
            {
                if (errorCode == ResponseConstant.ERROR_NONE)
                {
                    string email = postObject[keys[INDEX_EMAIL]].ToString();
                    if (!TextUtils.IsEmailValid(email))
                    {
                        InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_EMAIL_NOT_VALID, "The given email is not valid");
                        errorCode = InfiltratorConstant.ERROR_INJECTION;
                    }
                }               
                return this;
            }

            public PostErrorCase isUserExist()
            {
                if (errorCode == ResponseConstant.ERROR_NONE)
                {
                    string email = postObject[keys[INDEX_EMAIL]].ToString();
                    string password = postObject[keys[INDEX_PASSWORD]].ToString(); 

                    password = TextUtils.SanitizeInput(password);

                    if (!UserUtils.UserExist(context, email, password))
                    {
                        InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_EMAIL_PASSWORD_INCONSISTENT, "There is no user with given email and password");
                        errorCode = InfiltratorConstant.ERROR_INJECTION;
                    }
                }

                return this;
            }

            public PostErrorCase isUserVerified()
            {
                if (errorCode == ResponseConstant.ERROR_NONE)
                {
                    string email = postObject[keys[INDEX_EMAIL]].ToString();
                    string password = postObject[keys[INDEX_PASSWORD]].ToString();

                    if (!UserUtils.IsUserVerified(context, email))
                    {
                        InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.WARNING_USER_NOT_VERIFIED, "User is not verified");
                        errorCode = ResponseConstant.ERROR_USER_NOT_VERIFIED;
                    }
                }

                return this;
            }

            public PostErrorCase isUserLocationExist()
            {
                if (errorCode == ResponseConstant.ERROR_NONE)
                {
                    string email = postObject[keys[INDEX_EMAIL]].ToString();
                    string password = postObject[keys[INDEX_PASSWORD]].ToString();

                    if (!UserUtils.IsUserLocationExist(context, email))
                    {
                        InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.WARNING_USER_NOT_HAVE_LOCATION, "User have not any location info");
                        errorCode = ResponseConstant.ERROR_LOCATION_NOT_FOUND;
                    }
                }

                return this;
            }

            public PostErrorCase addOnErrorListener(EventHandler eventHandler)
            {
                Error += eventHandler;
                return this;
            }

            protected virtual void OnError(ErrorEventArgs e)
            {
                if (Error != null)
                    Error(this, e);
            }

            public PostErrorCase addOnSuccessListener(EventHandler eventHandler)
            {
                Success += eventHandler;
                return this;
            }

            protected virtual void OnSuccess(SuccessPostEventArgs e)
            {
                if (Success != null)
                    Success(this, e);
            }

            public void Check()
            {
                if(errorCode != ResponseConstant.ERROR_NONE)
                {
                    OnError(new ErrorEventArgs(errorCode));
                }
                else
                {
                    OnSuccess(new SuccessPostEventArgs(postObject));
                }
            }
        }

        public class GetErrorCase
        {
            public Context context;
            public List<string> keys;

            public const int INDEX_EMAIL = 0;
            public const int INDEX_PASSWORD = 1;

            public int errorCode;

            public event EventHandler Error;
            public event EventHandler Success;

            public GetErrorCase(Context context, params string[] keys)
            {
                this.context = context;
                this.keys = new List<string>(keys);

                errorCode = ResponseConstant.ERROR_NONE;
            }

            public GetErrorCase isValuesNullOrEmpty()
            {
                if (errorCode == ResponseConstant.ERROR_NONE)
                {
                    foreach (string key in keys)
                    {
                        if (string.IsNullOrEmpty(key.ToString()))
                        {
                            InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_POST_VALUE_NULL_OR_EMPTY, "All the post parameters are missing");
                            errorCode = InfiltratorConstant.ERROR_INJECTION;
                        }
                    }
                }
                return this;
            }

            public GetErrorCase isEmailValid()
            {
                if (errorCode == ResponseConstant.ERROR_NONE)
                {
                    string email = keys[INDEX_EMAIL].ToString();
                    if (!TextUtils.IsEmailValid(email))
                    {
                        InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_EMAIL_NOT_VALID, "The given email is not valid");
                        errorCode = InfiltratorConstant.ERROR_INJECTION;
                    }
                }

                return this;
            }

            public GetErrorCase isUserExist()
            {
                if (errorCode == ResponseConstant.ERROR_NONE)
                {
                    string email = keys[INDEX_EMAIL].ToString();
                    string password = keys[INDEX_PASSWORD].ToString();

                    password = TextUtils.SanitizeInput(password);

                    if (!UserUtils.UserExist(context, email, password))
                    {
                        InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.ERROR_EMAIL_PASSWORD_INCONSISTENT, "There is no user with given email and password");
                        errorCode = InfiltratorConstant.ERROR_INJECTION;
                    }
                }

                return this;
            }

            public GetErrorCase isUserVerified()
            {
                if (errorCode == ResponseConstant.ERROR_NONE)
                {
                    string email = keys[INDEX_EMAIL].ToString();
                    string password = keys[INDEX_PASSWORD].ToString();

                    if (!UserUtils.IsUserVerified(context, email))
                    {
                        InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.WARNING_USER_NOT_VERIFIED, "User is not verified");
                        errorCode = ResponseConstant.ERROR_USER_NOT_VERIFIED;
                    }
                }

                return this;
            }

            public GetErrorCase isUserLocationExist()
            {
                if (errorCode == ResponseConstant.ERROR_NONE)
                {
                    string email = keys[INDEX_EMAIL].ToString();
                    string password = keys[INDEX_PASSWORD].ToString();

                    if (!UserUtils.IsUserLocationExist(context, email))
                    {
                        InfiltratorUtils.AddInfiltrator(context, InfiltratorConstant.WARNING_USER_NOT_HAVE_LOCATION, "User have not any location info");
                        errorCode = ResponseConstant.ERROR_LOCATION_NOT_FOUND;
                    }
                }

                return this;
            }

            public GetErrorCase addOnErrorListener(EventHandler eventHandler)
            {
                Error += eventHandler;
                return this;
            }

            protected virtual void OnError(ErrorEventArgs e)
            {
                if (Error != null)
                    Error(this, e);
            }

            public GetErrorCase addOnSuccessListener(EventHandler eventHandler)
            {
                Success += eventHandler;
                return this;
            }

            protected virtual void OnSuccess(string[] parameters)
            {
                if (Success != null)
                    Success(this, new SuccessGetEventArgs(parameters));
            }

            public void Check()
            {
                if (errorCode != ResponseConstant.ERROR_NONE)
                {
                    OnError(new ErrorEventArgs(errorCode));
                }
                else
                {
                    OnSuccess(keys.ToArray());
                }
            }
        }
    }
}