using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.TransactionTypes.Models;

namespace KutokAccounting.Services.TransactionTypes.Interfaces;

public interface ITransactionTypeRepository
{
	ValueTask CreateAsync(TransactionType transactionType, CancellationToken cancellationToken);
	ValueTask<TransactionType> GetByIdAsync(int id, CancellationToken cancellationToken);

	ValueTask<PagedResult<TransactionType>> GetAsync(TransactionTypeQueryParameters parameters,
		CancellationToken cancellationToken);

	ValueTask<int> DeleteAsync(int id, CancellationToken cancellationToken);
	ValueTask UpdateAsync(TransactionType transactionType, CancellationToken cancellationToken);

	ValueTask<TransactionType?> GetByCodeAsync(string code, CancellationToken cancellationToken);
}