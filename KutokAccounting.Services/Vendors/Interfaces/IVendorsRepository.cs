using KutokAccounting.DataProvider.Models;

namespace KutokAccounting.Services.Vendors;

public interface IVendorRepository
{
    ValueTask CreateAsync(Vendor vendor, CancellationToken cancellationToken);
    ValueTask<Vendor> GetByIdAsync(int id, CancellationToken cancellationToken);
    ValueTask<List<Vendor>> GetVendorsAsync(CancellationToken cancellationToken);
    ValueTask DeleteAsync(int id, CancellationToken cancellationToken);
    ValueTask UpdateAsync(Vendor vendor, CancellationToken cancellationToken);
}