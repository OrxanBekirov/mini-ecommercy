using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Property(p => p.StockQuantity).IsRequired();

            builder.HasMany(p => p.ProductImages)
                   .WithOne(pi => pi.Product)
                   .HasForeignKey(pi => pi.ProductId);



            builder.Property(p => p.RowVersion)
       .IsRowVersion();
        }
    }
}
