using Humb.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Humb.API.Models
{
    public abstract class BaseResponse
    {
        private bool _error = false;
        public bool error {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
            }
        }
        private int _errorCode = ResponseConstant.ERROR_NONE;
        public int ErrorCode { get; set; }
    }
}