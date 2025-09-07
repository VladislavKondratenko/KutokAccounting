using KutokAccounting.DataProvider.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KutokAccounting.DataProvider.Configurations;

public class TransactionTypeConfiguration : IEntityTypeConfiguration<TransactionType>
{
    public void Configure(EntityTypeBuilder<TransactionType> builder)
    {
        builder.ToTable("transaction_type");
        
        builder.HasKey(tp => tp.Id);
        
        builder
            .Property(tp => tp.Name)
            .HasColumnName("name")
            .HasColumnType("TEXT")
            .HasMaxLength(512)
            .IsRequired();
        
        builder
            .Property(tp => tp.IsPositiveValue)
            .HasColumnName("is_positive_value")
            .HasColumnType("INTEGER")
            .HasConversion(ipv => ipv ? 1 : 0, ipv => ipv == 1)
            .IsRequired();

        builder
            .HasMany(tp => tp.Transactions)
            .WithOne(t => t.TransactionType)
            .HasForeignKey(t => t.TransactionTypeId)
            .IsRequired(false);
    }
}