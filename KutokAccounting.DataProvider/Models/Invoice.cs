namespace KutokAccounting.DataProvider.Models;

public class Invoice
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Number { get; set; } = null!;
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;
    public IEnumerable<Transaction>? Transactions { get; set; }
}