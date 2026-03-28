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

    public ViscaResponseKind ResponseKind { get; init; } = ViscaResponseKind.None;
    public string? ResponseSummary { get; init; }
    public ViscaParsedResponse? ParsedResponse { get; init; }

    public static ViscaTransportResult Success(byte[] sentBytes, byte[]? responseBytes, bool responseAttempted)
    {
        var parsed = ViscaResponseParser.Parse(responseBytes, responseAttempted, isTimeout: false);

        return new ViscaTransportResult
        {
            IsSuccess = true,
            SentBytes = sentBytes,
            SentHex = ViscaHex.ToHexString(sentBytes),
            ResponseReadAttempted = responseAttempted,
            ResponseBytes = responseBytes,
            ResponseHex = responseBytes is null ? null : ViscaHex.ToHexString(responseBytes),
            ResponseKind = parsed.Kind,
            ResponseSummary = parsed.Summary,
            ParsedResponse = parsed.ParsedResponse
        };
    }

    public static ViscaTransportResult Failure(string error, bool isTimeout, byte[]? sentBytes, bool responseAttempted)
    {
        var parsed = ViscaResponseParser.Parse(responseBytes: null, responseReadAttempted: responseAttempted, isTimeout: isTimeout);

        return new ViscaTransportResult
        {
            IsSuccess = false,
            Error = error,
            IsTimeout = isTimeout,
            SentBytes = sentBytes,
            SentHex = sentBytes is null ? null : ViscaHex.ToHexString(sentBytes),
            ResponseReadAttempted = responseAttempted,
            ResponseKind = parsed.Kind,
            ResponseSummary = parsed.Summary,
            ParsedResponse = null
        };
    }
}
