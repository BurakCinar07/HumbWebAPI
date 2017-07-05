using BookieAPI.Models.ResponseModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels
{
    public class RegisterResponse : BaseResponse
    {
        public RegisterResponse()
        {
            this.userLoginModel = new UserLoginModel();
        }
        public UserLoginModel userLoginModel { get; set; }

    }
}