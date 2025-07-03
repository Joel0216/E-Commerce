using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Mappings
{
    public class OrderMapping : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Status).IsRequired();

            builder.HasMany(o => o.OrderItems)
                   .WithOne(i => i.Order)
                   .HasForeignKey(i => i.OrderId);
        }
    }
}
