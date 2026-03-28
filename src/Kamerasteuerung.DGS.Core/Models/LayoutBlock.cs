namespace Kamerasteuerung.DGS.Core.Models;

public sealed class LayoutBlock
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public CameraType? CameraType { get; set; }
    public int PresetPriority { get; set; } = 100;
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; } = 300;
    public double Height { get; set; } = 200;
    public double RotationDegrees { get; set; }
}
