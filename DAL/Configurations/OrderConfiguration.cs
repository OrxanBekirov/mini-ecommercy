using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.TotalAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(o => o.OrderNumber)
                   .HasMaxLength(30)
                   .IsRequired();

            builder.HasIndex(o => o.OrderNumber).IsUnique();

            builder.Property(o => o.ShippingAddress)
                   .HasMaxLength(500);

            builder.Property(o => o.Note)
                   .HasMaxLength(500);

            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId);

            builder.HasOne(o => o.Payment)
                   .WithOne(p => p.Order)
                   .HasForeignKey<Payment>(p => p.OrderId);
        }
    }
}
