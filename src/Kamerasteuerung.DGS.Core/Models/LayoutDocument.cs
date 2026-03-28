namespace Kamerasteuerung.DGS.Core.Models;

public sealed class LayoutDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int DocumentVersion { get; set; } = 1;
    public string Name { get; set; } = "Neues Layout";
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
    public LayoutCanvas Canvas { get; set; } = new();
    public List<CameraProfile> Cameras { get; set; } = [];
    public List<LayoutBlock> Blocks { get; set; } = [];
    public List<LayoutSeat> Seats { get; set; } = [];
    public List<ParticipantEntry> Participants { get; set; } = [];

    [System.Text.Json.Serialization.JsonIgnore]
    public int Version
    {
        get => DocumentVersion;
        set => DocumentVersion = value;
    }

    [System.Text.Json.Serialization.JsonIgnore]
    public DateTime LastModifiedUtc
    {
        get => LastUpdatedUtc;
        set => LastUpdatedUtc = value;
    }

    [System.Text.Json.Serialization.JsonIgnore]
    public double CanvasWidth
    {
        get => Canvas.Width;
        set => Canvas.Width = value;
    }

    [System.Text.Json.Serialization.JsonIgnore]
    public double CanvasHeight
    {
        get => Canvas.Height;
        set => Canvas.Height = value;
    }
}
