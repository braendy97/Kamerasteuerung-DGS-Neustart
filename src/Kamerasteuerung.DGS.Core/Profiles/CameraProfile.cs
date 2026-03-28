namespace Kamerasteuerung.DGS.Core.Profiles;

public sealed record CameraProfile(
    CameraDeviceType DeviceType,
    string DisplayName,
    int DefaultPort,
    bool SupportsAiTracking,
    bool WaitForAckOnManualMove,
    bool SupportsExtendedPresets);
