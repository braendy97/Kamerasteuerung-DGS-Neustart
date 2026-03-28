namespace Kamerasteuerung.DGS.Core.Models;

public sealed class LayoutSeat
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Label { get; set; } = string.Empty;
    public string? BlockId { get; set; }
    public string CameraId { get; set; } = string.Empty;
    public int PresetNumber { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public string? AssignedParticipantId { get; set; }
}
