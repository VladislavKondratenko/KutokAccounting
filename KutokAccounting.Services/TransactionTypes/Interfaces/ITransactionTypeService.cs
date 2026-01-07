using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.TransactionTypes.Models;

namespace KutokAccounting.Services.TransactionTypes.Interfaces;

public interface ITransactionTypeService
{
	ValueTask<TransactionType> CreateAsync(TransactionTypeDto request, CancellationToken cancellationToken);
	ValueTask<TransactionType> GetByIdAsync(int id, CancellationToken cancellationToken);

	ValueTask<PagedResult<TransactionType>> GetAsync(TransactionTypeQueryParameters transactionTypeQueryParameters,
		CancellationToken cancellationToken);

	ValueTask UpdateAsync(TransactionTypeDto request, CancellationToken cancellationToken);
	ValueTask<int> DeleteAsync(int id, CancellationToken cancellationToken);
	ValueTask<TransactionType> GetByCodeAsync(string code, CancellationToken cancellationToken);
}