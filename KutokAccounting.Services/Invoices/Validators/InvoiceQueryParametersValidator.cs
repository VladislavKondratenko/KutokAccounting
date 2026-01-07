using FluentValidation;
using KutokAccounting.Services.Invoices.Models;

namespace KutokAccounting.Services.Invoices.Validators;

public class InvoiceQueryParametersValidator : AbstractValidator<InvoiceQueryParameters>
{
	public InvoiceQueryParametersValidator()
	{
		RuleFor(p => p.Pagination.Page)
			.GreaterThan(0)
			.LessThanOrEqualTo(100);

		RuleFor(p => p.Pagination.PageSize)
			.GreaterThanOrEqualTo(10);

		When(p => p.Filters is not null,
			() =>
			{
				RuleFor(p => p.Filters)
					.SetValidator(new InvoiceFiltersValidator());
			});

		When(p => p.Sorting is not null,
			() =>
			{
				RuleFor(p => p.Sorting)
					.SetValidator(new InvoiceSortingValidator());
			});

		When(p => p.SearchString is not null, () =>
		{
			RuleFor(p => p.SearchString)
				.MaximumLength(100);
		});
	}
}