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
			.HasIndex(t => t.Name)
			.HasDatabaseName("IX_Transaction_Name");

		builder
			.Property(t => t.Name)
			.HasColumnName("name")
			.HasColumnType("TEXT")
			.HasMaxLength(100)
			.IsRequired();

		builder
			.Property(t => t.Description)
			.HasColumnName("description")
			.HasColumnType("TEXT")
			.HasMaxLength(1024)
			.IsRequired(false);

		builder
			.Property(t => t.Money)
			.HasColumnName("value")
			.HasColumnType("NUMERIC")
			.HasConversion<MoneyConverter>()
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
			.OnDelete(DeleteBehavior.Restrict)
			.IsRequired(false);
	}
}