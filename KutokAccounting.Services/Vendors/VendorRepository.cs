using KutokAccounting.DataProvider;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.TransactionTypes.Exceptions;
using KutokAccounting.Services.Vendors.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KutokAccounting.Services.Vendors;

public sealed class VendorRepository : IVendorRepository
{
	private readonly KutokDbContext _dbContext;
	private readonly SemaphoreSlim _semaphoreSlim;
	private readonly ILogger<VendorRepository> _logger;

	public VendorRepository(
		KutokDbContext dbContext,
		[FromKeyedServices(KutokConfigurations.WriteOperationsSemaphore)]
		SemaphoreSlim semaphoreSlim,
		ILogger<VendorRepository> logger)
	{
		_dbContext = dbContext;
		_semaphoreSlim = semaphoreSlim;
		_logger = logger;
	}

	public async ValueTask CreateAsync(Vendor vendor, CancellationToken cancellationToken)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);

		try
		{
			await _dbContext.Vendors.AddAsync(vendor, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Failed to create vendor with Name: {VendorName}", vendor.Name);

			throw;
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	public async ValueTask<PagedResult<Vendor>> GetAsync(VendorQueryParameters vendorQueryParameters,
		CancellationToken cancellationToken)
	{
		IQueryable<Vendor> query = _dbContext.Vendors.AsNoTracking();

		if (string.IsNullOrWhiteSpace(vendorQueryParameters.Name) is false)
		{
			query = query.Where(v => v.Name == vendorQueryParameters.Name);
		}

		if (string.IsNullOrEmpty(vendorQueryParameters.SearchString) is false)
		{
			query = query.Where(v =>
				EF.Functions.Like(v.Name, $"%{vendorQueryParameters.SearchString}%") ||
				EF.Functions.Like(v.Description, $"%{vendorQueryParameters.SearchString}%"));
		}

		try
		{
			Task<int> countTask = query.CountAsync(cancellationToken);

			List<Vendor> vendors = await query
				.Skip(vendorQueryParameters.Pagination.Skip)
				.Take(vendorQueryParameters.Pagination.PageSize)
				.Select(v => new Vendor
				{
					Id = v.Id,
					Name = v.Name,
					Description = v.Description
				})
				.OrderBy(v => v.Name)
				.ToListAsync(cancellationToken);

			return new PagedResult<Vendor>
			{
				Items = vendors,
				Count = await countTask
			};
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Failed to retrieve vendors with QueryParameters: {QueryParameters}",
				vendorQueryParameters);

			throw;
		}
	}

	public async ValueTask<Vendor> GetByIdAsync(int id, CancellationToken cancellationToken)
	{
		try
		{
			Vendor? vendor = await _dbContext.Vendors
				.AsNoTracking()
				.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

			return vendor ?? throw new NotFoundException("Vendor not found");
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Failed to retrieve vendor with Id: {VendorId}", id);

			throw;
		}
	}

	public async ValueTask DeleteAsync(int id, CancellationToken cancellationToken)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);

		try
		{
			await _dbContext.Vendors
				.Where(v => v.Id == id)
				.ExecuteDeleteAsync(cancellationToken);
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Failed to delete vendor with Id: {VendorId}", id);

			throw;
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	public async ValueTask UpdateAsync(Vendor vendor, CancellationToken cancellationToken)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);

		try
		{
			await _dbContext.Vendors
				.Where(v => v.Id == vendor.Id)
				.ExecuteUpdateAsync(v => v
					.SetProperty(p => p.Name, vendor.Name)
					.SetProperty(p => p.Description, vendor.Description), cancellationToken);
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Failed to update vendor with Id: {VendorId}, Name: {VendorName}", vendor.Id,
				vendor.Name);

			throw;
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}
}