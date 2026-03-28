using Kamerasteuerung.DGS.App.Views;

namespace Kamerasteuerung.DGS.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>();

        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<LayoutDesignerPage>();
        builder.Services.AddSingleton<LiveControlPage>();
        builder.Services.AddSingleton<ParticipantsPage>();
        builder.Services.AddSingleton<SettingsPage>();

        return builder.Build();
    }
}
