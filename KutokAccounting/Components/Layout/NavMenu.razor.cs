using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Stores.Dtos;
using KutokAccounting.Services.Stores.Models;
using Microsoft.AspNetCore.Components;

namespace KutokAccounting.Components.Layout;

public partial class NavMenu : ComponentBase
{
	private IReadOnlyCollection<StoreDto> _stores;

	public void Dispose()
	{
		StoreStateNotifier.OnStoreStateChangedAsync -= UpdateStoresAsync;
	}

	protected override async Task OnInitializedAsync()
	{
		StoreStateNotifier.OnStoreStateChangedAsync += UpdateStoresAsync;
		await UpdateStoresAsync();
	}

	private async Task UpdateStoresAsync()
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		PagedResult<StoreDto> stores = await StoresService.GetPageAsync(new StoreQueryParameters
		{
			Pagination = new Pagination
			{
				PageSize = int.MaxValue
			}
		}, tokenSource.Token);

		_stores = stores.Items.ToList();
	}

	private string GetHrefToStoreAccounting(int id)
	{
		return $"/StoreAccounting/{id}";
	}

	private string GetHrefToStoreInvoices(int id)
	{
		return $"/invoices/{id}";
	}
}