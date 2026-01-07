using KutokAccounting.Components.Pages.Invoices.Models;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Invoices.Models;
using MudBlazor;

namespace KutokAccounting.Components.Pages.Invoices.Helpers;

public class StateFilterStrategy : IFilterStrategy
{
	private readonly Action<Filters, bool> _applyAction;

	public StateFilterStrategy(Action<Filters, bool> applyAction)
	{
		_applyAction = applyAction;
	}

	public void Apply(Filters filters, IFilterDefinition<InvoiceView> filterDefinition)
	{
		State? state = Enum.Parse<State>(filterDefinition.Value?.ToString());

		if (state is not null)
		{
			_applyAction(filters, state is State.Closed);
		}
	}
}