using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels
{
    public class PictureUploadResponse : BaseResponse
    {
        public string pictureURL { get; set; }
        public string thumbnailURL { get; set; }
    }
}