using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Stores.Dtos;
using KutokAccounting.Services.Stores.Models;
using Microsoft.AspNetCore.Components;

namespace KutokAccounting.Components.Pages;

public partial class HomePage : ComponentBase
{
	public IEnumerable<StoreDto> Stores { get; set; }

	[Inject]
	protected NavigationManager NavigationManager { get; set; } = default!;

	protected override async Task OnInitializedAsync()
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		PagedResult<StoreDto> stores = await StoresService.GetPageAsync(new StoreQueryParameters
		{
			Pagination = new Pagination
			{
				PageSize = int.MaxValue
			}
		}, tokenSource.Token);

		Stores = stores.Items;
	}

	private void OnTransactionsButtonClick(StoreDto store)
	{
		NavigationManager.NavigateTo($"/StoreAccounting/{store.Id}");
	}
}