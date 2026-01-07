using KutokAccounting.DataProvider.Models;

namespace KutokAccounting.Services.Invoices.Models;

public sealed record InvoiceQueryParameters
{
	public Filters? Filters { get; init; }
	public Sorting? Sorting { get; init; }
	public string? SearchString { get; init; }
	public Pagination Pagination { get; init; }
}