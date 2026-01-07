using FluentValidation;
using FluentValidation.Results;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.TransactionTypes.Exceptions;
using KutokAccounting.Services.TransactionTypes.Interfaces;
using KutokAccounting.Services.TransactionTypes.Models;
using Microsoft.Extensions.Logging;

namespace KutokAccounting.Services.TransactionTypes;

public sealed class TransactionTypeService : ITransactionTypeService
{
	private readonly ILogger<TransactionTypeService> _logger;
	private readonly IValidator<TransactionTypeDto> _transactionTypeDtoValidator;
	private readonly IValidator<TransactionTypeQueryParameters> _transactionTypeQueryValidator;
	private readonly ITransactionTypeRepository _repository;

	public TransactionTypeService(
		ITransactionTypeRepository repository,
		ILogger<TransactionTypeService> logger,
		IValidator<TransactionTypeDto> transactionTypeDtoValidator,
		IValidator<TransactionTypeQueryParameters> transactionTypeQueryValidator)
	{
		_repository = repository;
		_logger = logger;
		_transactionTypeDtoValidator = transactionTypeDtoValidator;
		_transactionTypeQueryValidator = transactionTypeQueryValidator;
	}

	public async ValueTask<TransactionType> CreateAsync(TransactionTypeDto request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		ValidationResult validationResult =
			await _transactionTypeDtoValidator.ValidateAsync(request, cancellationToken);

		if (validationResult.IsValid is false)
		{
			_logger.LogWarning("Transaction type creation request validation failed. Errors: {Errors}",
				validationResult.Errors);

			throw new ArgumentException(validationResult.ToString());
		}

		_logger.LogInformation("Validation succeeded for vendor {TransactionTypeName}", request.Name);

		TransactionType transactionType = new()
		{
			Name = request.Name,
			IsIncome = request.IsIncome
		};

		_logger.LogInformation("Saving transaction type to repository. Name: {TransactionTypeName}",
			transactionType.Name);

		await _repository.CreateAsync(transactionType, cancellationToken);

		_logger.LogInformation(
			"Transaction type {TransactionTypeName} successfully created",
			transactionType.Name);

		return transactionType;
	}

	public async ValueTask<TransactionType> GetByIdAsync(int id, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		_logger.LogInformation("Fetching transaction type by ID: {TransactionTypeId}", id);

		TransactionType? transactionType = await _repository.GetByIdAsync(id, cancellationToken);

		if (transactionType is null)
		{
			_logger.LogWarning("Transaction type with ID {TransactionTypeId} not found", id);

			throw new NotFoundException("Transaction type not found");
		}

		_logger.LogInformation(
			"Transaction type with ID {TransactionTypeId} retrieved successfully. Name: {TransactionTypeName}",
			transactionType.Id,
			transactionType.Name);

		return transactionType;
	}

	public async ValueTask<PagedResult<TransactionType>> GetAsync(
		TransactionTypeQueryParameters transactionTypeQueryParameters,
		CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		ValidationResult validationResult =
			await _transactionTypeQueryValidator.ValidateAsync(transactionTypeQueryParameters, cancellationToken);

		if (validationResult.IsValid is false)
		{
			_logger.LogWarning("Query parameters for transaction type validation failed. Errors: {Errors}",
				validationResult.Errors);

			throw new ArgumentException(validationResult.ToString());
		}

		PagedResult<TransactionType> transactionTypes =
			await _repository.GetAsync(transactionTypeQueryParameters, cancellationToken);

		if (transactionTypes is {Count: 0})
		{
			_logger.LogWarning("No transaction types found for query parameters: {@QueryParameters}",
				transactionTypeQueryParameters);

			throw new Exception("Transaction types list is empty");
		}

		_logger.LogInformation("Retrieved {Count} transaction types successfully", transactionTypes.Count);

		return transactionTypes;
	}

	public async ValueTask UpdateAsync(TransactionTypeDto request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		_logger.LogInformation("Updating transaction type with data: {TransactionTypeRequest}", request);

		ValidationResult validationResult =
			await _transactionTypeDtoValidator.ValidateAsync(request, cancellationToken);

		if (validationResult.IsValid is false)
		{
			_logger.LogWarning("Transaction type update validation failed: {ValidationErrors}",
				validationResult.Errors);

			throw new Exception(validationResult.ToString());
		}

		_logger.LogDebug("Validation succeeded for transaction type update: {TransactionTypeName}", request.Name);

		TransactionType transactionType = new()
		{
			Id = request.Id,
			Name = request.Name,
			IsIncome = request.IsIncome
		};

		await _repository.UpdateAsync(transactionType, cancellationToken);

		_logger.LogInformation("Transaction type {TransactionTypeName} updated successfully", transactionType.Name);
	}

	public async ValueTask<int> DeleteAsync(int id, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		int rowsDeleted = await _repository.DeleteAsync(id, cancellationToken);

		_logger.LogInformation("Transaction type with ID {TransactionTypeId} deleted successfully", id);

		return rowsDeleted;
	}

	public async ValueTask<TransactionType> GetByCodeAsync(string code, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		_logger.LogInformation("Fetching transaction type by Code: {TransactionTypeCode}", code);

		TransactionType? transactionType = await _repository.GetByCodeAsync(code, cancellationToken);

		if (transactionType is null)
		{
			_logger.LogWarning("Transaction type with Code {TransactionTypeCode} not found", code);

			throw new NotFoundException("Transaction type not found");
		}

		_logger.LogInformation(
			"Transaction type with Code {TransactionTypeCode} retrieved successfully. Name: {TransactionTypeName}",
			transactionType.Id,
			transactionType.Name);

		return transactionType;
	}
}