using FluentValidation;
using KutokAccounting.Services.Invoices.Models;

namespace KutokAccounting.Services.Invoices.Validators;

public class InvoiceSortingValidator : AbstractValidator<Sorting>
{
	public InvoiceSortingValidator()
	{
		RuleFor(s => s.SortBy)
			.MaximumLength(100);
	}
}