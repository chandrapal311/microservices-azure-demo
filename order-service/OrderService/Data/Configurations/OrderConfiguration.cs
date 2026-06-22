using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Models;

namespace OrderService.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Amount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(o => o.Status)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(o => o.RowVersion)
       .IsRowVersion();


            builder.HasMany(o => o.Items)
       .WithOne(i => i.Order)
       .HasForeignKey(i => i.OrderId);
        }
    }
}
