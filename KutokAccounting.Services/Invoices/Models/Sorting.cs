namespace KutokAccounting.Services.Invoices.Models;

public sealed record Sorting
{
	public string? SortBy { get; set; }
	public bool Descending { get; set; }
}