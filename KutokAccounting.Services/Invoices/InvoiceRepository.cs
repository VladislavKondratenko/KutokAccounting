using KutokAccounting.DataProvider;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Invoices.Interfaces;
using KutokAccounting.Services.Invoices.Models;
using KutokAccounting.Services.TransactionTypes.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KutokAccounting.Services.Invoices;

public class InvoiceRepository : IInvoiceRepository
{
	private readonly SemaphoreSlim _semaphoreSlim;
	private readonly KutokDbContext _dbContext;
	private readonly ILogger<InvoiceRepository> _logger;

	public InvoiceRepository(
		[FromKeyedServices(KutokConfigurations.WriteOperationsSemaphore)]
		SemaphoreSlim semaphoreSlim,
		KutokDbContext dbContext,
		ILogger<InvoiceRepository> logger)
	{
		_semaphoreSlim = semaphoreSlim;
		_dbContext = dbContext;
		_logger = logger;
	}

	public async ValueTask<PagedResult<Invoice>> GetAsync(InvoiceQueryParameters parameters,
		CancellationToken cancellationToken)
	{
		IQueryable<Invoice> query = _dbContext.Invoices.AsNoTracking();

		InvoiceQueryBuilder builder = new(query);

		query = builder
			.AddFilters(parameters.Filters)
			.AddSearchString(parameters.SearchString)
			.AddSorting(parameters.Sorting)
			.Build();

		try
		{
			Task<int> countTask = query.CountAsync(cancellationToken);

			List<Invoice> invoices = await query
				.Skip(parameters.Pagination.Skip)
				.Take(parameters.Pagination.PageSize)
				.Include(i => i.Transactions)
				.Include(i => i.StatusHistory)
				.Include(i => i.Vendor)
				.Include(i => i.Store)
				.Select(i => new Invoice
				{
					Id = i.Id,
					CreatedAt = i.CreatedAt,
					Number = i.Number,
					StatusHistory = i.StatusHistory,
					Vendor = i.Vendor,
					Store = i.Store,
					StoreId = i.StoreId,
					VendorId = i.VendorId,
					Transactions = i.Transactions
				})
				.ToListAsync(cancellationToken);
	
			return new PagedResult<Invoice>
			{
				Items = invoices,
				Count = await countTask
			};
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Failed to retrieve invoices with QueryParameters: {QueryParameters}", parameters);

			throw;
		}
	}

	public async ValueTask<Invoice?> GetByIdAsync(int id, CancellationToken cancellationToken)
	{
		try
		{
			Invoice? invoice = await _dbContext.Invoices
				.AsNoTracking()
				.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

			return invoice ?? throw new NotFoundException("Invoice not found");
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Failed to retrieve invoice with Id: {InvoiceId}", id);

			throw;
		}
	}

	public async ValueTask CreateAsync(Invoice invoice, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		await _semaphoreSlim.WaitAsync(cancellationToken);

		try
		{
			await _dbContext.Invoices.AddAsync(invoice, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to create invoice with Number: {InvoiceNumber}",
				invoice.Number);

			throw;
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	public async ValueTask UpdateAsync(Invoice invoice, CancellationToken cancellationToken)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);

		try
		{
			await _dbContext.Invoices
				.Where(i => i.Id == invoice.Id)
				.ExecuteUpdateAsync(
					i => i
						.SetProperty(p => p.Number, invoice.Number)
						.SetProperty(p => p.VendorId, invoice.VendorId), cancellationToken);
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Failed to update invoice with Id: {InvoiceId}, Name: {InvoiceNumber}", invoice.Id,
				invoice.Number);

			throw;
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	public async ValueTask DeleteAsync(int id, CancellationToken cancellationToken)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);

		try
		{
			await _dbContext.Invoices
				.Where(i => i.Id == id)
				.ExecuteDeleteAsync(cancellationToken);
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Failed to delete invoice with Id: {InvoiceId}", id);
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	public async ValueTask CloseAsync(int id, CancellationToken cancellationToken)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);

		try
		{
			Invoice? invoice = await _dbContext.Invoices.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

			if (invoice is not null)
			{
				invoice.StatusHistory.Add(new InvoiceStatus
				{
					CreatedAt = DateTime.Now,
					State = State.Closed
				});
			}

			await _dbContext.SaveChangesAsync(cancellationToken);
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Failed to close invoice with Id: {InvoiceId}", id);

			throw;
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}
}