﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.DTO.DTOs
{
    public class UserDTO : IUserDTO
    {
        public int Id { get; set; }
        public string NameSurname { get; set; }
        public string ProfilePictureURL { get; set; }
        public string ProfilePictureThumbnailURL { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
