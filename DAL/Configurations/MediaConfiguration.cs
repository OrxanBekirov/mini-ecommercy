using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Configurations
{
    public class MediaConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Url).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.PublicId).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ResourceType).HasMaxLength(50);

            builder.Property(x => x.OwnerKey).HasMaxLength(100);

            builder.HasIndex(x => new { x.OwnerType, x.OwnerId, x.OwnerKey });
        }
    }
}
