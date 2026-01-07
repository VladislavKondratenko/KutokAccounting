using KutokAccounting.Services.Invoices.Models;

namespace KutokAccounting.Services.Invoices.Interfaces;

public interface IInvoiceLifecycleService
{
	//Create
	ValueTask OpenInvoiceAsync(InvoiceDto request, CancellationToken cancellationToken);

	//CloseAsync
	ValueTask CloseInvoiceAsync(InvoiceDto request, CancellationToken cancellationToken);

	//UpdateAsync
	ValueTask EditInvoiceAsync(InvoiceDto request, CancellationToken cancellationToken);
}