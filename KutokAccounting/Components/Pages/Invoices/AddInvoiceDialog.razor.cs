using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Invoices.Interfaces;
using KutokAccounting.Services.Invoices.Models;
using KutokAccounting.Services.Vendors;
using KutokAccounting.Services.Vendors.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace KutokAccounting.Components.Pages.Invoices;

public partial class AddInvoiceDialog : ComponentBase
{
	private string _number = string.Empty;
	private Vendor _vendor;
	private bool _isSuccess;
	
	[Inject]
	public required IInvoiceService InvoiceService { get; set; }
	
	[Inject]
	public required IVendorService VendorService { get; set; }

	[CascadingParameter]
	public required IMudDialogInstance MudDialog { get; set; }

	[Parameter]
	public int StoreId { get; set; }

	private async Task OpenAsync()
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		InvoiceDto invoice = new()
		{
			Number = _number,
			StoreId = StoreId,
			VendorId = _vendor.Id,
		};

		await InvoiceService.CreateAsync(invoice, tokenSource.Token);

		MudDialog.Close(DialogResult.Ok(true));
	}

	private async Task<IEnumerable<Vendor>> SearchAsync(string value, CancellationToken cancellationToken)
	{
		VendorQueryParameters parameters = new(null, Pagination: new Pagination
		{
			Page = 1,
			PageSize = 10
		}, SearchString: value);

		PagedResult<Vendor> pagedResult = await VendorService.GetAsync(parameters, cancellationToken);

		return pagedResult.Items;
	}
	
	private void Cancel()
	{
		MudDialog.Close();
	}
}