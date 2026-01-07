namespace KutokAccounting.DataProvider.Models;

public class Transaction
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }
	public required Money Money { get; set; }
	public DateTime CreatedAt { get; set; }
	public int StoreId { get; set; }
	public Store? Store { get; set; }
	public int? TransactionTypeId { get; set; }
	public TransactionType? TransactionType { get; set; }
	public int? InvoiceId { get; set; }
	public Invoice? Invoice { get; set; }
}