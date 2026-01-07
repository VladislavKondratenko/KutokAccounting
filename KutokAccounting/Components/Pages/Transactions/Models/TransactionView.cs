using KutokAccounting.Components.Pages.Invoices.Models;
using KutokAccounting.Components.Pages.TransactionTypes.Models;
using KutokAccounting.DataProvider.Models;

namespace KutokAccounting.Components.Pages.Transactions.Models;

public sealed record TransactionView
{
	public required int Id { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }
	public Money Money { get; set; }
	public DateTime CreatedAt { get; set; }
	public int StoreId { get; set; }
	public required TransactionTypeView TransactionType { get; set; }
	public InvoiceView? Invoice { get; set; }
}