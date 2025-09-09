using FluentValidation;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Vendors.DataTransferObjects;

namespace KutokAccounting.Services.Vendors;

public class VendorService : IVendorService
{
    private readonly IVendorRepository _repository;
    private readonly AbstractValidator<VendorDto> _validator;
    
    public VendorService(IVendorRepository repository, AbstractValidator<VendorDto> validator)
    {
        _repository = repository;
        _validator = validator;
    }
    
    public async ValueTask<Vendor> CreateAsync(VendorDto request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
            throw new Exception(validationResult.ToString());

        var vendor = new Vendor()
        {
            Name = request.Name,
            Description = request.Description
        };

        await _repository.CreateAsync(vendor, cancellationToken);

        return vendor;
    }

    public async ValueTask<Vendor> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var vendor = await _repository.GetByIdAsync(id, cancellationToken);

        return vendor ?? throw new Exception("Vendor not found");
    }

    public async ValueTask<List<Vendor>> GetAsync(QueryParameters queryParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var vendors = await _repository.GetVendorsAsync(cancellationToken);

        return vendors ?? throw new Exception("Vendors list is empty");
    }

    public async ValueTask UpdateAsync(VendorDto request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new Exception(validationResult.ToString());

        var vendor = new Vendor()
        {
            Name = request.Name,
            Description = request.Description
        };
        
        await _repository.UpdateAsync(vendor, cancellationToken);
    }

    public async ValueTask DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await _repository.DeleteAsync(id, cancellationToken);
    }
}