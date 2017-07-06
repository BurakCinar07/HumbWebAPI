using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Data.Mapping
{
    public class BookMap : EntityTypeConfiguration<Book>
    {
        public BookMap()
        {
            HasKey(t => t.ID);
            Property(t => t.ID).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            Property(t => t.BookName).IsRequired().HasColumnType("nvarchar").HasMaxLength(64);
            Property(t => t.BookState).IsRequired().HasColumnType("int");
            Property(t => t.Author).IsOptional().HasColumnType("nvarchar").HasMaxLength(64);
            Property(t => t.GenreCode).IsRequired().HasColumnType("int");
            Property(t => t.BookPictureURL).IsRequired().HasColumnType("nvarchar").HasMaxLength(1024);
            Property(t => t.BookPictureThumbnailURL).IsRequired().HasColumnType("nvarchar").HasMaxLength(1024);
            Property(t => t.OwnerID).IsRequired().HasColumnType("int");
            Property(t => t.AddedByID).IsRequired().HasColumnType("int");
            Property(t => t.CreatedAt).IsRequired().HasColumnType("datetime2").HasPrecision(7);

        }
    }
}
