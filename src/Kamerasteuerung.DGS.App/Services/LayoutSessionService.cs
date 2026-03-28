using Kamerasteuerung.DGS.Core.Models;
using Kamerasteuerung.DGS.Core.Services;
using System.Linq;

namespace Kamerasteuerung.DGS.App.Services;

public sealed class LayoutSessionService
{
    private readonly AppSessionState _state;
    private readonly LayoutFileService _layoutFileService;
    private readonly SettingsFileService _settingsFileService;

    public LayoutSessionService(AppSessionState state, LayoutFileService layoutFileService, SettingsFileService settingsFileService)
    {
        _state = state;
        _layoutFileService = layoutFileService;
        _settingsFileService = settingsFileService;
    }

    public bool AddBlock()
    {
        if (_state.Layout is null)
        {
            _state.StatusMessage = "Kein Layout geladen";
            return false;
        }

        var nextNumber = _state.Layout.Blocks.Count + 1;
        var offset = (_state.Layout.Blocks.Count % 10) * 20;

        var block = new LayoutBlock
        {
            Name = $"Block {nextNumber}",
            X = 50 + offset,
            Y = 50 + offset,
            Width = 300,
            Height = 200,
            RotationDegrees = 0
        };

        _state.Layout.Blocks.Add(block);
        _state.SelectedBlockId = block.Id;
        _state.StatusMessage = "Block hinzugefügt";
        return true;
    }

    public void SelectBlock(string? blockId)
    {
        _state.SelectedBlockId = blockId;
    }

    public bool DeleteSelectedBlock()
    {
        if (_state.Layout is null)
        {
            _state.StatusMessage = "Kein Layout geladen";
            return false;
        }

        if (string.IsNullOrWhiteSpace(_state.SelectedBlockId))
        {
            _state.StatusMessage = "Kein Block ausgewählt";
            return false;
        }

        var removed = _state.Layout.Blocks.RemoveAll(b => b.Id == _state.SelectedBlockId);
        if (removed > 0)
        {
            _state.SelectedBlockId = null;
            _state.StatusMessage = "Block gelöscht";
            return true;
        }

        _state.StatusMessage = "Block nicht gefunden";
        return false;
    }

    public void CreateNewLayout()
    {
        _state.Layout = new LayoutDocument
        {
            Name = "Neues Layout",
            Canvas = new LayoutCanvas { Width = 1400, Height = 900, Zoom = 1.0 },
            Cameras = [],
            Blocks = [],
            Seats = [],
            Participants = [],
            LastUpdatedUtc = DateTime.UtcNow
        };

        _state.SelectedBlockId = null;
        _state.StatusMessage = "Neues Layout erstellt";
    }

    public async Task OpenLayoutAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Layoutdatei öffnen",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".json", ".dgslayout.json" } },
                    { DevicePlatform.Android, new[] { "application/json" } },
                    { DevicePlatform.iOS, new[] { "public.json" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.json" } }
                })
            });

            if (result is null)
            {
                _state.StatusMessage = "Layout öffnen abgebrochen";
                return;
            }

            _state.StatusMessage = "Layout wird geladen ...";

            var tempPath = await CopyToLocalPathIfNeededAsync(result, cancellationToken);
            var loaded = await _layoutFileService.LoadLayoutAsync(tempPath, cancellationToken);

            _state.Layout = loaded;
            _state.Settings.LastLayoutFilePath = tempPath;
            await _settingsFileService.SaveAppSettingsAsync(AppPaths.SettingsFilePath, _state.Settings, cancellationToken);

            _state.StatusMessage = $"Layout erfolgreich geladen: {loaded.Name}";
        }
        catch (Exception)
        {
            _state.StatusMessage = "Layout konnte nicht geladen werden";
        }
    }

    public async Task SaveLayoutAsync(CancellationToken cancellationToken = default)
    {
        if (_state.Layout is null)
        {
            _state.StatusMessage = "Kein Layout zum Speichern vorhanden";
            return;
        }

        try
        {
            var path = _state.Settings.LastLayoutFilePath;
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Path.Combine(FileSystem.AppDataDirectory, "layouts", SanitizeFileName(_state.Layout.Name) + ".dgslayout.json");
            }

            await _layoutFileService.SaveLayoutAsync(path, _state.Layout, cancellationToken);

            _state.Settings.LastLayoutFilePath = path;
            await _settingsFileService.SaveAppSettingsAsync(AppPaths.SettingsFilePath, _state.Settings, cancellationToken);

            _state.StatusMessage = "Layout gespeichert";
        }
        catch (Exception)
        {
            _state.StatusMessage = "Layout konnte nicht gespeichert werden";
        }
    }

    private static async Task<string> CopyToLocalPathIfNeededAsync(FileResult file, CancellationToken cancellationToken)
    {
        var targetPath = Path.Combine(FileSystem.AppDataDirectory, "layouts", file.FileName);
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);

        await using var source = await file.OpenReadAsync();
        await using var target = File.Create(targetPath);
        await source.CopyToAsync(target, cancellationToken);

        return targetPath;
    }

    private static string SanitizeFileName(string name)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var cleaned = new string(name.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
        return string.IsNullOrWhiteSpace(cleaned) ? "layout" : cleaned;
    }
}
