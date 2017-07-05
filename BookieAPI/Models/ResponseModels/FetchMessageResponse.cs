using BookieAPI.Models.DAL;
using BookieAPI.Models.ResponseModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels
{
    public class FetchMessageResponse : BaseResponse
    {
        public List<FetchMessageModel> Messages { get; set; }
        
    }
}