using Kamerasteuerung.DGS.Core.Models;

namespace Kamerasteuerung.DGS.Core.Visca;

public sealed class ViscaPresetCommandSender
{
    private readonly ViscaPresetCommandService _commandService = new();
    private readonly ViscaTcpTransportService _transportService = new();

    public async Task<ViscaPresetSendResult> SendPresetCommandAsync(CameraProfile cameraProfile, ViscaPresetAction action, int presetNumber, ViscaTransportOptions? transportOptions = null, CancellationToken cancellationToken = default)
    {
        var build = _commandService.BuildPresetCommand(cameraProfile, action, presetNumber);
        if (!build.IsSuccess || build.CommandBytes is null)
        {
            return ViscaPresetSendResult.Failure(build.Error ?? "Befehl konnte nicht gebaut werden", isTimeout: false, action, presetNumber);
        }

        var transport = await _transportService.SendAsync(cameraProfile, build.CommandBytes, transportOptions, cancellationToken);
        if (!transport.IsSuccess)
        {
            var result = ViscaPresetSendResult.Failure(transport.Error ?? "Senden fehlgeschlagen", transport.IsTimeout, action, presetNumber);
            return result with
            {
                CommandBytes = transport.SentBytes,
                CommandHex = transport.SentHex,
                ResponseBytes = transport.ResponseBytes,
                ResponseHex = transport.ResponseHex
            };
        }

        return ViscaPresetSendResult.Success(action, presetNumber, transport.SentBytes!, transport.ResponseBytes);
    }
}
