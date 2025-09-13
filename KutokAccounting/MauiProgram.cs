using KutokAccounting.DataProvider;
using KutokAccounting.Services.Vendors;
using KutokAccounting.Services.Vendors.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace KutokAccounting;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMudServices();
        builder.Services.AddMauiBlazorWebView();

        builder.Services.AddDbContext<KutokDbContext>(options =>
        {
            options.UseSqlite(KutokConfigurations.ConnectionString);
        });

        builder.Services.AddScoped<IVendorService, VendorService>();
        builder.Services.AddScoped<IVendorRepository, VendorRepository>();
        builder.Services.AddScoped<VendorDtoValidator>();
        builder.Services.AddScoped<QueryParametersValidator>();


#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}