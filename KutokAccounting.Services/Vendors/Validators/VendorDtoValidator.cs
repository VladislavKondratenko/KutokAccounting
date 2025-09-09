using FluentValidation;
using KutokAccounting.Services.Vendors.DataTransferObjects;

namespace KutokAccounting.Services.Vendors.Validators;

public sealed class VendorDtoValidator : AbstractValidator<VendorDto>
{
    public VendorDtoValidator()
    {
        RuleFor(r => r.Name)
            .NotNull()
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(r => r.Description)
            .NotNull()
            .NotEmpty()
            .MaximumLength(1024);
    }
}