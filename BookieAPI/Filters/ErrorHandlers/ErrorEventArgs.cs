using BookieAPI.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Filters.ErrorHandlers
{
    public class ErrorEventArgs : EventArgs
    {
        public int errorCode { get; set; }

        public ErrorEventArgs(int errorCode)
        {
            this.errorCode = errorCode;
        }
    }
}