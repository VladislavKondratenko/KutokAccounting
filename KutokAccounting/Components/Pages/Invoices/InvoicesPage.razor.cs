using KutokAccounting.Components.Pages.Invoices.Helpers;
using KutokAccounting.Components.Pages.Invoices.Models;
using KutokAccounting.Components.Pages.Transactions;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Invoices.Interfaces;
using KutokAccounting.Services.Invoices.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace KutokAccounting.Components.Pages.Invoices;

public partial class InvoicesPage : ComponentBase
{
	private readonly HashSet<string> _stringFilterOperators = ["equals"];
	private MudDataGrid<InvoiceView> _dataGrid = new();
	private string? _searchString;
	private MudDateRangePicker _picker = new();
	private DateRange? _dateRange;

	#region TestingCustomFilterTemplate

	private bool _filterOpen;
	private bool _selectAll = true;
	private HashSet<Vendor> _selectedVendors = new();
	private HashSet<Vendor> _filterVendors = new();
	private readonly IEnumerable<Vendor> _vendors = new List<Vendor>();
	private string _icon = Icons.Material.Outlined.FilterAlt;
	private FilterDefinition<Element> _filterDefinition;

	private void SelectAll(bool value)
	{
		_selectAll = value;

		if (value)
		{
			_selectedVendors = _vendors.ToHashSet();
		}
		else
		{
			_selectedVendors.Clear();
		}
	}

	private void SelectedChanged(bool value, Vendor item)
	{
		if (value)
		{
			_selectedVendors.Add(item);
		}
		else
		{
			_selectedVendors.Remove(item);
		}

		if (_selectedVendors.Count == _vendors.Count())
		{
			_selectAll = true;
		}
		else
		{
			_selectAll = false;
		}
	}

	private async Task ClearFilterAsync(FilterContext<Element> context)
	{
		_selectedVendors = _vendors.ToHashSet();
		_filterVendors = _vendors.ToHashSet();
		_icon = Icons.Material.Outlined.FilterAlt;
		await context.Actions.ClearFilterAsync(_filterDefinition);
		_filterOpen = false;
	}

	#endregion

	[Inject]
	public required IDialogService DialogService { get; set; }

	[Inject]
	public required IInvoiceService InvoiceService { get; set; }

	[Parameter]
	public int StoreId { get; set; }

	public async Task<GridData<InvoiceView>> GetInvoicesAsync(GridState<InvoiceView> state)
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		try
		{
			return await GetInvoicesInternalAsync(state, tokenSource.Token);
		}
		catch (Exception)
		{
			return new GridData<InvoiceView>();
		}
	}

	protected override void OnInitialized()
	{
		if (_dateRange is not null)
		{
			return;
		}

		DateTime now = DateTime.Now;

		_dateRange = new DateRange(new DateTime(now.Year, now.Month, 1),
			new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1));
	}

	private async ValueTask<GridData<InvoiceView>> GetInvoicesInternalAsync(
		GridState<InvoiceView> state,
		CancellationToken cancellationToken)
	{
		QueryMapper mapper = new(StoreId, _searchString, _dateRange);

		InvoiceQueryParameters parameters = mapper.MapToQueryParameters(state);

		PagedResult<Invoice> invoices = await InvoiceService.GetAsync(parameters, cancellationToken);

		List<Vendor?> currentVendors = invoices.Items
			.Select(i => i.Vendor)
			.Where(v => v != null)
			.GroupBy(v => v.Id)
			.Select(g => g.First())
			.ToList();

		foreach (Vendor? vendor in currentVendors)
		{
			if (_vendors.All(v => v.Id != vendor.Id))
			{
				((List<Vendor>) _vendors).Add(vendor);
				_selectedVendors.Add(vendor);
			}
		}

		List<InvoiceView> result = invoices.Items.Select(i => new InvoiceView
		{
			Id = i.Id,
			Number = i.Number,
			Vendor = i.Vendor,
			StatusHistory = i.StatusHistory,
			Money = i.Transactions.FirstOrDefault()?.Money ?? default
		}).ToList();

		return new GridData<InvoiceView>
		{
			Items = result,
			TotalItems = invoices.Count
		};
	}

	private async Task OnAddButtonClick()
	{
		DialogParameters<AddInvoiceDialog> parameters = new()
		{
			{
				d => d.StoreId, StoreId
			}
		};

		DialogOptions options = new()
		{
			FullWidth = true,
			MaxWidth = MaxWidth.Medium,
			CloseButton = true,
			CloseOnEscapeKey = true
		};

		IDialogReference dialog =
			await DialogService.ShowAsync<AddInvoiceDialog>("Додати нову накладну", parameters, options);

		DialogResult? result = await dialog.Result;

		if (result?.Canceled is false)
		{
			await _dataGrid.ReloadServerData();
		}
	}

	private async Task OnEditButtonClick(InvoiceView invoice)
	{
		DialogParameters<EditInvoiceDialog> parameters = new()
		{
			{
				d => d.StoreId, StoreId
			},
			{
				d => d.Invoice, invoice
			}
		};

		DialogOptions options = new()
		{
			FullWidth = true,
			MaxWidth = MaxWidth.Medium,
			CloseButton = true,
			CloseOnEscapeKey = true
		};

		IDialogReference dialog =
			await DialogService.ShowAsync<EditInvoiceDialog>("Редагувати накладну", parameters, options);

		DialogResult? result = await dialog.Result;

		if (result?.Canceled is false)
		{
			await _dataGrid.ReloadServerData();
		}
	}

	private async Task OnAddTransactionClick(InvoiceView invoice)
	{
		DialogParameters<AddTransactionDialog> parameters = new()
		{
			{
				d => d.StoreId, StoreId
			},
			{
				d => d.InvoiceId, invoice.Id
			}
		};

		DialogOptions options = new()
		{
			FullWidth = true,
			MaxWidth = MaxWidth.Medium,
			CloseButton = true,
			CloseOnEscapeKey = true
		};

		IDialogReference dialog =
			await DialogService.ShowAsync<AddTransactionDialog>("Редагувати накладну", parameters, options);

		DialogResult? result = await dialog.Result;

		if (result?.Canceled is false)
		{
			await _dataGrid.ReloadServerData();
		}
	}

	private async Task OnDeleteButtonClick(InvoiceView invoice)
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		await InvoiceService.DeleteAsync(invoice.Id, tokenSource.Token);

		await _dataGrid.ReloadServerData();
	}

	private async Task OnCloseButtonClick(InvoiceView invoice)
	{
		DialogParameters<CloseInvoiceDialog> parameters = new()
		{
			{
				d => d.StoreId, StoreId
			},
			{
				d => d.Invoice, invoice
			}
		};

		DialogOptions options = new()
		{
			FullWidth = true,
			MaxWidth = MaxWidth.Medium,
			CloseButton = true,
			CloseOnEscapeKey = true
		};

		IDialogReference dialog =
			await DialogService.ShowAsync<CloseInvoiceDialog>("Закрити накладну", parameters, options);

		DialogResult? result = await dialog.Result;

		if (result?.Canceled is false)
		{
			await _dataGrid.ReloadServerData();
		}
	}

	private async Task OnDateRangeChanged()
	{
		await _dataGrid.ReloadServerData();
	}
}