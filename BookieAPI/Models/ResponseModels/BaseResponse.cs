using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookieAPI.Models.DAL;
using BookieAPI.Constants;
namespace BookieAPI.Models.ResponseModels
{
    public class BaseResponse
    {
        private bool _error = true;
        public bool error {
            get
            {
                return _error;
            }
            set {
                _error = value;
            }
        }
        private int _errorCode = ResponseConstant.ERROR_NONE;
        public int errorCode {
            get
            {
                return _errorCode;
            }
            set
            {
                _errorCode = value;
            }
        }        
    }
}