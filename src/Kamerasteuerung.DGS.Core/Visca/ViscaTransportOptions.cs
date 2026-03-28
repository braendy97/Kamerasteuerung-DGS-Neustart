namespace Kamerasteuerung.DGS.Core.Visca;

public sealed class ViscaTransportOptions
{
    public TimeSpan ConnectTimeout { get; init; } = TimeSpan.FromSeconds(2);
    public TimeSpan WriteTimeout { get; init; } = TimeSpan.FromSeconds(2);
    public TimeSpan ReadTimeout { get; init; } = TimeSpan.FromMilliseconds(400);

    public int MaxResponseBytes { get; init; } = 256;
    public bool ReadResponse { get; init; } = true;
}
