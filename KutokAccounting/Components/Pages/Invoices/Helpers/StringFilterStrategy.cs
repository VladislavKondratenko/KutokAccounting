using KutokAccounting.Components.Pages.Invoices.Models;
using KutokAccounting.Services.Invoices.Models;
using MudBlazor;

namespace KutokAccounting.Components.Pages.Invoices.Helpers;

public class StringFilterStrategy : IFilterStrategy
{
	private readonly Action<Filters, string> _applyAction;

	public StringFilterStrategy(Action<Filters, string> applyAction)
	{
		_applyAction = applyAction;
	}

	public void Apply(Filters filters, IFilterDefinition<InvoiceView> filterDefinition)
	{
		string? value = filterDefinition.Value?.ToString();

		if (string.IsNullOrWhiteSpace(value) is false)
		{
			_applyAction(filters, value);
		}
	}
}