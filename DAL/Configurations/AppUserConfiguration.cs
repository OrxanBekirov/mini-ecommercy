using Entities.Concrete;
using Entities.Concrete.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FullName).HasMaxLength(150);

            // 1-1 AppUser ↔ Cart
            builder.HasOne(u => u.Cart)
                   .WithOne(c => c.AppUser)
                   .HasForeignKey<Cart>(c => c.AppUserId);
        }
    }
}
