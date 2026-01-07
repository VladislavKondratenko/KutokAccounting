using KutokAccounting.Components.Pages.Invoices.Models;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Helpers;
using KutokAccounting.Services.Invoices.Models;
using KutokAccounting.Services.Stores.Models;
using MudBlazor;

namespace KutokAccounting.Components.Pages.Invoices.Helpers;

public class QueryMapper
{
	private readonly int _storeId;
	private readonly string? _searchString;
	private readonly DateRange? _dateRange;

	private readonly Dictionary<string, IFilterStrategy> _strategies = new()
	{
		[InvoiceFiltersConstants.Number] = new StringFilterStrategy((f, v) => f.Number = v),
		[InvoiceFiltersConstants.State] = new StateFilterStrategy((f, v) => f.IsClosed = v)
	};

	public QueryMapper(int storeId,
		string? searchString,
		DateRange? dateRange)
	{
		_storeId = storeId;
		_searchString = searchString;
		_dateRange = dateRange;
	}

	public InvoiceQueryParameters MapToQueryParameters(GridState<InvoiceView> state)
	{
		Filters filters = new()
		{
			StoreId = _storeId
		};

		Sorting sorting = MapSorting(state.SortDefinitions.FirstOrDefault());

		IFilterDefinition<InvoiceView>? filterDefinition = state.FilterDefinitions.FirstOrDefault();

		string? key = filterDefinition?.Title;

		if (string.IsNullOrWhiteSpace(key) is false && _strategies.TryGetValue(key, out IFilterStrategy? strategy))
		{
			strategy.Apply(filters, filterDefinition);
		}

		MapDateRange(filters, _dateRange);

		return new InvoiceQueryParameters
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

	private Sorting MapSorting(SortDefinition<InvoiceView>? sortDefinition)
	{
		Sorting sorting = new();

		if (sortDefinition is null)
		{
			return sorting;
		}

		sorting.SortBy = sortDefinition.SortBy;

		sorting.Descending = sortDefinition.Descending;

		return sorting;
	}
}