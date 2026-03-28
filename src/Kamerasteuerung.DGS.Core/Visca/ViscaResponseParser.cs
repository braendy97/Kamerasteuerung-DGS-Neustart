namespace Kamerasteuerung.DGS.Core.Visca;

public static class ViscaResponseParser
{
    public static ViscaResponseParseResult Parse(byte[]? responseBytes, bool responseReadAttempted, bool isTimeout)
    {
        if (!responseReadAttempted)
        {
            return ViscaResponseParseResult.None("Antwortlesen deaktiviert");
        }

        if (isTimeout)
        {
            return ViscaResponseParseResult.None("Timeout beim Antwortlesen");
        }

        if (responseBytes is null)
        {
            return ViscaResponseParseResult.None("Keine Antwort (null)");
        }

        if (responseBytes.Length == 0)
        {
            return ViscaResponseParseResult.None("Keine Antwort (leer)");
        }

        // Minimaler VISCA-Parser (für diesen Projektstand):
        // - Frames enden mit 0xFF
        // - ACK:        0x90 0x4y 0xFF
        // - Completion: 0x90 0x5y 0xFF
        // - Error:      0x90 0x6y 0xFF
        //   y = socketNumber (low nibble)
        // Quelle/Adresse steckt im low nibble des ersten Bytes (0x9n)
        // Es können mehrere Frames im Buffer vorkommen.

        var best = FindBestFrame(responseBytes);
        if (best is null)
        {
            return ViscaResponseParseResult.Unknown("Kein parsebarer VISCA-Frame gefunden");
        }

        return ViscaResponseParseResult.Parsed(best);
    }

    private static ViscaParsedResponse? FindBestFrame(byte[] bytes)
    {
        ViscaParsedResponse? best = null;

        for (var i = 0; i < bytes.Length; i++)
        {
            var b0 = bytes[i];
            if ((b0 & 0xF0) != 0x90)
            {
                continue;
            }

            var frameEnd = IndexOf(bytes, 0xFF, startIndex: i);
            if (frameEnd < 0)
            {
                break;
            }

            var frameLength = frameEnd - i + 1;
            var parsed = ParseSingleFrame(bytes, i, frameLength);
            if (parsed is null)
            {
                i = frameEnd;
                continue;
            }

            if (best is null || Score(parsed.Kind) > Score(best.Kind))
            {
                best = parsed;
            }

            i = frameEnd;
        }

        return best;
    }

    private static int IndexOf(byte[] bytes, byte value, int startIndex)
    {
        for (var i = startIndex; i < bytes.Length; i++)
        {
            if (bytes[i] == value)
            {
                return i;
            }
        }

        return -1;
    }

    private static int Score(ViscaResponseKind kind)
        => kind switch
        {
            ViscaResponseKind.Error => 3,
            ViscaResponseKind.Completion => 2,
            ViscaResponseKind.Ack => 1,
            _ => 0
        };

    private static ViscaParsedResponse? ParseSingleFrame(byte[] bytes, int offset, int length)
    {
        if (length < 3)
        {
            return null;
        }

        var b0 = bytes[offset];
        var b1 = bytes[offset + 1];
        var bLast = bytes[offset + length - 1];

        if ((b0 & 0xF0) != 0x90 || bLast != 0xFF)
        {
            return null;
        }

        var sourceAddress = b0 & 0x0F;
        var socket = b1 & 0x0F;
        var kindNibble = b1 & 0xF0;

        if (kindNibble == 0x40)
        {
            return new ViscaParsedResponse
            {
                Kind = ViscaResponseKind.Ack,
                Summary = $"ACK (addr={sourceAddress}, socket={socket})",
                SourceAddress = sourceAddress,
                SocketNumber = socket,
                FrameOffset = offset,
                FrameLength = length
            };
        }

        if (kindNibble == 0x50)
        {
            return new ViscaParsedResponse
            {
                Kind = ViscaResponseKind.Completion,
                Summary = $"Completion (addr={sourceAddress}, socket={socket})",
                SourceAddress = sourceAddress,
                SocketNumber = socket,
                FrameOffset = offset,
                FrameLength = length
            };
        }

        if (kindNibble == 0x60)
        {
            byte? errorCode = null;
            if (length >= 4)
            {
                errorCode = bytes[offset + 2];
            }

            return new ViscaParsedResponse
            {
                Kind = ViscaResponseKind.Error,
                Summary = $"Error (addr={sourceAddress}, socket={socket}, code=0x{(errorCode ?? 0):X2})",
                SourceAddress = sourceAddress,
                SocketNumber = socket,
                ErrorCode = errorCode,
                ErrorDescription = DescribeErrorCode(errorCode),
                FrameOffset = offset,
                FrameLength = length
            };
        }

        return new ViscaParsedResponse
        {
            Kind = ViscaResponseKind.Unknown,
            Summary = $"Unbekannte VISCA-Antwort (addr={sourceAddress}, b1=0x{b1:X2})",
            SourceAddress = sourceAddress,
            SocketNumber = socket,
            FrameOffset = offset,
            FrameLength = length
        };
    }

    private static string? DescribeErrorCode(byte? errorCode)
    {
        if (errorCode is null)
        {
            return null;
        }

        return errorCode.Value switch
        {
            0x01 => "Message length error",
            0x02 => "Syntax error",
            0x03 => "Command buffer full",
            0x04 => "Command cancelled",
            0x05 => "No socket",
            0x41 => "Command not executable",
            _ => null
        };
    }
}
