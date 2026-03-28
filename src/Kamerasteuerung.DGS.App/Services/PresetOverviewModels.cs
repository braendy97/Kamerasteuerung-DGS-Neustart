using Kamerasteuerung.DGS.Core.Models;

namespace Kamerasteuerung.DGS.App.Services;

public enum PresetSeatStatus
{
    None = 0,
    Valid = 1,
    Invalid = 2,
    Conflict = 3
}

public sealed class PresetSeatStatusEntry
{
    public required string SeatId { get; init; }
    public required string SeatLabel { get; init; }
    public required string BlockId { get; init; }
    public required string BlockName { get; init; }
    public required CameraType? BlockCameraType { get; init; }
    public required int BlockPresetPriority { get; init; }
    public required int PresetNumber { get; init; }
    public required PresetSeatStatus Status { get; init; }
}

public sealed class PresetBlockStatusSummary
{
    public required string BlockId { get; init; }
    public required string BlockName { get; init; }
    public required CameraType? CameraType { get; init; }
    public required int PresetPriority { get; init; }
    public required int SeatCount { get; init; }
    public int? FirstValidPreset { get; init; }
    public int? LastValidPreset { get; init; }
}

public sealed class PresetOverview
{
    public required int PresetStart { get; init; }
    public required int PresetEnd { get; init; }
    public required IReadOnlyList<PresetBlockStatusSummary> Blocks { get; init; }
    public required IReadOnlyList<PresetSeatStatusEntry> Seats { get; init; }
}
