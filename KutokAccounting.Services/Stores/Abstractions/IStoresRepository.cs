using KutokAccounting.DataProvider.Models;

namespace KutokAccounting.Services.Stores.Abstractions;

public interface IStoresRepository
{
	Task CreateStoreAsync(Store store);
	int GetStoresCount();
	IQueryable<Store> GetStoresPage(int pageSize, int pageNumber);
	Task UpdateStoreAsync(int storeId, Store updatedStore);
	Task DeleteStoreAsync(int storeId);
}