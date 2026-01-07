using FluentValidation;
using KutokAccounting.Components.Pages.Stores;
using KutokAccounting.Components.Pages.Transactions;
using KutokAccounting.DataProvider;
using KutokAccounting.DataProvider.Models;
using KutokAccounting.Logging.Extensions;
using KutokAccounting.Services.Invoices;
using KutokAccounting.Services.Invoices.Interfaces;
using KutokAccounting.Services.Invoices.Models;
using KutokAccounting.Services.Invoices.Validators;
using KutokAccounting.Services.Stores;
using KutokAccounting.Services.Stores.Abstractions;
using KutokAccounting.Services.Stores.Dtos;
using KutokAccounting.Services.Stores.Models;
using KutokAccounting.Services.Transactions;
using KutokAccounting.Services.Transactions.Interfaces;
using KutokAccounting.Services.Transactions.Models;
using KutokAccounting.Services.Transactions.Validators;
using KutokAccounting.Services.TransactionTypes;
using KutokAccounting.Services.TransactionTypes.Interfaces;
using KutokAccounting.Services.TransactionTypes.Models;
using KutokAccounting.Services.TransactionTypes.Validators;
using KutokAccounting.Services.Vendors;
using KutokAccounting.Services.Vendors.Models;
using KutokAccounting.Services.Vendors.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using MudBlazor;
using MudBlazor.Services;
using WinRT.Interop;

namespace KutokAccounting;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		MauiAppBuilder builder = MauiApp.CreateBuilder();

		builder.Services.AddMudServices();
		
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

#if WINDOWS
		builder.ConfigureLifecycleEvents(events =>
			events.AddWindows(wndLifeCycleBuilder =>
				wndLifeCycleBuilder.OnWindowCreated(window =>
				{
					IntPtr hWnd = WindowNative.GetWindowHandle(window);
					WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
					AppWindow? appWindow = AppWindow.GetFromWindowId(myWndId);

					if (appWindow.Presenter is OverlappedPresenter overlapped)
					{
						overlapped.Maximize();
					}
				})));
#endif
				
		builder.Services.AddTransient<MudLocalizer, UkrainianLocalizer>();
		builder.Services.AddMauiBlazorWebView();

		builder.Services.AddDbContext<KutokDbContext>(options =>
		{
			options.UseSqlite(KutokConfigurations.ConnectionString);
		});
		
		builder.Services.AddScoped<IInvoiceService, InvoiceService>();
		builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
		builder.Services.AddScoped<IValidator<InvoiceDto>, InvoiceDtoValidator>();

		builder.Services
			.AddScoped<IValidator<InvoiceQueryParameters>, InvoiceQueryParametersValidator>();

		builder.Services.AddFileLogging();

		builder.Services.AddScoped<IVendorService, VendorService>();
		builder.Services.AddScoped<IVendorRepository, VendorRepository>();
		builder.Services.AddScoped<IValidator<TransactionTypeDto>, TransactionTypeDtoValidator>();

		builder.Services
			.AddScoped<IValidator<TransactionTypeQueryParameters>, TransactionTypeQueryParametersValidator>();

		builder.Services.AddScoped<ITransactionTypeService, TransactionTypeService>();
		builder.Services.AddScoped<ITransactionTypeRepository, TransactionTypeRepository>();
		builder.Services.AddScoped<IValidator<VendorDto>, VendorDtoValidator>();
		builder.Services.AddScoped<IValidator<VendorQueryParameters>, VendorQueryParametersValidator>();
		builder.Services.AddKeyedSingleton(KutokConfigurations.WriteOperationsSemaphore, new SemaphoreSlim(1, 1));
		builder.Services.AddSingleton<StoreStateNotifier>();
		builder.Services.AddScoped<TransactionsStateNotifier>();
		builder.Services.AddScoped<IValidator<Pagination>, PaginationValidator>();
		builder.Services.AddScoped<IValidator<StoreDto>, StoreDtoValidator>();
		builder.Services.AddScoped<IStoresRepository, StoresRepository>();
		builder.Services.AddScoped<IStoresService, StoresService>();
		builder.Services.AddScoped<ITransactionService, TransactionService>();
		builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
		builder.Services.AddScoped<IValidator<TransactionDto>, TransactionDtoValidator>();
		builder.Services.AddScoped<IValidator<TransactionQueryParameters>, TransactionQueryParametersValidator>();
		
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif
		
		
		
		return builder.Build();
	}
}