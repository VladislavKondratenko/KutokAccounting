using KutokAccounting.Services.Stores.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KutokAccounting.Services.Stores.Extensions;

public static class StoreServicesExtensions
{
	public static void AddStoreServices(this IServiceCollection services)
	{
		services.AddScoped<IStoresRepository, StoresRepository>();
		services.AddScoped<IStoresService, StoresService>();
	}
}