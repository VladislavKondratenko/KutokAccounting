using KutokAccounting.DataProvider.Models;

namespace KutokAccounting.Services.Invoices.Models;

public sealed record InvoiceDto
{
	public int Id { get; set; }
	public required string Number { get; init; }
	public required int StoreId { get; init; }
	public required int VendorId { get; init; }
	public Money Money { get; init; }
}