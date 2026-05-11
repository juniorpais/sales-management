using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.SaleNumber).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Date).IsRequired();
        builder.Property(s => s.CustomerId).IsRequired().HasColumnType("uuid");
        builder.Property(s => s.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(s => s.BranchId).IsRequired().HasColumnType("uuid");
        builder.Property(s => s.BranchName).IsRequired().HasMaxLength(200);
        builder.Property(s => s.IsCancelled).IsRequired().HasDefaultValue(false);
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt);

        builder.HasIndex(s => s.SaleNumber).IsUnique();

        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(s => s.TotalAmount);
        builder.Ignore(s => s.DomainEvents);
    }
}
