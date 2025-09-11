using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Stores.Abstractions;
using KutokAccounting.Services.Stores.Dtos;
using KutokAccounting.Services.Stores.Extensions;

namespace KutokAccounting.Services.Stores;

public class StoresService : IStoresService
{
	private readonly IStoresRepository _repository;
	public StoresService(IStoresRepository repository)
	{
		_repository = repository;
	}

	public async Task CreateStoreAsync(StoreDto storeDto)
	{
		var storeModel = storeDto.FromDto();
		await _repository.CreateStoreAsync(storeModel);
	}

	public IQueryable<Store> GetStoresPageAsync(int pageSize, int pageNumber)
	{
		return _repository.GetStoresPage(pageSize, pageNumber);
	}

	public int GetAllStoresCount()
	{
		return _repository.GetStoresCount();
	}

	public async Task UpdateStore(int storeId, StoreDto updatedStoreDto)
	{
		var updatedStoreModel = updatedStoreDto.FromDto();
		await _repository.UpdateStoreAsync(storeId, updatedStoreModel);
	}

	public async Task DeleteStore(int storeId)
	{
		if (storeId >= 0 is false)
		{
			throw new ArgumentException("Invalid store id");
		}
		
		await _repository.DeleteStoreAsync(storeId);
	}
}