namespace Kamerasteuerung.DGS.Core.Visca;

public sealed record ViscaParsedResponse
{
    public required ViscaResponseKind Kind { get; init; }

    public string? Summary { get; init; }

    public int? SourceAddress { get; init; }
    public int? SocketNumber { get; init; }

    public byte? ErrorCode { get; init; }
    public string? ErrorDescription { get; init; }

    public int? FrameOffset { get; init; }
    public int? FrameLength { get; init; }
}
