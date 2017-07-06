﻿using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Data.Mapping
{
    class InfiltratorMap : EntityTypeConfiguration<Infiltrator>
    {
        public InfiltratorMap()
        {
            HasKey(t => t.ID);
            Property(t => t.ID).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(t => t.CreatedAt).HasColumnType("datetime2").HasPrecision(7);

            Property(t => t.IPAdress).IsRequired().HasColumnType("nvarchar").HasMaxLength(64);
            Property(t => t.ExtraInfo).HasColumnType("nvarchar(max)");
        }
    }
}