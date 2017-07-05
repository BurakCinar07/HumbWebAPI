using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels.Models
{
    public class FetchMessageModel
    {
        public int messageID { get; set; }
        public UserModel fromUser { get; set; }
        public UserModel toUser { get; set; }
        public string messageText { get; set; }
        public string createdAt { get; set; }
        public int messageState { get; set; }
    }
}