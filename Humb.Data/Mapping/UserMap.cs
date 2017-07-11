using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Data.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            HasKey(t => t.Id);
            Property(t => t.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(t => t.CreatedAt).IsRequired().HasColumnType("datetime2").HasPrecision(7);

            Property(t => t.NameSurname).IsRequired().HasMaxLength(128).HasColumnType("nvarchar");
            Property(t => t.Email).IsRequired().HasMaxLength(256).HasColumnType("nvarchar");
            Property(t => t.Password).IsRequired().HasMaxLength(64).HasColumnType("nvarchar");
            Property(t => t.ProfilePictureUrl).HasMaxLength(1024).HasColumnType("nvarchar");
            Property(t => t.ProfilePictureThumbnailUrl).HasMaxLength(1024).HasColumnType("nvarchar");
            Property(t => t.Bio).HasMaxLength(1024).HasColumnType("nvarchar");
            Property(t => t.Latitude).IsOptional().HasColumnType("float");
            Property(t => t.Longitude).IsOptional().HasColumnType("float");
            Property(t => t.FcmToken).IsOptional().HasColumnType("nvarchar(max)");
            Property(t => t.EmailVerified).IsRequired().HasColumnType("bit");
            Property(t => t.VerificationHash).IsOptional().HasMaxLength(64).HasColumnType("nvarchar");

            ToTable("User");      
        }
    }
}
