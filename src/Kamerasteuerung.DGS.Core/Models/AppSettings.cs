namespace Kamerasteuerung.DGS.Core.Models;

public sealed class AppSettings
{
    public string? LastOpenedLayoutPath { get; set; }
    public string? DefaultLayoutDirectory { get; set; }
    public bool AutoLoadLastLayoutOnStartup { get; set; } = true;
    public int PreferredPresetRangeStart { get; set; } = 10;
    public int PreferredPresetRangeEnd { get; set; } = 210;
}
