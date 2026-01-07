using FluentValidation;
using KutokAccounting.Services.Transactions.Models;

namespace KutokAccounting.Services.Transactions.Validators;

public class TransactionSortingValidator : AbstractValidator<Sorting>
{
	public TransactionSortingValidator()
	{
		RuleFor(s => s.SortBy)
			.MaximumLength(100);
	}
}