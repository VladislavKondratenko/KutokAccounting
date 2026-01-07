using FluentValidation;
using FluentValidation.Results;
using KutokAccounting.DataProvider;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Invoices.Interfaces;
using KutokAccounting.Services.Invoices.Models;
using KutokAccounting.Services.Transactions.Interfaces;
using KutokAccounting.Services.TransactionTypes.Exceptions;
using Microsoft.Extensions.Logging;

namespace KutokAccounting.Services.Invoices;

public class InvoiceService : IInvoiceService
{
	private readonly ITransactionRepository _transactionRepository;
	private readonly IInvoiceRepository _invoiceRepository;
	private readonly ILogger<InvoiceService> _logger;
	private readonly IValidator<InvoiceDto> _invoiceDtoValidator;
	private readonly IValidator<InvoiceQueryParameters> _invoiceQueryValidator;

	public InvoiceService(
		ITransactionRepository transactionRepository,
		IInvoiceRepository invoiceRepository,
		ILogger<InvoiceService> logger,
		IValidator<InvoiceDto> invoiceDtoValidator,
		IValidator<InvoiceQueryParameters> invoiceQueryValidator)
	{
		_transactionRepository = transactionRepository;
		_invoiceRepository = invoiceRepository;
		_logger = logger;
		_invoiceDtoValidator = invoiceDtoValidator;
		_invoiceQueryValidator = invoiceQueryValidator;
	}

	public async ValueTask<PagedResult<Invoice>> GetAsync(InvoiceQueryParameters parameters,
		CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		ValidationResult validationResult = await _invoiceQueryValidator.ValidateAsync(parameters, cancellationToken);

		if (validationResult.IsValid is false)
		{
			_logger.LogWarning("Query parameters for invoice validation failed. Errors: {Errors}",
				validationResult.Errors);

			throw new ArgumentException(validationResult.ToString());
		}

		PagedResult<Invoice> invoices = await _invoiceRepository.GetAsync(parameters, cancellationToken);

		if (invoices is {Count: 0})
		{
			_logger.LogWarning("No invoices found for query parameters: {@Parameters}",
				parameters);

			throw new Exception("Invoice list is empty");
		}

		_logger.LogInformation("Retrieved {Count} invoices successfully", invoices.Count);

		return invoices;
	}

	public async ValueTask<Invoice> GetByIdAsync(int id, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		_logger.LogInformation("Fetching invoice by ID: {InvoiceId}", id);

		Invoice? invoice = await _invoiceRepository.GetByIdAsync(id, cancellationToken);

		if (invoice is null)
		{
			_logger.LogWarning("Invoice with ID {InvoiceId} not found", id);

			throw new NotFoundException("Invoice not found");
		}

		_logger.LogInformation(
			"Invoice with ID {InvoiceId} retrieved successfully. Number: {InvoiceNumber}",
			invoice.Id,
			invoice.Number);

		return invoice;
	}

	public async ValueTask<Invoice> CreateAsync(InvoiceDto request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		ValidationResult validationResult =
			await _invoiceDtoValidator.ValidateAsync(request, cancellationToken);

		if (validationResult.IsValid is false)
		{
			_logger.LogWarning("Invoice creation request validation failed. Errors: {Errors}",
				validationResult.Errors);

			throw new ArgumentException(validationResult.ToString());
		}

		_logger.LogInformation("Validation succeeded for invoice {InvoiceNumber}", request.Number);

		Invoice invoice = new()
		{
			CreatedAt = DateTime.Now,
			Number = request.Number,
			StoreId = request.StoreId,
			VendorId = request.VendorId,
			StatusHistory =
			[
				new InvoiceStatus
				{
					CreatedAt = DateTime.Now,
					State = State.Active
				}
			]
		};

		_logger.LogInformation("Saving invoice to repository. Name: {InvoiceNumber}",
			invoice.Number);

		await _invoiceRepository.CreateAsync(invoice, cancellationToken);

		_logger.LogInformation(
			"Invoice {InvoiecNumber} successfully created",
			invoice.Number);

		return invoice;
	}

	public async ValueTask UpdateAsync(InvoiceDto request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		_logger.LogInformation("Updating invoice with data: {InvoiceRequest}", request);

		ValidationResult validationResult =
			await _invoiceDtoValidator.ValidateAsync(request, cancellationToken);

		if (validationResult.IsValid is false)
		{
			_logger.LogWarning("Invoice update validation failed: {ValidationErrors}",
				validationResult.Errors);

			throw new Exception(validationResult.ToString());
		}

		_logger.LogDebug("Validation succeeded for invoice update: {InvoiceNumber}", request.Number);

		Invoice invoice = new()
		{
			Number = request.Number,
			VendorId = request.VendorId
		};

		await _invoiceRepository.UpdateAsync(invoice, cancellationToken);

		_logger.LogInformation("Invoice {InvoiceNumber} updated successfully", invoice.Number);
	}

	public async ValueTask DeleteAsync(int id, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		await _invoiceRepository.DeleteAsync(id, cancellationToken);

		_logger.LogInformation("Invoice with ID {InvoiceId} deleted successfully", id);
	}

	public async ValueTask CloseAsync(InvoiceDto request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		ValidationResult validationResult = await _invoiceDtoValidator.ValidateAsync(request, cancellationToken);

		if (validationResult.IsValid is false)
		{
			_logger.LogWarning("Invoice close validation failed {ValidationErrors}", validationResult.Errors);

			throw new ValidationException(validationResult.ToString());
		}

		await _invoiceRepository.CloseAsync(request.Id, cancellationToken);

		Transaction transaction = new()
		{
			Name = $"{request.Number}",
			Description = $"{request.Number}",
			Money = request.Money,
			CreatedAt = DateTime.Now,
			StoreId = request.StoreId,
			TransactionType = new TransactionType
			{
				Name = "СЛУЖБОВИЙ ТИП ТРАНЗАКЦІЇ",
				IsIncome = false,
				Code = KutokConfigurations.CloseInvoiceTransactionTypeCode
			},
			InvoiceId = request.Id
		};

		await _transactionRepository.CreateAsync(transaction, cancellationToken);
	}
}