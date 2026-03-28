namespace Kamerasteuerung.DGS.App.Services;

public static class AppPaths
{
    public static string SettingsFilePath => Path.Combine(FileSystem.AppDataDirectory, "appsettings.json");
}
