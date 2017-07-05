using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels.Models
{
    public class UserLoginModel
    {
        public int ID { get; set; }
        public string nameSurname { get; set; }
        public string profilePictureURL { get; set; }
        public string profilePictureThumbnailURL { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public bool emailVerified { get; set; }
        public string bio { get; set; }
        public int counter { get; set; }
        public int point { get; set; }
    }
}