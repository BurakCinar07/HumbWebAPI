using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels
{
    public class UserChangePasswordResponse : BaseResponse
    {
        public string newPassword { get; set; }
    }
}