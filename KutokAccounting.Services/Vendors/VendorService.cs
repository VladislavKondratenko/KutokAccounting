using FluentValidation;
using FluentValidation.Results;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.TransactionTypes.Exceptions;
using KutokAccounting.Services.Vendors.Models;
using Microsoft.Extensions.Logging;

namespace KutokAccounting.Services.Vendors;

public sealed class VendorService : IVendorService
{
	private readonly ILogger<VendorService> _logger;
	private readonly IValidator<VendorQueryParameters> _vendorQueryValidator;
	private readonly IVendorRepository _repository;
	private readonly IValidator<VendorDto> _vendorDtoValidator;

	public VendorService(
		IVendorRepository repository,
		IValidator<VendorDto> vendorDtoValidator,
		IValidator<VendorQueryParameters> vendorQueryValidator,
		ILogger<VendorService> logger)
	{
		_repository = repository;
		_vendorDtoValidator = vendorDtoValidator;
		_logger = logger;
		_vendorQueryValidator = vendorQueryValidator;
	}

	public async ValueTask<Vendor> CreateAsync(VendorDto request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		ValidationResult validationResult = await _vendorDtoValidator.ValidateAsync(request, cancellationToken);

		if (validationResult.IsValid is false)
		{
			_logger.LogWarning("Vendor creation request validation failed. Errors: {Errors}", validationResult.Errors);

			throw new ArgumentException(validationResult.ToString());
		}

		_logger.LogInformation("Validation succeeded for vendor {VendorName}", request.Name);

		Vendor vendor = new()
		{
			Name = request.Name,
			Description = request.Description
		};

		//TODO: подумать о том как привести это в порядок.
		_logger.LogInformation("Saving vendor to repository. Name: {VendorName}", vendor.Name);

		await _repository.CreateAsync(vendor, cancellationToken);

		_logger.LogInformation("Vendor {VendorName} successfully created", vendor.Name);

		return vendor;
	}

	public async ValueTask<Vendor> GetByIdAsync(int id, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		_logger.LogInformation("Fetching vendor by ID: {VendorId}", id);

		Vendor vendor = await _repository.GetByIdAsync(id, cancellationToken);

		if (vendor is null)
		{
			_logger.LogWarning("Vendor with ID {VendorId} not found", id);

			throw new NotFoundException("Vendor not found");
		}

		_logger.LogInformation("Vendor with ID {VendorId} retrieved successfully. Name: {VendorName}", vendor.Id,
			vendor.Name);

		return vendor;
	}

	public async ValueTask<PagedResult<Vendor>> GetAsync(VendorQueryParameters vendorQueryParameters,
		CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		_logger.LogInformation("Fetching vendors with query parameters: {QueryParameters}", vendorQueryParameters);

		ValidationResult validationResult =
			await _vendorQueryValidator.ValidateAsync(vendorQueryParameters, cancellationToken);

		if (validationResult.IsValid is false)
		{
			_logger.LogWarning("Query parameters validation failed. Errors: {Errors}", validationResult.Errors);

			throw new ArgumentException(validationResult.ToString());
		}

		PagedResult<Vendor> vendors = await _repository.GetAsync(vendorQueryParameters, cancellationToken);

		if (vendors is {Count: 0})
		{
			_logger.LogWarning("No vendors found for query parameters: {@QueryParameters}", vendorQueryParameters);

			throw new Exception("Vendors list is empty");
		}

		_logger.LogInformation("Retrieved {Count} vendors successfully", vendors.Count);

		return vendors;
	}

	public async ValueTask UpdateAsync(VendorDto request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		_logger.LogInformation("Updating vendor with data: {VendorRequest}", request);

		ValidationResult validationResult = await _vendorDtoValidator.ValidateAsync(request, cancellationToken);

		if (validationResult.IsValid is false)
		{
			_logger.LogWarning("Vendor update validation failed: {ValidationErrors}", validationResult.Errors);

			throw new Exception(validationResult.ToString());
		}

		_logger.LogDebug("Validation succeeded for vendor update: {VendorName}", request.Name);

		Vendor vendor = new()
		{
			Id = request.Id,
			Name = request.Name,
			Description = request.Description
		};

		await _repository.UpdateAsync(vendor, cancellationToken);

		_logger.LogInformation("Vendor {VendorName} updated successfully", vendor.Name);
	}

	public async ValueTask DeleteAsync(int id, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		await _repository.DeleteAsync(id, cancellationToken);

		_logger.LogInformation("Vendor with ID {VendorId} deleted successfully", id);
	}
}