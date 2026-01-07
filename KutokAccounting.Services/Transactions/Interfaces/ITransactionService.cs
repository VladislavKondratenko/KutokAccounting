using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Transactions.Models;

namespace KutokAccounting.Services.Transactions.Interfaces;

public interface ITransactionService
{
	ValueTask<PagedResult<Transaction>> GetAsync(TransactionQueryParameters transactionQueryParameters,
		CancellationToken cancellationToken);

	ValueTask CalculateAsync(CalculationResult result,
		CalculationQueryParameters parameters,
		CancellationToken cancellationToken);

	ValueTask<Transaction> GetByIdAsync(int id, CancellationToken cancellationToken);
	ValueTask<Transaction> CreateAsync(TransactionDto request, CancellationToken cancellationToken);
	ValueTask UpdateAsync(TransactionDto request, CancellationToken cancellationToken);
	ValueTask DeleteAsync(int id, CancellationToken cancellationToken);
}