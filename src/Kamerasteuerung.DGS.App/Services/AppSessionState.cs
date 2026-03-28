using Kamerasteuerung.DGS.Core.Models;

namespace Kamerasteuerung.DGS.App.Services;

public sealed class AppSessionState
{
    public AppSettings Settings { get; set; } = new();
    public LayoutDocument? Layout { get; set; }
    public string StatusMessage { get; set; } = "App wird initialisiert ...";
}
