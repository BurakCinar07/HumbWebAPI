﻿using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Data.Mapping
{
    class ReportBookMap : EntityTypeConfiguration<ReportBook>
    {
        public ReportBookMap()
        {
            HasKey(t => t.Id);
            Property(t => t.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(t => t.CreatedAt).HasColumnType("datetime2").HasPrecision(7);

            Property(t => t.ReportInfo).HasColumnType("nvarchar(max)");
        }
    }
}
