using FluentValidation;
using FluentValidation.Results;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Transactions.Interfaces;
using KutokAccounting.Services.Transactions.Models;
using KutokAccounting.Services.TransactionTypes.Exceptions;
using Microsoft.Extensions.Logging;

namespace KutokAccounting.Services.Transactions;

public sealed class TransactionService : ITransactionService
{
	private readonly ITransactionRepository _repository;
	private readonly ILogger<Transaction> _logger;
	private readonly IValidator<TransactionDto> _transactionDtoValidator;
	private readonly IValidator<TransactionQueryParameters> _transactionQueryValidator;

	public TransactionService(
		ITransactionRepository repository,
		ILogger<Transaction> logger,
		IValidator<TransactionDto> transactionDtoValidator,
		IValidator<TransactionQueryParameters> transactionQueryValidator)
	{
		_repository = repository;
		_logger = logger;
		_transactionDtoValidator = transactionDtoValidator;
		_transactionQueryValidator = transactionQueryValidator;
	}

	public async ValueTask<Transaction> CreateAsync(TransactionDto request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		ValidationResult validationResult = await _transactionDtoValidator.ValidateAsync(request, cancellationToken);

		if (validationResult.IsValid is false)
		{
			_logger.LogWarning("Transaction creation request validation failed. Errors: {Errors}",
				validationResult.Errors);

			throw new ArgumentException(validationResult.ToString());
		}

		_logger.LogInformation("Validation succeeded for transaction {TransactionName}", request.Name);

		Transaction transaction = new()
		{
			Name = request.Name,
			Description = request.Description,
			Money = new Money(request.Value),
			CreatedAt = DateTime.Now,
			StoreId = request.StoreId,
			TransactionTypeId = request.TransactionTypeId,
			InvoiceId = request.InvoiceId
		};

		_logger.LogInformation("Saving transaction to repository. Name: {TransactionName}",
			transaction.Name);

		await _repository.CreateAsync(transaction, cancellationToken);

		_logger.LogInformation(
			"Transaction  {TransactionName} successfully created",
			transaction.Name);

		return transaction;
	}

	public async ValueTask<PagedResult<Transaction>> GetAsync(TransactionQueryParameters transactionQueryParameters,
		CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		ValidationResult validationResult =
			await _transactionQueryValidator.ValidateAsync(transactionQueryParameters, cancellationToken);

		if (validationResult.IsValid is false)
		{
			_logger.LogWarning("Query parameters for transaction validation failed. Errors: {Errors}",
				validationResult.Errors);

			throw new ArgumentException(validationResult.ToString());
		}

		PagedResult<Transaction> transactions =
			await _repository.GetAsync(transactionQueryParameters, cancellationToken);

		if (transactions is {Count: 0})
		{
			_logger.LogWarning("No transactions found for query parameters: {@QueryParameters}",
				transactionQueryParameters);

			throw new Exception("Transaction list is empty");
		}

		_logger.LogInformation("Retrieved {Count} transactions successfully", transactions.Count);

		return transactions;
	}

	public async ValueTask CalculateAsync(CalculationResult result,
		CalculationQueryParameters parameters,
		CancellationToken cancellationToken)
	{
		if (parameters.Range.HasValue is false)
		{
			return;
		}

		cancellationToken.ThrowIfCancellationRequested();

		IAsyncEnumerable<TransactionCalculationView> transactions =
			_repository.EnumerateTransactionsAsync(parameters, cancellationToken);

		int counter = 0;

		await foreach (TransactionCalculationView transactionView in transactions)
		{
			await Task.Yield();

			Interlocked.Increment(ref counter);

			if (counter % 5 == 0)
			{
				await Task.Delay(1, cancellationToken);
			}

			if (transactionView.Sign)
			{
				result.Income += transactionView.Money;
				result.Profit += transactionView.Money;
			}
			else
			{
				result.Expense += transactionView.Money;
				result.Profit -= transactionView.Money;
			}
		}
	}

	public async ValueTask<Transaction> GetByIdAsync(int id, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		_logger.LogInformation("Fetching transaction by ID: {TransactionId}", id);

		Transaction? transaction = await _repository.GetByIdAsync(id, cancellationToken);

		if (transaction is null)
		{
			_logger.LogWarning("Transaction with ID {TransactionId} not found", id);

			throw new NotFoundException("Transaction not found");
		}

		_logger.LogInformation(
			"Transaction with ID {TransactionId} retrieved successfully. Name: {TransactionName}",
			transaction.Id,
			transaction.Name);

		return transaction;
	}

	public async ValueTask UpdateAsync(TransactionDto request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		_logger.LogInformation("Updating transaction with data: {TransactionRequest}", request);

		ValidationResult validationResult =
			await _transactionDtoValidator.ValidateAsync(request, cancellationToken);

		if (validationResult.IsValid is false)
		{
			_logger.LogWarning("Transaction update validation failed: {ValidationErrors}",
				validationResult.Errors);

			throw new Exception(validationResult.ToString());
		}

		_logger.LogDebug("Validation succeeded for transaction update: {TransactionName}", request.Name);

		Transaction transaction = new()
		{
			Id = request.Id,
			Name = request.Name,
			Description = request.Description,
			Money = new Money(request.Value),
			TransactionTypeId = request.TransactionTypeId,
			InvoiceId = request.InvoiceId
		};

		await _repository.UpdateAsync(transaction, cancellationToken);

		_logger.LogInformation("Transaction {TransactionName} updated successfully", transaction.Name);
	}

	public async ValueTask DeleteAsync(int id, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		await _repository.DeleteAsync(id, cancellationToken);

		_logger.LogInformation("Transaction with ID {TransactionId} deleted successfully", id);
	}
}