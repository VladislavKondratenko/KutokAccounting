using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Invoices.Models;

namespace KutokAccounting.Services.Invoices.Interfaces;

public interface IInvoiceRepository
{
	ValueTask<PagedResult<Invoice>> GetAsync(InvoiceQueryParameters parameters,
		CancellationToken cancellationToken);

	ValueTask<Invoice?> GetByIdAsync(int id, CancellationToken cancellationToken);
	ValueTask CreateAsync(Invoice invoice, CancellationToken cancellationToken);
	ValueTask CloseAsync(int id, CancellationToken cancellationToken);
	ValueTask UpdateAsync(Invoice invoice, CancellationToken cancellationToken);
	ValueTask DeleteAsync(int id, CancellationToken cancellationToken);
}