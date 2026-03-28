namespace Kamerasteuerung.DGS.Core.Models;

public sealed class ParticipantEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string DisplayName { get; set; } = string.Empty;
}
