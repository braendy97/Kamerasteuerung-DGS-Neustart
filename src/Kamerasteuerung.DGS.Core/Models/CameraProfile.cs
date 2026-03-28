namespace Kamerasteuerung.DGS.Core.Models;

public sealed class CameraProfile
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public CameraType CameraType { get; set; }
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 5678;
    public int PresetStart { get; set; } = 10;
    public int PresetEnd { get; set; } = 210;
    public bool SupportsTracking { get; set; }
}
