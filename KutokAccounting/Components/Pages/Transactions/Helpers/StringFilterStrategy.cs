using KutokAccounting.Components.Pages.Transactions.Models;
using KutokAccounting.Services.Transactions.Models;
using MudBlazor;

namespace KutokAccounting.Components.Pages.Transactions.Helpers;

public class StringFilterStrategy : IFilterStrategy
{
	private readonly Action<Filters, string> _applyAction;

	public StringFilterStrategy(Action<Filters, string> applyAction)
	{
		_applyAction = applyAction;
	}

	public void Apply(Filters filters, IFilterDefinition<TransactionView> filterDefinition)
	{
		string? value = filterDefinition.Value?.ToString();

		if (string.IsNullOrWhiteSpace(value) is false)
		{
			_applyAction(filters, value);
		}
	}
}