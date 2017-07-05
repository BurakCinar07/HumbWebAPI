using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels
{
    public class SuccessResponse
    {
        private bool _error = false;
        public bool error
        {
            get
            {
                return _error;
            }
            set
            {

                _error = value;
            }
        }
    }
}