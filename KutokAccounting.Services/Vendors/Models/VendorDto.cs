namespace KutokAccounting.Services.Vendors.DataTransferObjects;

public sealed record VendorDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}