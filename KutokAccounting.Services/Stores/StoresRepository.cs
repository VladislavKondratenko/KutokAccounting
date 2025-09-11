using KutokAccounting.DataProvider;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Stores.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace KutokAccounting.Services.Stores;

public class StoresRepository : IStoresRepository
{
	private readonly KutokDbContext _dbContext;
	private readonly DbSet<Store> _storesDbSet;
	public StoresRepository(KutokDbContext dbContext)
	{
		_dbContext = dbContext;
		_storesDbSet = dbContext.Stores;
	}
	public async Task CreateStoreAsync(Store store)
	{
		if (StoreExists(store))
		{
			throw new ArgumentException($"Store {store.Name} already exists");
		}
		if (HasRequiredData(store) is false)
		{
			throw new NullReferenceException("Missing some required data for creating store");
		}
		
		_storesDbSet.Add(store);
		await _dbContext.SaveChangesAsync();
	}
	
	public int GetStoresCount()
	{
		return _storesDbSet.Count();
	}
	public IQueryable<Store> GetStoresPage(int pageSize, int pageNumber)
	{
		_storesDbSet.AsNoTracking();
		var startPosition = pageSize * (pageNumber - 1);
		return _storesDbSet.Skip(startPosition).Take(pageSize);
	}

	/// <summary>
	/// updates store without the need to save changes manually
	/// </summary>
	/// <param name="storeId">store id to update</param>
	/// <param name="updatedStore">new characteristics of store</param>
	public async Task UpdateStoreAsync(int storeId, Store updatedStore)
	{
		var store = await _storesDbSet.FindAsync(storeId);

		ThrowIfStoreIsNull(store, storeId);

		//tech fields like history remain the same
		store.Name = updatedStore.Name;
		store.IsOpened = updatedStore.IsOpened;
		store.SetupDate = updatedStore.SetupDate;
		store.Address = updatedStore.Address;
		
		_storesDbSet.Update(store);
		await _dbContext.SaveChangesAsync();
	}

	public async Task DeleteStoreAsync(int storeId)
	{
		var store = _storesDbSet.Find(storeId);
		ThrowIfStoreIsNull(store, storeId);
		
		_storesDbSet.Remove(store);
		await _dbContext.SaveChangesAsync();
	}
	private bool StoreExists(Store store)
	{
		return _storesDbSet.Any(s => s.Id == store.Id);
	}
	private bool HasRequiredData(Store store)
	{
		return store.IsOpened != null && store.SetupDate != null && store.Name != null;
	}

	private void ThrowIfStoreIsNull(Store? store, int storeId)
	{
		if (store is null)
		{
			throw new ArgumentException($"Store with id {storeId} does not exist");
		}
	}
}