using KutokAccounting.DataProvider.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KutokAccounting.DataProvider.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoice");

        builder.HasKey(i => i.Id);
        
        builder
            .HasIndex(i => i.CreatedAt)
            .HasDatabaseName("index_created_at");
        
        builder
            .Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("INTEGER")
            .HasConversion<DateTimeConverter>()
            .IsRequired();
        
        builder
            .Property(i => i.Number)
            .HasColumnName("number")
            .HasColumnType("TEXT")
            .HasMaxLength(100)
            .IsRequired();

        builder
            .HasOne(i => i.Store)
            .WithMany(s => s.Invoices)
            .HasForeignKey(i => i.StoreId)
            .IsRequired();
        
        builder
            .HasOne(i => i.Vendor)
            .WithMany(v => v.Invoices)
            .HasForeignKey(i => i.VendorId)
            .IsRequired();
    }
}