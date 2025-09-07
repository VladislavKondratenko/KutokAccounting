namespace KutokAccounting.DataProvider.Models;

public class Transaction
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public int Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public int TransactionTypeId { get; set; }
    public TransactionType TransactionType { get; set; } = null!;
    public int? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
}