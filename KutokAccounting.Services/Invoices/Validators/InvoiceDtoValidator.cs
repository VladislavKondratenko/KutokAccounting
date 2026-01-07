using FluentValidation;
using KutokAccounting.Services.Invoices.Models;

namespace KutokAccounting.Services.Invoices.Validators;

public class InvoiceDtoValidator : AbstractValidator<InvoiceDto>
{
	public InvoiceDtoValidator()
	{
		RuleFor(i => i.Number)
			.MaximumLength(100);

		RuleFor(i => i.VendorId)
			.GreaterThan(0);

		RuleFor(i => i.StoreId)
			.GreaterThan(0);

		RuleFor(t => t.Money.Value)
			.GreaterThanOrEqualTo(0);
	}
}