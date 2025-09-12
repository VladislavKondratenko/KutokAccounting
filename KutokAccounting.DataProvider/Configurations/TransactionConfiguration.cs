using KutokAccounting.DataProvider.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KutokAccounting.DataProvider.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transaction");
        
        builder.HasKey(t => t.Id);

        builder
            .HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Transaction_Created_At");
         
        builder
            .Property(t => t.Description)
            .HasColumnName("description")
            .HasColumnType("TEXT")
            .HasMaxLength(1024);
        
        builder
            .Property(t => t.Value)
            .HasColumnName("value")
            .HasColumnType("INTEGER")
            .IsRequired();
        
        builder
            .Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("INTEGER")
            .HasConversion<DateTimeConverter>()
            .IsRequired();

        builder
            .HasOne(t => t.Store)
            .WithMany(s => s.Transactions)
            .HasForeignKey(t => t.StoreId)
            .IsRequired();
        
        builder
            .HasOne(t => t.TransactionType)
            .WithMany(tp => tp.Transactions)
            .HasForeignKey(t => t.TransactionTypeId)
            .IsRequired();
        
        builder
            .HasOne(t => t.Invoice)
            .WithMany(i => i.Transactions)
            .HasForeignKey(t => t.InvoiceId)
            .IsRequired(false);
    }
}