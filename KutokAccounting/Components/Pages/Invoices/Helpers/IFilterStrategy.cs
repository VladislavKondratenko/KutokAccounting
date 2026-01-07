using KutokAccounting.Components.Pages.Invoices.Models;
using KutokAccounting.Services.Invoices.Models;
using MudBlazor;

namespace KutokAccounting.Components.Pages.Invoices.Helpers;

public interface IFilterStrategy
{
	void Apply(Filters filters, IFilterDefinition<InvoiceView> filterDefinition);
}