namespace KutokAccounting.DataProvider.Models;

public class InvoiceStatus
{
	public int Id { get; set; }
	public int InvoiceId { get; set; }
	public Invoice? Invoice { get; set; }
	public required DateTime CreatedAt { get; set; }
	public required State State { get; set; }
}