namespace Kamerasteuerung.DGS.Core.Visca;

public sealed record ViscaResponseParseResult
{
    public required bool HasBytes { get; init; }
    public required bool IsParsed { get; init; }

    public required ViscaResponseKind Kind { get; init; }
    public string? Summary { get; init; }

    public ViscaParsedResponse? ParsedResponse { get; init; }

    public static ViscaResponseParseResult None(string summary)
        => new()
        {
            HasBytes = false,
            IsParsed = true,
            Kind = ViscaResponseKind.None,
            Summary = summary,
            ParsedResponse = null
        };

    public static ViscaResponseParseResult Unknown(string? summary)
        => new()
        {
            HasBytes = true,
            IsParsed = false,
            Kind = ViscaResponseKind.Unknown,
            Summary = summary,
            ParsedResponse = null
        };

    public static ViscaResponseParseResult Parsed(ViscaParsedResponse parsed)
        => new()
        {
            HasBytes = true,
            IsParsed = true,
            Kind = parsed.Kind,
            Summary = parsed.Summary,
            ParsedResponse = parsed
        };
}
