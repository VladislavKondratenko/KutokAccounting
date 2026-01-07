using KutokAccounting.Services.Stores.Models;

namespace KutokAccounting.Services.Invoices.Models;

public sealed record Filters
{
	public bool? IsClosed { get; set; }
	public int? VendorId { get; set; }
	public required int StoreId { get; set; }
	public string? Number { get; set; }
	public DateTimeRange? Range { get; set; }
}