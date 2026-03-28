namespace Kamerasteuerung.DGS.Core.Constants;

public static class PresetRange
{
    public const int UserMin = 10;
    public const int UserMax = 210;
    public const int AbsoluteMin = 0;
    public const int AbsoluteMax = 255;

    public static bool IsWithinUserRange(int presetNumber) =>
        presetNumber >= UserMin && presetNumber <= UserMax;
}
