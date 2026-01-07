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
			.Property(tp => tp.IsIncome)
			.HasColumnName("is_positive_value")
			.HasColumnType("INTEGER")
			.HasConversion<BooleanConverter>()
			.IsRequired();

		builder
			.HasMany(tp => tp.Transactions)
			.WithOne(t => t.TransactionType)
			.HasForeignKey(t => t.TransactionTypeId)
			.IsRequired(false);

		//New feature
		builder
			.Property(i => i.Code)
			.HasColumnName("code")
			.HasColumnType("TEXT")
			.HasMaxLength(100)
			.HasDefaultValue(KutokConfigurations.CustomTransactionTypeCode) //Зробити константою
			.IsRequired();

		//New feature
		builder.HasData(new TransactionType
		{
			Id = 1,
			Name = "Відкриття накладної",
			IsIncome = false,
			Code = KutokConfigurations.OpenInvoiceTransactionTypeCode // Зробити константою
		}, new TransactionType
		{
			Id = 2,
			Name = "Закриття накладної",
			IsIncome = true,
			Code = KutokConfigurations.CloseInvoiceTransactionTypeCode // Зробити константою
		});
	}
}