using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Stores.Abstractions;

namespace KutokAccounting.Services.Stores;

public class StoresService : IStoresService
{
	private readonly IStoresRepository _repository;
	public StoresService(IStoresRepository repository)
	{
		_repository = repository;
	}

	public async Task CreateStoreAsync()
	{
		
		
	}
}