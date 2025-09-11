using KutokAccounting.DataProvider.Models;
using KutokAccounting.Services.Stores.Dtos;

namespace KutokAccounting.Services.Stores.Extensions;

public static class StoreDtoExtensions
{
	public static Store FromDto(this StoreDto dto)
	{
		return new Store
		{
			Name = dto.Name,
			IsOpened = dto.IsOpened,
			SetupDate = dto.SetupDate,
			Address = dto.Address,
		};
	}
}