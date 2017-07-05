using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookieAPI.Models.ResponseModels.Models
{
    public class UserModel
    {
        public int ID { get; set; }
        public string nameSurname { get; set; }
        public string profilePictureURL { get; set; }
        public string profilePictureThumbnailURL { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
    }
}