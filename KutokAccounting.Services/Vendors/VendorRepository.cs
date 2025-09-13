using KutokAccounting.DataProvider;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Vendors.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace KutokAccounting.Services.Vendors;

public sealed class VendorRepository : IVendorRepository
{
    private readonly KutokDbContext _dbContext;

    public VendorRepository(KutokDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask CreateAsync(Vendor vendor, CancellationToken cancellationToken)
    {
        await _dbContext.Vendors.AddAsync(vendor, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask<VendorPagedResult> GetAsync(QueryParameters queryParameters, CancellationToken cancellationToken)
    {
        var pagination = new Pagination()
        {
            Page = queryParameters.Page,
            PageSize = queryParameters.PageSize
        };
        
        var pagedResult = new VendorPagedResult();

        var query = _dbContext.Vendors.AsQueryable();

        if (string.IsNullOrWhiteSpace(queryParameters.Name) is false)
            query = query.Where(v => v.Name == queryParameters.Name);

        if (string.IsNullOrEmpty(queryParameters.SearchString) is false)
            query = query.Where(v =>
                EF.Functions.Like(v.Name, $"%{queryParameters.SearchString}%") || EF.Functions.Like(v.Description, $"%{queryParameters.SearchString}%"));

        pagedResult.Count = await query.CountAsync(cancellationToken);

        var vendors = await query
            .AsNoTracking()
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .Select(v => new Vendor
            {
                Id = v.Id,
                Name = v.Name,
                Description = v.Description
            })
            .OrderBy(v => v.Name)
            .ToListAsync(cancellationToken);

        pagedResult.Vendors = vendors;

        return pagedResult;
    }

    public async ValueTask<Vendor?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Vendors
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async ValueTask DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await _dbContext.Vendors
            .Where(v => v.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async ValueTask UpdateAsync(Vendor vendor, CancellationToken cancellationToken)
    {
        await _dbContext.Vendors
            .Where(v => v.Id == vendor.Id)
            .ExecuteUpdateAsync(v => v
                .SetProperty(p => p.Name, vendor.Name)
                .SetProperty(p => p.Description, vendor.Description), cancellationToken);
    }
}