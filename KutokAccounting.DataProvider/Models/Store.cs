namespace KutokAccounting.DataProvider.Models;

public class Store
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsOpened { get; set; }
    public DateTime SetupDate { get; set; }
    public string? Address { get; set; }
    public IEnumerable<Transaction>? Transactions { get; set; }
    public IEnumerable<Invoice>? Invoices { get; set; }
}