﻿using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Data.Mapping
{
    class BookInteractionMap : EntityTypeConfiguration<BookInteraction>
    {
        public BookInteractionMap()
        {
            HasKey(t => t.Id);
            Property(t => t.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            Property(t => t.CreatedAt).IsRequired().HasColumnType("datetime2").HasPrecision(7);
            Property(t => t.UserId).IsRequired().HasColumnType("int");
            Property(t => t.BookId).IsRequired().HasColumnType("int");
            Property(t => t.InteractionType).IsRequired().HasColumnType("int");

            ToTable("BookInteraction");

            //HasRequired(t => t.User).WithMany(t => t.BookInteractions).HasForeignKey(t => t.UserID);
            //HasRequired(t => t.Book).WithMany(t => t.BookInteractions).HasForeignKey(t => t.BookID);
        }
    }
}
