using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace BookieAPI.Filters.ErrorHandlers
{
    public class SuccessPostEventArgs : EventArgs
    {
        public JObject postObject { get; set; }

        public SuccessPostEventArgs(JObject postObject)
        {
            this.postObject = postObject;
        }
    }
}