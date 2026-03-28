namespace Kamerasteuerung.DGS.Core.Models;

public sealed class LayoutDocument
{
    public int DocumentVersion { get; set; } = 1;
    public string Name { get; set; } = "Neues Layout";
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
    public LayoutCanvas Canvas { get; set; } = new();
    public List<CameraProfile> Cameras { get; set; } = [];
    public List<LayoutBlock> Blocks { get; set; } = [];
    public List<LayoutSeat> Seats { get; set; } = [];
    public List<ParticipantEntry> Participants { get; set; } = [];
}
