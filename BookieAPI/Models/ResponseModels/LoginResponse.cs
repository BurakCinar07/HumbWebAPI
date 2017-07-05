using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels
{
    public class LoginResponse : BaseResponse
    {
        public LoginResponse()
        {
            this.userLoginModel = new UserLoginModel();
        }
        public LoginResponse(BaseResponse response)
        {
            this.error = response.error;
            this.errorCode = response.errorCode;
            this.userLoginModel = new UserLoginModel();
        }
        public UserLoginModel userLoginModel { get; set; }   
        public int[] lovedGenres { get; set; }    
    }
}