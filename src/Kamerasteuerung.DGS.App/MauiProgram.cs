using Kamerasteuerung.DGS.App.Views;
using Kamerasteuerung.DGS.App.Services;
using Kamerasteuerung.DGS.Core.Services;

namespace Kamerasteuerung.DGS.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>();

        builder.Services.AddSingleton<AppSessionState>();
        builder.Services.AddSingleton<SettingsFileService>();
        builder.Services.AddSingleton<LayoutFileService>();
        builder.Services.AddSingleton<AppStartupService>();
        builder.Services.AddSingleton<LayoutSessionService>();

        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<LayoutDesignerPage>();
        builder.Services.AddSingleton<LiveControlPage>();
        builder.Services.AddSingleton<ParticipantsPage>();
        builder.Services.AddSingleton<SettingsPage>();

        return builder.Build();
    }
}
