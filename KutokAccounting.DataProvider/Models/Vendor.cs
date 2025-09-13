namespace KutokAccounting.DataProvider.Models;

public class Vendor
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public IEnumerable<Invoice>? Invoices { get; set; }
}