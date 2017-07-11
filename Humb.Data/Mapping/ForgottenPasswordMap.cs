using Humb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Data.Mapping
{
    class ForgottenPasswordMap : EntityTypeConfiguration<ForgottenPassword>
    {
        public ForgottenPasswordMap()
        {
            HasKey(t => t.Id);
            Property(t => t.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(t => t.CreatedAt).HasColumnType("datetime2").HasPrecision(7);

            Property(t => t.Email).IsRequired().HasColumnType("nvarchar").HasMaxLength(256);
            Property(t => t.Token).IsRequired().HasColumnType("nvarchar").HasMaxLength(64);
            Property(t => t.NewPassword).IsRequired().HasColumnType("nvarchar").HasMaxLength(64);

        }
    }
}
