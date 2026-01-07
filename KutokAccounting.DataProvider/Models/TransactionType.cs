namespace KutokAccounting.DataProvider.Models;

public class TransactionType
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public bool IsIncome { get; set; }
	public string? Code { get; set; }
	public IEnumerable<Transaction>? Transactions { get; set; }
}