namespace KutokAccounting.DataProvider.Models;

public class Invoice
{
	public int Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public required string Number { get; set; }
	public int StoreId { get; set; }
	public Store? Store { get; set; }
	public int VendorId { get; set; }
	public Vendor? Vendor { get; set; }
	public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
	public ICollection<InvoiceStatus> StatusHistory { get; set; } = new List<InvoiceStatus>();
}