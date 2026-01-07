using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Invoices.Models;
using Microsoft.EntityFrameworkCore;

namespace KutokAccounting.Services;

public class InvoiceQueryBuilder
{
	private IQueryable<Invoice> _query;

	public InvoiceQueryBuilder(IQueryable<Invoice> query)
	{
		_query = query;
	}

	public InvoiceQueryBuilder AddSearchString(string? searchString)
	{
		if (string.IsNullOrWhiteSpace(searchString) is false)
		{
			_query = _query.Where(i => EF.Functions.Like(i.Number, $"%{searchString}%"));
		}

		return this;
	}

	public InvoiceQueryBuilder AddSorting(Sorting? sorting)
	{
		if (sorting is null || string.IsNullOrWhiteSpace(sorting.SortBy))
		{
			return this;
		}

		_query = sorting.SortBy switch
		{
			nameof(Invoice.Number) => sorting.Descending
				? _query.OrderByDescending(i => i.Number)
				: _query.OrderBy(i => i.Number),

			nameof(State) => sorting.Descending
				? _query.OrderByDescending(i => i.StatusHistory.First().State)
				: _query.OrderBy(i => i.StatusHistory.First().State),

			nameof(Invoice.Vendor.Name) => sorting.Descending
				? _query.OrderByDescending(i => i.Vendor.Name)
				: _query.OrderBy(i => i.Vendor.Name),

			nameof(Invoice.CreatedAt) => sorting.Descending
				? _query.OrderByDescending(i => i.CreatedAt)
				: _query.OrderBy(i => i.CreatedAt),

			_ => _query.OrderBy(i => i.CreatedAt)
		};

		return this;
	}

	public InvoiceQueryBuilder AddFilters(Filters? filters)
	{
		if (filters is not null)
		{
			if (filters.Number is not null)
			{
				_query = _query.Where(i => i.Number == filters.Number);
			}

			if (filters.VendorId is not null)
			{
				_query = _query.Where(i => i.VendorId == filters.VendorId);
			}

			if (filters.IsClosed is not null)
			{
				_query = filters.IsClosed switch
				{
					true => _query.Where(i => i.StatusHistory.First().State == State.Closed),
					_ => _query.Where(i => i.StatusHistory.First().State == State.Active)
				};
			}

			if (filters.Range is not null)
			{
				_query = _query.Where(i =>
					i.CreatedAt >= filters.Range.Value.StartOfRange && i.CreatedAt <= filters.Range.Value.EndOfRange);
			}

			_query = _query.Where(i => i.StoreId == filters.StoreId);
		}

		return this;
	}

	public IQueryable<Invoice> Build()
	{
		return _query;
	}
}