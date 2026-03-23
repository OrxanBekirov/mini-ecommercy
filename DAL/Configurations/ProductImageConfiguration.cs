using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.HasKey(pi => pi.Id);
            builder.Property(pi => pi.ImageUrl).IsRequired();
            builder.Property(pi => pi.PublicId).HasMaxLength(200);
            builder.Property(pi => pi.IsMain).HasDefaultValue(false);

        }
    }
}
