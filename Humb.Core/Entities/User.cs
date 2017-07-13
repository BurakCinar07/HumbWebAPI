﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Humb.Core.Entities
{
    public partial class User : BaseEntity
    {
        public User()
        {
            BookInteractions = new HashSet<BookInteraction>();
            LovedGenres = new HashSet<LovedGenre>();
        }
        [Required()]
        public string NameSurname { get; set; }
        [Required()]
        public string Email { get; set; }
        [Required()]
        public string Password { get; set; }
        
        public string ProfilePictureUrl { get; set; }

        public string ProfilePictureThumbnailUrl { get; set; }
        public string Bio { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string FcmToken { get; set; }

        public bool EmailVerified { get; set; }

        public string VerificationHash { get; set; }

        public virtual ICollection<BookInteraction> BookInteractions { get; set; }

        public virtual ICollection<LovedGenre> LovedGenres { get; set; }
    }
}
