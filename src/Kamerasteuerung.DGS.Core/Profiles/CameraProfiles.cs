namespace Kamerasteuerung.DGS.Core.Profiles;

public static class CameraProfiles
{
    public static readonly CameraProfile V600 = new(
        DeviceType: CameraDeviceType.V600,
        DisplayName: "V600",
        DefaultPort: 5678,
        SupportsAiTracking: false,
        WaitForAckOnManualMove: false,
        SupportsExtendedPresets: true);

    public static readonly CameraProfile SMTAVV60XL = new(
        DeviceType: CameraDeviceType.SMTAVV60XL,
        DisplayName: "SMTAV V60XL",
        DefaultPort: 5678,
        SupportsAiTracking: true,
        WaitForAckOnManualMove: true,
        SupportsExtendedPresets: true);

    public static CameraProfile Get(CameraDeviceType deviceType) =>
        deviceType switch
        {
            CameraDeviceType.V600 => V600,
            CameraDeviceType.SMTAVV60XL => SMTAVV60XL,
            _ => throw new ArgumentOutOfRangeException(nameof(deviceType), deviceType, "Unbekannter Kameratyp.")
        };
}
