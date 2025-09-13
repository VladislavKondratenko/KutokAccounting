using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Vendors.DataTransferObjects;

namespace KutokAccounting.Services.Vendors;

public interface IVendorService
{
    ValueTask<Vendor> CreateAsync(VendorDto request, CancellationToken cancellationToken);
    ValueTask<Vendor> GetByIdAsync(int id, CancellationToken cancellationToken);
    ValueTask<VendorPagedResult> GetAsync(QueryParameters queryParameters, CancellationToken cancellationToken);
    ValueTask UpdateAsync(VendorDto request, CancellationToken cancellationToken);
    ValueTask DeleteAsync(int id, CancellationToken cancellationToken);
}