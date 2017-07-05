using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Filters.ErrorHandlers
{
    public class SuccessGetEventArgs : EventArgs
    {
        public string[] parameters { get; set; }

        public SuccessGetEventArgs(string[] parameters)
        {
            this.parameters = parameters;
        }
    }
}