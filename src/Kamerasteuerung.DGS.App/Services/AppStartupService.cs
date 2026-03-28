using Kamerasteuerung.DGS.Core.Services;

namespace Kamerasteuerung.DGS.App.Services;

public sealed class AppStartupService
{
    private readonly SettingsFileService _settingsFileService;
    private readonly LayoutFileService _layoutFileService;
    private readonly AppSessionState _state;

    public AppStartupService(SettingsFileService settingsFileService, LayoutFileService layoutFileService, AppSessionState state)
    {
        _settingsFileService = settingsFileService;
        _layoutFileService = layoutFileService;
        _state = state;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _state.StatusMessage = "Einstellungen werden geladen ...";

        _state.Settings = await _settingsFileService.LoadAppSettingsAsync(AppPaths.SettingsFilePath, cancellationToken);

        var lastPath = _state.Settings.LastLayoutFilePath;
        if (string.IsNullOrWhiteSpace(lastPath))
        {
            _state.Layout = null;
            _state.StatusMessage = "Kein Layoutpfad konfiguriert";
            return;
        }

        if (!File.Exists(lastPath))
        {
            _state.Layout = null;
            _state.StatusMessage = "Layoutdatei nicht gefunden";
            return;
        }

        try
        {
            _state.StatusMessage = "Layout wird geladen ...";
            _state.Layout = await _layoutFileService.LoadLayoutAsync(lastPath, cancellationToken);
            _state.StatusMessage = $"Layout erfolgreich geladen: {_state.Layout.Name}";
        }
        catch
        {
            _state.Layout = null;
            _state.StatusMessage = "Layout konnte nicht geladen werden";
        }
    }
}
