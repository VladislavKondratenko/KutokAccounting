using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Vendors.Models;
using MudBlazor;

namespace KutokAccounting.Components.Pages.Vendors;

public partial class VendorsPage
{
	private readonly HashSet<string> _filterOperators = new()
	{
		"equals"
	};

	private MudDataGrid<Vendor> _dataGrid = new();

	private string? _searchString;

	private async Task<GridData<Vendor>> GetVendorsAsync(GridState<Vendor> state)
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		string? filter = state.FilterDefinitions.FirstOrDefault()?.Value?.ToString();

		VendorQueryParameters queryParameters = new(filter, _searchString, new Pagination
		{
			Page = state.Page + 1,
			PageSize = state.PageSize
		});

		try
		{
			PagedResult<Vendor> pagedResult = await VendorService.GetAsync(queryParameters, tokenSource.Token);

			return new GridData<Vendor>
			{
				Items = pagedResult.Items,
				TotalItems = pagedResult.Count
			};
		}
		catch (Exception)
		{
			return new GridData<Vendor>();
		}
		finally
		{
			StateHasChanged();
		}
	}

	private async Task OnDeleteButtonClick(Vendor vendor)
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		await VendorService.DeleteAsync(vendor.Id, tokenSource.Token);

		await _dataGrid.ReloadServerData();
	}

	private async Task OnEditButtonClick(Vendor vendor)
	{
		DialogOptions options = new()
		{
			FullWidth = true,
			MaxWidth = MaxWidth.Medium,
			CloseButton = true,
			CloseOnEscapeKey = true
		};

		DialogParameters<EditVendorDialog> parameters = new()
		{
			{
				d => d.Vendor, vendor
			}
		};

		IDialogReference dialog =
			await DialogService.ShowAsync<EditVendorDialog>("Редагувати постачальника", parameters, options);

		DialogResult? result = await dialog.Result;

		if (result?.Canceled is false)
		{
			await _dataGrid.ReloadServerData();
		}
	}

	private async Task OnAddButtonClick()
	{
		DialogOptions options = new()
		{
			FullWidth = true,
			MaxWidth = MaxWidth.Medium,
			CloseButton = true,
			CloseOnEscapeKey = true
		};

		IDialogReference dialog =
			await DialogService.ShowAsync<AddVendorDialog>("Додати нового постачальника", options);

		DialogResult? result = await dialog.Result;

		if (result?.Canceled is false)
		{
			await _dataGrid.ReloadServerData();
		}
	}
}