namespace BookieAPI.Models.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            BookInteractions = new HashSet<BookInteraction>();
            LovedGenres = new HashSet<LovedGenre>();
        }
        [Key]
        public int userID { get; set; }

        [Required()]
        [StringLength(70)]
        public string nameSurname { get; set; }

        [Required()]
        [StringLength(320)]
        public string email { get; set; }

        [Required()]
        [StringLength(50)]
        public string password { get; set; }

        [StringLength(1024)]
        public string profilePictureURL { get; set; }

        [StringLength(1024)]
        public string profilePictureThumbnailURL { get; set; }
        [StringLength(1024)]
        public string bio { get; set; }

        public double? latitude { get; set; }

        public double? longitude { get; set; }

        public string fcmToken { get; set; }

        public bool emailVerified { get; set; }

        [StringLength(50)]
        public string verificationHash { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime createdAt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookInteraction> BookInteractions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LovedGenre> LovedGenres { get; set; }
    }
}
