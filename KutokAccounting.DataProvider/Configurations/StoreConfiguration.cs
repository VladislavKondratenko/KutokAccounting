using KutokAccounting.DataProvider.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KutokAccounting.DataProvider.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("store");
        
        builder.HasKey(s => s.Id);

        builder
            .HasIndex(s => s.Name)
            .HasDatabaseName("index_name");

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .HasColumnType("TEXT")
            .HasMaxLength(512)
            .IsRequired();
        
        builder
            .Property(s => s.IsOpened)
            .HasColumnName("is_opened")
            .HasColumnType("INTEGER")
            .HasConversion<BooleanConverter>()
            .IsRequired();
        
        builder
            .Property(s => s.SetupDate)
            .HasColumnName("setup_date")
            .HasColumnType("INTEGER")
            .HasConversion<DateTimeConverter>()
            .IsRequired();
        
        builder
            .Property(s => s.Address)
            .HasColumnName("address")
            .HasColumnType("TEXT")
            .HasMaxLength(100)
            .IsRequired();
    }
}