using KutokAccounting.DataProvider.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KutokAccounting.DataProvider.Configurations;

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.ToTable("vendor");

        builder.HasKey(v => v.Id);

        builder
            .Property(v => v.Name)
            .HasColumnName("name")
            .HasColumnType("TEXT")
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(v => v.Description)
            .HasColumnName("description")
            .HasColumnType("TEXT")
            .HasMaxLength(1024)
            .IsRequired(false);
    }
}