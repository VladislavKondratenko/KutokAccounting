using KutokAccounting.Components.Pages.TransactionTypes.Models;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.TransactionTypes.Models;
using MudBlazor;

namespace KutokAccounting.Components.Pages.TransactionTypes;

public partial class TransactionTypesPage
{
	private readonly HashSet<string> _filterOperators = new()
	{
		"equals"
	};

	private MudDataGrid<TransactionTypeView> _dataGrid = new();

	private string? _searchString;

	private TransactionTypeQueryParameters BuildQuery(GridState<TransactionTypeView> state)
	{
		Filters? filters = new();

		ICollection<IFilterDefinition<TransactionTypeView>> filterDefinitions = state.FilterDefinitions;

		if (filterDefinitions is not null)
		{
			foreach (IFilterDefinition<TransactionTypeView> filter in filterDefinitions)
			{
				if (filter.Title == TransactionTypeFiltersConstants.Name)
				{
					filters.Name = filter?.Value?.ToString();
				}
				else if (filter.Title == TransactionTypeFiltersConstants.Type)
				{
					filters.IsIncome = filter.Value is not TransactionTypeFiltersConstants.Expense;
				}
			}
		}

		return new TransactionTypeQueryParameters(filters, _searchString, new Pagination
		{
			Page = state.Page + 1,
			PageSize = state.PageSize
		});
	}

	private async Task<GridData<TransactionTypeView>> GetTransactionTypesAsync(GridState<TransactionTypeView> state)
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		try
		{
			return await GetTransactionTypeInternalAsync(state, tokenSource.Token);
		}
		catch (Exception)
		{
			return new GridData<TransactionTypeView>();
		}
		finally
		{
			StateHasChanged();
		}
	}

	private async ValueTask<GridData<TransactionTypeView>> GetTransactionTypeInternalAsync(
		GridState<TransactionTypeView> state,
		CancellationToken cancellationToken)
	{
		TransactionTypeQueryParameters transactionTypeQueryParameters = BuildQuery(state);

		PagedResult<TransactionType> pagedResult =
			await TransactionTypeService.GetAsync(transactionTypeQueryParameters, cancellationToken);

		List<TransactionTypeView> result = pagedResult.Items.Select(tp => new TransactionTypeView
		{
			Id = tp.Id,
			Name = tp.Name,
			IsIncome = tp.IsIncome
		}).ToList();

		return new GridData<TransactionTypeView>
		{
			Items = result,
			TotalItems = pagedResult.Count
		};
	}

	private async Task OnDeleteButtonClick(TransactionTypeView transactionType)
	{
		using CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(30));

		int rowsDeleted = await TransactionTypeService.DeleteAsync(transactionType.Id, tokenSource.Token);

		if (rowsDeleted == 0)
		{
			DialogOptions options = new()
			{
				FullWidth = true,
				MaxWidth = MaxWidth.Medium,
				CloseOnEscapeKey = true
			};

			bool? result = await DialogService.ShowMessageBox("⚠️Увага",
				"Неможливо видалити тип транзакції, оскільки існують пов'язані з ним транзакції!", "Ок",
				options: options);
		}

		await _dataGrid.ReloadServerData();
	}

	private async Task OnEditButtonClick(TransactionTypeView transactionType)
	{
		DialogOptions options = new()
		{
			FullWidth = true,
			MaxWidth = MaxWidth.Medium,
			CloseButton = true,
			CloseOnEscapeKey = true
		};

		DialogParameters<EditTransactionTypeDialog> parameters = new()
		{
			{
				d => d.TransactionTypeView, transactionType
			}
		};

		IDialogReference dialog =
			await DialogService.ShowAsync<EditTransactionTypeDialog>("Редагувати тип транзакції", parameters, options);

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
			await DialogService.ShowAsync<AddTransactionTypeDialog>("Додати новий тип транзакції", options);

		DialogResult? result = await dialog.Result;

		if (result?.Canceled is false)
		{
			await _dataGrid.ReloadServerData();
		}
	}
}