namespace Kamerasteuerung.DGS.Core.Visca;

public sealed class ViscaCommandBuildResult
{
    public required bool IsSuccess { get; init; }
    public byte[]? CommandBytes { get; init; }
    public string? Error { get; init; }

    public static ViscaCommandBuildResult Success(byte[] bytes) => new()
    {
        IsSuccess = true,
        CommandBytes = bytes
    };

    public static ViscaCommandBuildResult Failure(string error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}
