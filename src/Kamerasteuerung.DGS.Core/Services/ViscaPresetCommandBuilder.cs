using Kamerasteuerung.DGS.Core.Constants;
using Kamerasteuerung.DGS.Core.Profiles;

namespace Kamerasteuerung.DGS.Core.Services;

public enum ViscaPresetCommandType
{
    Reset = 0,
    Set = 1,
    Recall = 2
}

public static class ViscaPresetCommandBuilder
{
    public static byte[] Build(CameraDeviceType deviceType, ViscaPresetCommandType commandType, int presetNumber)
    {
        if (presetNumber < PresetRange.AbsoluteMin || presetNumber > PresetRange.AbsoluteMax)
        {
            throw new ArgumentOutOfRangeException(nameof(presetNumber), $"Preset muss zwischen {PresetRange.AbsoluteMin} und {PresetRange.AbsoluteMax} liegen.");
        }

        var profile = CameraProfiles.Get(deviceType);

        if (!profile.SupportsExtendedPresets && presetNumber > 127)
        {
            throw new InvalidOperationException("Dieses Kameraprofil unterstützt keine erweiterten Presets.");
        }

        // Grundlogik:
        // - VISCA-Basisbefehl
        // - Presetnummer als Parameter
        // Die genaue Byte-Codierung für >127 wird im ersten Hardwareblock validiert.
        return
        [
            0x81,
            0x01,
            0x04,
            0x3F,
            (byte)commandType,
            (byte)presetNumber,
            0xFF
        ];
    }
}
