using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels
{
    public class UserValidationResponse : BaseResponse
    {
        private bool _isValid = false;
        public bool isValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                _isValid = value;
            }
        }
    }
}