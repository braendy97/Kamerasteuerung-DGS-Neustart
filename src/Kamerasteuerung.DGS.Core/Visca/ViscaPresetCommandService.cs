using Kamerasteuerung.DGS.Core.Constants;
using Kamerasteuerung.DGS.Core.Models;
using Kamerasteuerung.DGS.Core.Profiles;
using Kamerasteuerung.DGS.Core.Services;

namespace Kamerasteuerung.DGS.Core.Visca;

public sealed class ViscaPresetCommandService
{
    public ViscaCommandBuildResult BuildPresetCommand(Kamerasteuerung.DGS.Core.Models.CameraProfile cameraProfile, ViscaPresetAction action, int presetNumber)
    {
        if (cameraProfile is null)
        {
            return ViscaCommandBuildResult.Failure("CameraProfile ist null");
        }

        if (!PresetRange.IsWithinUserRange(presetNumber))
        {
            return ViscaCommandBuildResult.Failure($"Preset ungültig (erlaubt: {PresetRange.UserMin}–{PresetRange.UserMax})");
        }

        var deviceType = cameraProfile.CameraType switch
        {
            CameraType.AudienceV600 => CameraDeviceType.V600,
            CameraType.StageSmtavV60XL => CameraDeviceType.SMTAVV60XL,
            _ => CameraDeviceType.Unknown
        };

        if (deviceType == CameraDeviceType.Unknown)
        {
            return ViscaCommandBuildResult.Failure("Unbekannter Kameratyp");
        }

        var commandType = action switch
        {
            ViscaPresetAction.Clear => ViscaPresetCommandType.Reset,
            ViscaPresetAction.Store => ViscaPresetCommandType.Set,
            ViscaPresetAction.Recall => ViscaPresetCommandType.Recall,
            _ => ViscaPresetCommandType.Recall
        };

        try
        {
            var bytes = ViscaPresetCommandBuilder.Build(deviceType, commandType, presetNumber);
            return ViscaCommandBuildResult.Success(bytes);
        }
        catch (Exception ex)
        {
            return ViscaCommandBuildResult.Failure(ex.Message);
        }
    }
}
