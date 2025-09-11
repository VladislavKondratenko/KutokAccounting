using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Stores.Dtos;

namespace KutokAccounting.Services.Stores.Abstractions;

public interface IStoresService
{
	Task CreateStoreAsync(StoreDto storeDto);
	IQueryable<Store> GetStoresPageAsync(int pageSize, int pageNumber);
	int GetAllStoresCount();
	Task UpdateStore(int storeId, StoreDto updatedStoreDto);
	Task DeleteStore(int storeId);
}