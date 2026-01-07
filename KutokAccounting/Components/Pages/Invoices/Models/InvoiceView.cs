using KutokAccounting.DataProvider.Models;

namespace KutokAccounting.Components.Pages.Invoices.Models;

public sealed record InvoiceView
{
	public required int Id { get; set; }
	public required string Number { get; set; }
	public required Vendor Vendor { get; set; }
	public required IEnumerable<InvoiceStatus> StatusHistory { get; set; }
	public string VendorName => Vendor.Name;
	public Money Money { get; set; }
	public InvoiceStatus Status => StatusHistory.Last();
}