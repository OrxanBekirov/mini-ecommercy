using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Configurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.Property(x => x.AppUserId).IsRequired();
            builder.Property(x => x.ProductId).IsRequired();
            builder.HasOne(x=>x.AppUser)
                .WithMany(X=>X.Wishlists)
                .HasForeignKey(builder=>builder.AppUserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x=>x.Product).WithMany(X=>X.Wishlists)
                .HasForeignKey(builder=>builder.ProductId).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(x => new { x.AppUserId, x.ProductId }).IsUnique();
        }
    }
}
