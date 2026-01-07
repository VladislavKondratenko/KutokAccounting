using KutokAccounting.DataProvider.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KutokAccounting.DataProvider.Configurations;

public class InvoiceStatusConfiguration : IEntityTypeConfiguration<InvoiceStatus>
{
	public void Configure(EntityTypeBuilder<InvoiceStatus> builder)
	{
		builder.ToTable("invoice_status");

		builder.HasKey(s => s.Id);

		builder
			.Property(s => s.State)
			.HasColumnName("state")
			.HasColumnType("INTEGER")
			.IsRequired();

		builder
			.Property(s => s.CreatedAt)
			.HasColumnName("created_at")
			.HasColumnType("INTEGER")
			.HasConversion<DateTimeConverter>()
			.IsRequired();
	}
}