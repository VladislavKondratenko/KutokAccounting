using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Vendors.DataTransferObjects;
using KutokAccounting.Services.Vendors.Validators;
using Microsoft.Extensions.Logging;

namespace KutokAccounting.Services.Vendors;

public sealed class VendorService : IVendorService
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly QueryParametersValidator _queryValidator;
    private readonly IVendorRepository _repository;
    private readonly VendorDtoValidator _vendorModelValidator;

    public VendorService(IVendorRepository repository, VendorDtoValidator vendorModelValidator,
        QueryParametersValidator queryValidator, ILoggerFactory loggerFactory)
    {
        _repository = repository;
        _vendorModelValidator = vendorModelValidator;
        _loggerFactory = loggerFactory;
        _queryValidator = queryValidator;
    }

    public async ValueTask<Vendor> CreateAsync(VendorDto request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var logger = _loggerFactory.CreateLogger<VendorService>();

        var validationResult = await _vendorModelValidator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid is false)
        {
            logger.LogWarning("Vendor creation request validation failed. Errors: {Errors}", validationResult.Errors);

            throw new ArgumentException(validationResult.ToString());
        }

        logger.LogInformation("Validation succeeded for vendor {VendorName}", request.Name);

        var vendor = new Vendor
        {
            Name = request.Name,
            Description = request.Description
        };

        logger.LogInformation("Saving vendor to repository. Name: {VendorName}", vendor.Name);

        await _repository.CreateAsync(vendor, cancellationToken);

        logger.LogInformation("Vendor {VendorName} successfully created with ID {VendorId}", vendor.Name, vendor.Id);

        return vendor;
    }

    public async ValueTask<Vendor> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var logger = _loggerFactory.CreateLogger<VendorService>();

        logger.LogInformation("Fetching vendor by ID: {VendorId}", id);

        var vendor = await _repository.GetByIdAsync(id, cancellationToken);

        if (vendor == null)
        {
            logger.LogWarning("Vendor with ID {VendorId} not found", id);

            throw new Exception("Vendor not found");
        }

        logger.LogInformation("Vendor with ID {VendorId} retrieved successfully. Name: {VendorName}", vendor.Id,
            vendor.Name);

        return vendor;
    }

    public async ValueTask<VendorPagedResult> GetAsync(QueryParameters queryParameters,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var logger = _loggerFactory.CreateLogger<VendorService>();

        logger.LogInformation("Fetching vendors with query parameters: {QueryParameters}", queryParameters);

        var validationResult = await _queryValidator.ValidateAsync(queryParameters, cancellationToken);

        if (validationResult.IsValid is false)
        {
            logger.LogWarning("Query parameters validation failed. Errors: {Errors}", validationResult.Errors);

            throw new Exception(validationResult.ToString());
        }

        var vendors = await _repository.GetAsync(queryParameters, cancellationToken);

        if (vendors == null || vendors.Count == 0)
        {
            logger.LogWarning("No vendors found for query parameters: {@QueryParameters}", queryParameters);

            throw new Exception("Vendors list is empty");
        }

        logger.LogInformation("Retrieved {Count} vendors successfully", vendors.Count);

        return vendors;
    }

    public async ValueTask UpdateAsync(VendorDto request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var logger = _loggerFactory.CreateLogger<VendorService>();

        logger.LogInformation("Updating vendor with data: {VendorRequest}", request);

        var validationResult = await _vendorModelValidator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid is false)
        {
            logger.LogWarning("Vendor update validation failed: {ValidationErrors}", validationResult.Errors);

            throw new Exception(validationResult.ToString());
        }

        logger.LogDebug("Validation succeeded for vendor update: {VendorName}", request.Name);

        var vendor = new Vendor
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description
        };

        await _repository.UpdateAsync(vendor, cancellationToken);

        logger.LogInformation("Vendor {VendorName} updated successfully", vendor.Name);
    }

    public async ValueTask DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var logger = _loggerFactory.CreateLogger<VendorService>();

        await _repository.DeleteAsync(id, cancellationToken);

        logger.LogInformation("Vendor with ID {VendorId} deleted successfully", id);
    }
}