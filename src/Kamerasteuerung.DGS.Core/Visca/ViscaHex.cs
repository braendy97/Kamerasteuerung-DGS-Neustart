using System.Globalization;

namespace Kamerasteuerung.DGS.Core.Visca;

public static class ViscaHex
{
    public static string ToHexString(byte[] bytes)
    {
        if (bytes is null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        if (bytes.Length == 0)
        {
            return string.Empty;
        }

        return string.Join(" ", bytes.Select(b => b.ToString("X2", CultureInfo.InvariantCulture)));
    }
}
