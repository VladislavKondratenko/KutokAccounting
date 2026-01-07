using KutokAccounting.DataProvider;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.TransactionTypes.Exceptions;
using KutokAccounting.Services.TransactionTypes.Interfaces;
using KutokAccounting.Services.TransactionTypes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KutokAccounting.Services.TransactionTypes;

public sealed class TransactionTypeRepository : ITransactionTypeRepository
{
	private readonly KutokDbContext _dbContext;
	private readonly SemaphoreSlim _semaphoreSlim;
	private readonly ILogger<TransactionTypeRepository> _logger;

	public TransactionTypeRepository(
		KutokDbContext dbContext,
		[FromKeyedServices(KutokConfigurations.WriteOperationsSemaphore)]
		SemaphoreSlim semaphoreSlim,
		ILogger<TransactionTypeRepository> logger)
	{
		_dbContext = dbContext;
		_logger = logger;
		_semaphoreSlim = semaphoreSlim;
	}

	public async ValueTask CreateAsync(TransactionType transactionType, CancellationToken cancellationToken)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);

		try
		{
			await _dbContext.TransactionTypes.AddAsync(transactionType, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}
		catch (Exception e)
		{
			_logger.LogInformation(e, "Failed to create transaction type with Name: {TransactionTypeName}",
				transactionType.Name);
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	public async ValueTask<PagedResult<TransactionType>> GetAsync(
		TransactionTypeQueryParameters parameters,
		CancellationToken cancellationToken)
	{
		IQueryable<TransactionType> query = _dbContext.TransactionTypes.AsNoTracking();

		if (string.IsNullOrWhiteSpace(parameters?.Filters?.Name) is false)
		{
			query = query.Where(tp => tp.Name == parameters.Filters.Name);
		}

		if (parameters?.Filters?.IsIncome is not null)
		{
			query = query.Where(tp => tp.IsIncome == parameters.Filters.IsIncome);
		}

		if (string.IsNullOrWhiteSpace(parameters?.SearchString) is false)
		{
			query = query.Where(tp => EF.Functions.Like(tp.Name, $"%{parameters.SearchString}%"));
		}

		try
		{
			Task<int> countTask = query.CountAsync(cancellationToken);

			List<TransactionType> transactionTypes = await query
				.Skip(parameters.Pagination.Skip)
				.Take(parameters.Pagination.PageSize)
				.Where(tp => tp.Code == KutokConfigurations.CustomTransactionTypeCode) //Переробити + додати константу
				.Select(tp => new TransactionType
				{
					Id = tp.Id,
					Name = tp.Name,
					IsIncome = tp.IsIncome
				})
				.OrderBy(tp => tp.Name)
				.ToListAsync(cancellationToken);

			return new PagedResult<TransactionType>
			{
				Items = transactionTypes,
				Count = await countTask
			};
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Failed to retrieve transaction types with QueryParameters: {QueryParameters}",
				parameters);

			throw;
		}
	}

	public async ValueTask<TransactionType> GetByIdAsync(int id, CancellationToken cancellationToken)
	{
		try
		{
			TransactionType? transactionType = await _dbContext.TransactionTypes
				.AsNoTracking()
				.FirstOrDefaultAsync(tp => tp.Id == id, cancellationToken);

			return transactionType ?? throw new NotFoundException("Transaction type not found.");
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Failed to retrieve transaction type with Id: {TransactionTypeId}", id);

			throw;
		}
	}

	public async ValueTask<int> DeleteAsync(int id, CancellationToken cancellationToken)
	{
		int rowsDeleted = 0;

		await _semaphoreSlim.WaitAsync(cancellationToken);

		try
		{
			rowsDeleted = await _dbContext.TransactionTypes
				.Where(transactionType => transactionType.Id == id)
				.ExecuteDeleteAsync(cancellationToken);
		}
		catch (Exception e)
		{
			_logger.LogWarning(e, "Failed to delete transaction type with Id: {TransactionTypeId}", id);
		}
		finally
		{
			_semaphoreSlim.Release();
		}

		return rowsDeleted;
	}

	public async ValueTask UpdateAsync(TransactionType transactionType, CancellationToken cancellationToken)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);

		try
		{
			await _dbContext.TransactionTypes
				.Where(tp => tp.Id == transactionType.Id)
				.ExecuteUpdateAsync(tp => tp
					.SetProperty(p => p.Name, transactionType.Name)
					.SetProperty(p => p.IsIncome, transactionType.IsIncome), cancellationToken);
		}
		catch (Exception e)
		{
			_logger.LogWarning(e,
				"Failed to update transaction type with Id: {TransactionTypeId}, Name: {TransactionTypeName}",
				transactionType.Id, transactionType.Name);
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	public async ValueTask<TransactionType?> GetByCodeAsync(string code, CancellationToken cancellationToken)
	{
		return await _dbContext.TransactionTypes
			.AsNoTracking()
			.FirstOrDefaultAsync(tp => tp.Code == code, cancellationToken);
	}
}