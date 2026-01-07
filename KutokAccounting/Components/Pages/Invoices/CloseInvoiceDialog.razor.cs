using KutokAccounting.Components.Pages.Invoices.Models;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Invoices.Interfaces;
using KutokAccounting.Services.Invoices.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace KutokAccounting.Components.Pages.Invoices;

public partial class CloseInvoiceDialog : ComponentBase
{
	private string _value = string.Empty;
	private bool _isSuccess;

	[Inject]
	public required IInvoiceService InvoiceService { get; set; }

	[CascadingParameter]
	public required IMudDialogInstance MudDialog { get; set; }

	[Parameter]
	public required int StoreId { get; set; }

	[Parameter]
	public required InvoiceView Invoice { get; set; }

	public async Task CloseAsync()
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		InvoiceDto invoice = new()
		{
			Id = Invoice.Id,
			Number = Invoice.Number,
			StoreId = StoreId,
			VendorId = Invoice.Vendor.Id,
			Money = Money.Parse(_value)
		};

		await InvoiceService.CloseAsync(invoice, tokenSource.Token);

		MudDialog.Close(DialogResult.Ok(true));
	}

	private string ValidateValue(string value)
	{
		return MoneyFormatRegex.MoneyValueRegex().IsMatch(value)
			? string.Empty
			: "Значення повинно бути додатним числом з двома знаками після точки (наприклад, 123.45 або 123,45).";
	}

	private void Cancel()
	{
		MudDialog.Cancel();
	}
}