using FluentValidation;
using KutokAccounting.Services.Invoices.Models;

namespace KutokAccounting.Services.Invoices.Validators;

public class InvoiceFiltersValidator : AbstractValidator<Filters>
{
	public InvoiceFiltersValidator()
	{
		RuleFor(f => f.Number)
			.MaximumLength(100);

		RuleFor(f => f.StoreId)
			.GreaterThan(0);

		RuleFor(f => f.VendorId)
			.GreaterThan(0)
			.When(f => f.VendorId.HasValue);

		RuleFor(f => f.Range)
			.Must(r => r.Value.StartOfRange < r.Value.EndOfRange)
			.When(f => f.Range.HasValue);
	}
}