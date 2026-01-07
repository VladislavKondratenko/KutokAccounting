using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Transactions.Models;
using Microsoft.EntityFrameworkCore;

namespace KutokAccounting.Services.Transactions;

public class TransactionQueryBuilder
{
	private IQueryable<Transaction> _query;

	public TransactionQueryBuilder(IQueryable<Transaction> query)
	{
		_query = query;
	}

	public TransactionQueryBuilder AddSearchString(string? searchString)
	{
		if (string.IsNullOrWhiteSpace(searchString) is false)
		{
			_query = _query.Where(t => EF.Functions.Like(t.Name + " " + t.Description, $"%{searchString}%"));
		}

		return this;
	}

	public TransactionQueryBuilder AddSorting(Sorting? sorting)
	{
		if (sorting is null || string.IsNullOrWhiteSpace(sorting.SortBy))
		{
			return this;
		}

		_query = sorting.SortBy switch
		{
			nameof(Transaction.Name) => sorting.Descending
				? _query.OrderByDescending(t => t.Name)
				: _query.OrderBy(t => t.Name),

			nameof(Transaction.Description) => sorting.Descending
				? _query.OrderByDescending(t => t.Description)
				: _query.OrderBy(t => t.Description),

			nameof(Money) + "." + nameof(Money.Value) => sorting.Descending
				? _query.OrderByDescending(t => t.Money)
				: _query.OrderBy(t => t.Money),

			nameof(Transaction.CreatedAt) => sorting.Descending
				? _query.OrderByDescending(t => t.CreatedAt)
				: _query.OrderBy(t => t.CreatedAt),

			_ => _query.OrderBy(t => t.CreatedAt)
		};

		return this;
	}

	public TransactionQueryBuilder AddFilters(Filters? filters)
	{
		if (filters is not null)
		{
			if (filters.Name is not null)
			{
				_query = _query.Where(t => t.Name == filters.Name);
			}

			if (filters.Description is not null)
			{
				_query = _query.Where(t => t.Description == filters.Description);
			}

			if (filters.TransactionTypeId is not null)
			{
				_query = _query.Where(t => t.TransactionTypeId == filters.TransactionTypeId);
			}

			if (filters.LessThan is not null)
			{
				_query = _query.Where(t => t.Money < filters.LessThan);
			}

			if (filters.MoreThan is not null)
			{
				_query = _query.Where(t => t.Money > filters.MoreThan);
			}

			if (filters.Value is not null)
			{
				_query = _query.Where(t => t.Money == filters.Value);
			}

			if (filters.Range is not null)
			{
				_query = _query.Where(t => t.CreatedAt >= filters.Range.Value.StartOfRange &&
					t.CreatedAt <= filters.Range.Value.EndOfRange);
			}

			_query = _query.Where(t => t.StoreId == filters.StoreId);
		}

		return this;
	}

	public IQueryable<Transaction> Build()
	{
		return _query;
	}
}