namespace Kamerasteuerung.DGS.Core.Visca;

public sealed class ViscaTransportResult
{
    public required bool IsSuccess { get; init; }

    public string? Error { get; init; }
    public bool IsTimeout { get; init; }

    public string? SentHex { get; init; }
    public byte[]? SentBytes { get; init; }

    public bool ResponseReadAttempted { get; init; }
    public byte[]? ResponseBytes { get; init; }
    public string? ResponseHex { get; init; }

    public static ViscaTransportResult Success(byte[] sentBytes, byte[]? responseBytes, bool responseAttempted)
    {
        return new ViscaTransportResult
        {
            IsSuccess = true,
            SentBytes = sentBytes,
            SentHex = ViscaHex.ToHexString(sentBytes),
            ResponseReadAttempted = responseAttempted,
            ResponseBytes = responseBytes,
            ResponseHex = responseBytes is null ? null : ViscaHex.ToHexString(responseBytes)
        };
    }

    public static ViscaTransportResult Failure(string error, bool isTimeout, byte[]? sentBytes, bool responseAttempted)
    {
        return new ViscaTransportResult
        {
            IsSuccess = false,
            Error = error,
            IsTimeout = isTimeout,
            SentBytes = sentBytes,
            SentHex = sentBytes is null ? null : ViscaHex.ToHexString(sentBytes),
            ResponseReadAttempted = responseAttempted
        };
    }
}
