using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels
{
    public class MessageSenderResponse : BaseResponse
    {
        public int oldMessageID { get; set; }
        public int newMessageID { get; set; }
    }
}