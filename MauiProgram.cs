using Microsoft.Extensions.Logging;
using MauiApp3.Data;
using MauiApp3.Services;

namespace MauiApp3;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        
        // Database
        builder.Services.AddDbContext<AppDbContext>();
        
        // Core Services
        builder.Services.AddScoped<IJournalService, JournalService>();
        builder.Services.AddScoped<ITagService, TagService>();
        builder.Services.AddScoped<IStreakService, StreakService>();
        builder.Services.AddScoped<IThemeService, ThemeService>();
        builder.Services.AddScoped<IExportService, ExportService>();
        
        // Auth Services
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<AppState>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
