using FluentValidation;
using KutokAccounting.Services.Transactions.Models;

namespace KutokAccounting.Services.Transactions.Validators;

public class TransactionFiltersValidator : AbstractValidator<Filters>
{
	public TransactionFiltersValidator()
	{
		RuleFor(f => f.Name)
			.MaximumLength(100);

		RuleFor(f => f.Description)
			.MaximumLength(1024);

		RuleFor(f => f.StoreId)
			.GreaterThan(0);

		RuleFor(f => f.TransactionTypeId)
			.GreaterThan(0)
			.When(f => f.TransactionTypeId.HasValue);

		RuleFor(f => f.Value)
			.Must(m => m.Value.Value >= 0)
			.When(f => f.Value.HasValue);

		RuleFor(f => f.MoreThan)
			.Must(m => m.Value.Value >= 0)
			.When(f => f.MoreThan.HasValue);

		RuleFor(f => f.LessThan)
			.Must(m => m.Value.Value >= 0)
			.When(f => f.LessThan.HasValue);

		RuleFor(f => f.Range)
			.Must(r =>
				r.Value.StartOfRange < r.Value.EndOfRange)
			.When(f => f.Range.HasValue);
	}
}