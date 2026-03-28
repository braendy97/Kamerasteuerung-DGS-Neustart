using Kamerasteuerung.DGS.Core.Models;
using System.Net.Sockets;

namespace Kamerasteuerung.DGS.Core.Visca;

public sealed class ViscaTcpTransportService
{
    public async Task<ViscaTransportResult> SendAsync(CameraProfile cameraProfile, byte[] commandBytes, ViscaTransportOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (cameraProfile is null)
        {
            return ViscaTransportResult.Failure("CameraProfile ist null", isTimeout: false, sentBytes: null, responseAttempted: false);
        }

        if (string.IsNullOrWhiteSpace(cameraProfile.Host))
        {
            return ViscaTransportResult.Failure("Host ist leer", isTimeout: false, sentBytes: commandBytes, responseAttempted: false);
        }

        if (cameraProfile.Port <= 0 || cameraProfile.Port > 65535)
        {
            return ViscaTransportResult.Failure("Port ungültig", isTimeout: false, sentBytes: commandBytes, responseAttempted: false);
        }

        if (commandBytes is null || commandBytes.Length == 0)
        {
            return ViscaTransportResult.Failure("Befehl ist leer", isTimeout: false, sentBytes: commandBytes, responseAttempted: false);
        }

        options ??= CreateDefaultOptions(cameraProfile);

        var responseAttempted = options.ReadResponse;

        using var client = new TcpClient();

        try
        {
            using var connectCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            connectCts.CancelAfter(options.ConnectTimeout);

            await client.ConnectAsync(cameraProfile.Host, cameraProfile.Port, connectCts.Token);

            using var stream = client.GetStream();

            using (var writeCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                writeCts.CancelAfter(options.WriteTimeout);
                await stream.WriteAsync(commandBytes, writeCts.Token);
                await stream.FlushAsync(writeCts.Token);
            }

            if (!options.ReadResponse)
            {
                return ViscaTransportResult.Success(commandBytes, responseBytes: null, responseAttempted: false);
            }

            try
            {
                using var readCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                readCts.CancelAfter(options.ReadTimeout);

                var buffer = new byte[Math.Max(1, options.MaxResponseBytes)];
                var read = await stream.ReadAsync(buffer, readCts.Token);

                if (read <= 0)
                {
                    return ViscaTransportResult.Success(commandBytes, responseBytes: Array.Empty<byte>(), responseAttempted: true);
                }

                var response = buffer.AsSpan(0, read).ToArray();
                return ViscaTransportResult.Success(commandBytes, response, responseAttempted: true);
            }
            catch (OperationCanceledException)
            {
                return ViscaTransportResult.Failure("ReadTimeout", isTimeout: true, sentBytes: commandBytes, responseAttempted: true);
            }
        }
        catch (OperationCanceledException)
        {
            return ViscaTransportResult.Failure("Connect/Write Timeout", isTimeout: true, sentBytes: commandBytes, responseAttempted: responseAttempted);
        }
        catch (SocketException ex)
        {
            return ViscaTransportResult.Failure($"SocketFehler: {ex.SocketErrorCode}", isTimeout: false, sentBytes: commandBytes, responseAttempted: responseAttempted);
        }
        catch (IOException ex)
        {
            return ViscaTransportResult.Failure($"IO-Fehler: {ex.Message}", isTimeout: false, sentBytes: commandBytes, responseAttempted: responseAttempted);
        }
        catch (Exception ex)
        {
            return ViscaTransportResult.Failure(ex.Message, isTimeout: false, sentBytes: commandBytes, responseAttempted: responseAttempted);
        }
    }

    public static ViscaTransportOptions CreateDefaultOptions(CameraProfile cameraProfile)
    {
        return cameraProfile.CameraType switch
        {
            CameraType.StageSmtavV60XL => new ViscaTransportOptions
            {
                ConnectTimeout = TimeSpan.FromSeconds(2),
                WriteTimeout = TimeSpan.FromSeconds(2),
                ReadTimeout = TimeSpan.FromMilliseconds(600),
                ReadResponse = true,
                MaxResponseBytes = 256
            },
            _ => new ViscaTransportOptions
            {
                ConnectTimeout = TimeSpan.FromSeconds(2),
                WriteTimeout = TimeSpan.FromSeconds(2),
                ReadTimeout = TimeSpan.FromMilliseconds(400),
                ReadResponse = true,
                MaxResponseBytes = 256
            }
        };
    }
}
