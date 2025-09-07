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
            .HasMaxLength(100)
            .IsRequired();
        
        builder
            .Property(tp => tp.IsPositiveValue)
            .HasColumnName("is_positive_value")
            .HasColumnType("INTEGER")
            .HasConversion<BooleanConverter>()
            .IsRequired();

        builder
            .HasMany(tp => tp.Transactions)
            .WithOne(t => t.TransactionType)
            .HasForeignKey(t => t.TransactionTypeId)
            .IsRequired(false);
    }
}