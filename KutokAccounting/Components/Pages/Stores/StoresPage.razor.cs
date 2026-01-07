using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Stores.Dtos;
using KutokAccounting.Services.Stores.Models;
using MudBlazor;

namespace KutokAccounting.Components.Pages.Stores;

partial class StoresPage
{
	private readonly DialogOptions _dialogOptions = new()
	{
		FullWidth = true,
		MaxWidth = MaxWidth.Medium,
		CloseButton = true,
		CloseOnEscapeKey = true
	};

	private MudDataGrid<StoreDto> _dataGrid;
	private StoreQueryParameters _queryStoreParameters = new();

	public async Task<GridData<StoreDto>> GetStoresAsync(GridState<StoreDto> state)
	{
		GridData<StoreDto> gridData = new()
		{
			Items = new List<StoreDto>(),
			TotalItems = 0
		};

		try
		{
			using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

			_queryStoreParameters.Pagination = new Pagination
			{
				Page = state.Page + 1,
				PageSize = state.PageSize
			};

			PagedResult<StoreDto> storesPage =
				await StoresService.GetPageAsync(_queryStoreParameters, tokenSource.Token);

			gridData.Items = storesPage.Items;
			gridData.TotalItems = storesPage.Count;
		}
		catch (Exception e)
		{
			await DialogService.ShowMessageBox("Виникла помилка", $"Текст помилки: {e}");
		}
		finally
		{
			StateHasChanged();
		}

		return gridData;
	}

	public async Task OnAddShopButtonClick()
	{
		IDialogReference dialog = await DialogService.ShowAsync<AddStoreDialog>("Додати магазин", _dialogOptions);
		DialogResult? dialogResult = await dialog.Result;

		if (_dataGrid != null && !dialogResult.Canceled)
		{
			await _dataGrid.ReloadServerData();
			StateHasChanged();
			StoreStateNotifier.StoreStateChanged();
		}
	}

	public async Task OnEditButtonClick(StoreDto? storeDto)
	{
		try
		{
			using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

			DialogParameters<EditStoreDialog> parameters = new()
			{
				{
					d => d.Store, storeDto
				}
			};

			IDialogReference dialog =
				await DialogService.ShowAsync<EditStoreDialog>("Редагувати данні магазину", parameters, _dialogOptions);

			DialogResult? dialogResult = await dialog.Result;

			if (_dataGrid != null && !dialogResult.Canceled)
			{
				await _dataGrid.ReloadServerData();
				StateHasChanged();
				StoreStateNotifier.StoreStateChanged();
			}
		}
		catch (Exception e)
		{
			await DialogService.ShowMessageBox("Exception", $"{e.Message}");
		}
	}

	private async Task OnDeleteButtonClick(StoreDto? storeDto)
	{
		bool? shouldBeDeleted = await DialogService.ShowMessageBox("Увага!",
			$"Ви точно хочете видалити магазин {storeDto.Name}?", "Так", "Ні");

		if (shouldBeDeleted.GetValueOrDefault())
		{
			await TryDeleteStoreAsync(storeDto);
			StateHasChanged();
			StoreStateNotifier.StoreStateChanged();
		}
	}

	private async Task TryDeleteStoreAsync(StoreDto storeDto)
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		try
		{
			await StoresService.DeleteAsync(storeDto.Id, tokenSource.Token);

			if (_dataGrid != null)
			{
				await _dataGrid.ReloadServerData();
			}
		}
		catch (Exception e)
		{
			await DialogService.ShowMessageBox("Виникла помилка", $"Текст помилки: {e}");
		}
	}

	private async Task OnMoreButtonClick(StoreDto storeDto)
	{
		DialogParameters<EditStoreDialog> parameters = new()
		{
			{
				d => d.Store, storeDto
			}
		};

		await DialogService.ShowAsync<MoreStoreInfoDialog>("Більше інформації про магазин", parameters, _dialogOptions);
	}

	private async Task OnSearchButtonClick()
	{
		DialogParameters<SearchStoreDialog> dialogParameters = new()
		{
			{
				d => d.StoreQueryParameters, _queryStoreParameters
			}
		};

		IDialogReference dialog =
			await DialogService.ShowAsync<SearchStoreDialog>("Пошук", dialogParameters, _dialogOptions);

		DialogResult? dialogResult = await dialog.Result;

		if (dialogResult!.Data != null && dialogResult.Data is StoreQueryParameters storeProperties)
		{
			_queryStoreParameters = storeProperties;
		}

		if (_dataGrid != null && !dialogResult.Canceled)
		{
			await _dataGrid.ReloadServerData();
		}
	}
}