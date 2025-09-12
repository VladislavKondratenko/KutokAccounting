using KutokAccounting.DataProvider.Models;

namespace KutokAccounting.Services.Vendors.DataTransferObjects;

public sealed record VendorPagedResult
{
    public IEnumerable<Vendor>? Vendors { get; set; }
    public int Count { get; set; }
}