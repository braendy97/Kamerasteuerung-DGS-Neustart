namespace Kamerasteuerung.DGS.Core.Visca;

public sealed record ViscaPresetSendResult
{
    public required bool IsSuccess { get; init; }

    public string? Error { get; init; }
    public bool IsTimeout { get; init; }

    public ViscaPresetAction Action { get; init; }
    public int PresetNumber { get; init; }

    public byte[]? CommandBytes { get; init; }
    public string? CommandHex { get; init; }

    public byte[]? ResponseBytes { get; init; }
    public string? ResponseHex { get; init; }

    public bool ResponseReadAttempted { get; init; }
    public ViscaResponseKind ResponseKind { get; init; } = ViscaResponseKind.None;
    public string? ResponseSummary { get; init; }
    public byte? ResponseErrorCode { get; init; }
    public string? ResponseErrorDescription { get; init; }

    public static ViscaPresetSendResult Failure(string error, bool isTimeout, ViscaPresetAction action, int presetNumber)
        => new()
        {
            IsSuccess = false,
            Error = error,
            IsTimeout = isTimeout,
            Action = action,
            PresetNumber = presetNumber,
            ResponseKind = ViscaResponseKind.None
        };

    public static ViscaPresetSendResult Success(ViscaPresetAction action, int presetNumber, byte[] commandBytes, byte[]? responseBytes)
        => new()
        {
            IsSuccess = true,
            Action = action,
            PresetNumber = presetNumber,
            CommandBytes = commandBytes,
            CommandHex = ViscaHex.ToHexString(commandBytes),
            ResponseBytes = responseBytes,
            ResponseHex = responseBytes is null ? null : ViscaHex.ToHexString(responseBytes),
            ResponseReadAttempted = true
        };
}
