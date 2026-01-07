namespace KutokAccounting.Services.Transactions.Models;

public sealed record TransactionDto
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }
	public long Value { get; set; }
	public int StoreId { get; set; }
	public int? TransactionTypeId { get; set; }
	public int? InvoiceId { get; set; }
}