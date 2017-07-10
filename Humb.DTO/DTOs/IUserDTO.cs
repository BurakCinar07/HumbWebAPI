using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.DTO.DTOs
{
    interface IUserDTO
    {
        int Id { get; set; }
        string NameSurname { get; set; }
        string ProfilePictureURL { get; set; }
        string ProfilePictureThumbnailURL { get; set; }
        double? Latitude { get; set; }
        double? Longitude { get; set; }
    }
}
