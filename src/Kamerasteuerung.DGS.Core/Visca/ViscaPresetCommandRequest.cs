using Kamerasteuerung.DGS.Core.Models;

namespace Kamerasteuerung.DGS.Core.Visca;

public sealed class ViscaPresetCommandRequest
{
    public required CameraType CameraType { get; init; }
    public required int PresetNumber { get; init; }
    public required ViscaPresetAction Action { get; init; }
}
