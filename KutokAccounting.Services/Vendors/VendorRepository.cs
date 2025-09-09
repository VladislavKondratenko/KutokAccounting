using KutokAccounting.DataProvider;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Vendors.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace KutokAccounting.Services.Vendors;

public class VendorRepository : IVendorRepository
{
    private readonly KutokDbContext _dbContext;

    public VendorRepository(KutokDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async ValueTask CreateAsync(Vendor vendor, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await _dbContext.Vendors.AddAsync(vendor, cancellationToken);
    }

    public async ValueTask<List<Vendor>> GetAsync(QueryParameters queryParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();   
        
        var page = queryParameters.Page;
        var pageSize = queryParameters.PageSize;
        
        var query = _dbContext.Vendors.AsQueryable();
        if (!string.IsNullOrWhiteSpace(queryParameters.Name))
            query = query.Where(v => v.Name == queryParameters.Name);

        return await query
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new Vendor()
            {
                Id = v.Id,
                Name = v.Name,
                Description = v.Description
            })
            .OrderBy(v => v.Name)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<Vendor?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        return await _dbContext.Vendors
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async ValueTask DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await _dbContext.Vendors
            .Where(v => v.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async ValueTask UpdateAsync(Vendor vendor, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await _dbContext.Vendors
            .Where(v => v.Id == vendor.Id)
            .ExecuteUpdateAsync(v => v
                    .SetProperty(p => p.Name, vendor.Name)
                    .SetProperty(p => p.Description, vendor.Description), cancellationToken); 
    }
}