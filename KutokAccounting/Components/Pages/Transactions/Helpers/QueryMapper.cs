using KutokAccounting.Components.Pages.Transactions.Models;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Helpers;
using KutokAccounting.Services.Stores.Models;
using KutokAccounting.Services.Transactions.Models;
using MudBlazor;

namespace KutokAccounting.Components.Pages.Transactions.Helpers;

public class QueryMapper
{
	private readonly int _storeId;
	private readonly string? _searchString;
	private readonly DateRange? _dateRange;

	private readonly Dictionary<string, IFilterStrategy> _strategies = new()
	{
		[TransactionFiltersConstants.Name] = new StringFilterStrategy((f, v) => f.Name = v),
		[TransactionFiltersConstants.Description] = new StringFilterStrategy((f, v) => f.Description = v),
		[TransactionFiltersConstants.Value] = new MoneyFilterStrategy(),
		[TransactionFiltersConstants.TransactionType] = new TransactionTypeStrategy()
	};

	public QueryMapper(int storeId,
		string? searchString,
		DateRange? dateRange)
	{
		_storeId = storeId;
		_searchString = searchString;
		_dateRange = dateRange;
	}

	public TransactionQueryParameters MapToQueryParameters(GridState<TransactionView> state)
	{
		Filters filters = new()
		{
			StoreId = _storeId
		};

		Sorting sorting = MapSorting(state.SortDefinitions.FirstOrDefault());

		IFilterDefinition<TransactionView>? filterDefinition = state.FilterDefinitions.FirstOrDefault();

		string? key = filterDefinition?.Title;

		if (key is not null && _strategies.TryGetValue(key, out IFilterStrategy? strategy))
		{
			strategy.Apply(filters, filterDefinition);
		}

		MapDateRange(filters, _dateRange);

		return new TransactionQueryParameters
		{
			Filters = filters,
			Pagination = new Pagination
			{
				Page = state.Page + 1,
				PageSize = state.PageSize
			},
			SearchString = _searchString,
			Sorting = sorting
		};
	}

	private void MapDateRange(Filters filters, DateRange? range)
	{
		if (range is {Start: not null, End: not null})
		{
			filters.Range = range.Start == range.End
				? DateTimeHelper.GetDayStartEndRange(range.Start.Value)
				: new DateTimeRange(range.Start.Value, range.End.Value);
		}
	}

	private Sorting MapSorting(SortDefinition<TransactionView>? sortDefinition)
	{
		Sorting sorting = new();

		if (sortDefinition is not null)
		{
			sorting.SortBy = sortDefinition.SortBy;

			sorting.Descending = sortDefinition.Descending;
		}

		return sorting;
	}
}