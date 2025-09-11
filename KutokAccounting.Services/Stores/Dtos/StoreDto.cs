namespace KutokAccounting.Services.Stores.Dtos;

public class StoreDto
{
	public string? Name { get; set; }
	public bool IsOpened { get; set; }
	public DateTime SetupDate { get; set; }
	public string? Address { get; set; }
}