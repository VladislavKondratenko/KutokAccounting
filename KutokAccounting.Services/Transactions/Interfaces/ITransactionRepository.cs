using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Transactions.Models;

namespace KutokAccounting.Services.Transactions.Interfaces;

public interface ITransactionRepository
{
	ValueTask<PagedResult<Transaction>> GetAsync(TransactionQueryParameters parameters,
		CancellationToken cancellationToken);

	IAsyncEnumerable<TransactionCalculationView> EnumerateTransactionsAsync(CalculationQueryParameters parameters,
		CancellationToken cancellationToken);

	ValueTask<Transaction> GetByIdAsync(int id, CancellationToken cancellationToken);
	ValueTask CreateAsync(Transaction transaction, CancellationToken cancellationToken);
	ValueTask UpdateAsync(Transaction transaction, CancellationToken cancellationToken);
	ValueTask DeleteAsync(int id, CancellationToken cancellationToken);
}