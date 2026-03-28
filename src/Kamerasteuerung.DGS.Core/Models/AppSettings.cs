namespace Kamerasteuerung.DGS.Core.Models;

public sealed class AppSettings
{
    public string? LastLayoutFilePath { get; set; }
    public string? LayoutsFolderPath { get; set; }
    public bool AutoLoadLastLayoutOnStartup { get; set; } = true;

    public string? V600Host { get; set; }
    public int V600Port { get; set; } = 5678;

    public string? SmtavV60XlHost { get; set; }
    public int SmtavV60XlPort { get; set; } = 5678;

    public int PreferredPresetRangeStart { get; set; } = 10;
    public int PreferredPresetRangeEnd { get; set; } = 210;

    [System.Text.Json.Serialization.JsonIgnore]
    public string? LastOpenedLayoutPath
    {
        get => LastLayoutFilePath;
        set => LastLayoutFilePath = value;
    }

    [System.Text.Json.Serialization.JsonIgnore]
    public string? DefaultLayoutDirectory
    {
        get => LayoutsFolderPath;
        set => LayoutsFolderPath = value;
    }
}
