using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(p => p.Method)
                   .IsRequired();

            builder.Property(p => p.Status)
                   .IsRequired();

            builder.Property(p => p.ProviderReference)
                   .HasMaxLength(200);

            // 1-1 relation
            builder.HasOne(p => p.Order)
                   .WithOne(o => o.Payment)
                   .HasForeignKey<Payment>(p => p.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
