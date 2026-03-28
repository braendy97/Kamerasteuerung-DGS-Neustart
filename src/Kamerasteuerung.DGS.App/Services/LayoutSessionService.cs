using Kamerasteuerung.DGS.Core.Models;
using Kamerasteuerung.DGS.Core.Services;
using System.Linq;

namespace Kamerasteuerung.DGS.App.Services;

public sealed class LayoutSessionService
{
    private readonly AppSessionState _state;
    private readonly LayoutFileService _layoutFileService;
    private readonly SettingsFileService _settingsFileService;

    public const int UserPresetStart = 10;
    public const int UserPresetEnd = 210;

    public PresetOverview? CurrentPresetOverview { get; private set; }
    public string PresetOverviewText { get; private set; } = string.Empty;

    public LayoutSessionService(AppSessionState state, LayoutFileService layoutFileService, SettingsFileService settingsFileService)
    {
        _state = state;
        _layoutFileService = layoutFileService;
        _settingsFileService = settingsFileService;
    }

    public bool SetSelectedBlockPresetPriority(int presetPriority)
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

        var block = _state.Layout.Blocks.FirstOrDefault(b => b.Id == _state.SelectedBlockId);
        if (block is null)
        {
            _state.StatusMessage = "Block nicht gefunden";
            return false;
        }

        block.PresetPriority = presetPriority;
        _state.StatusMessage = $"Preset-Priorität gesetzt: {presetPriority} (niedriger = früher)";
        return true;
    }

    public bool SetSelectedBlockPresetsFromStart(int startPreset)
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

        if (startPreset < UserPresetStart || startPreset > UserPresetEnd)
        {
            _state.StatusMessage = $"Start-Preset ungültig (erlaubt: {UserPresetStart}–{UserPresetEnd})";
            return false;
        }

        var block = _state.Layout.Blocks.FirstOrDefault(b => b.Id == _state.SelectedBlockId);
        if (block is null)
        {
            _state.StatusMessage = "Block nicht gefunden";
            return false;
        }

        var seats = _state.Layout.Seats
            .Where(s => s.BlockId == block.Id)
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.Y)
            .ThenBy(s => s.X)
            .ThenBy(s => s.Label)
            .ToList();

        var preset = startPreset;
        var setCount = 0;
        var overflowCount = 0;

        foreach (var seat in seats)
        {
            if (preset > UserPresetEnd)
            {
                seat.PresetNumber = 0;
                overflowCount++;
                continue;
            }

            seat.PresetNumber = preset;
            preset++;
            setCount++;
        }

        _state.StatusMessage = overflowCount == 0
            ? $"Block-Presets gesetzt ab {startPreset} ({setCount} Sitze)"
            : $"Block-Presets gesetzt ab {startPreset} ({setCount} Sitze) | Überlauf: {overflowCount} → 0";

        return true;
    }

    public bool SetSeatPreset(string seatId, int presetNumber)
    {
        if (_state.Layout is null)
        {
            _state.StatusMessage = "Kein Layout geladen";
            return false;
        }

        if (presetNumber != 0 && (presetNumber < UserPresetStart || presetNumber > UserPresetEnd))
        {
            _state.StatusMessage = $"Preset ungültig (erlaubt: 0 oder {UserPresetStart}–{UserPresetEnd})";
            return false;
        }

        var seat = _state.Layout.Seats.FirstOrDefault(s => s.Id == seatId);
        if (seat is null)
        {
            _state.StatusMessage = "Sitz nicht gefunden";
            return false;
        }

        seat.PresetNumber = presetNumber;
        _state.StatusMessage = presetNumber == 0
            ? "Sitz-Preset entfernt (0)"
            : $"Sitz-Preset gesetzt: {presetNumber}";
        return true;
    }

    public bool GenerateSeatsForSelectedBlock(bool overwriteExisting = true)
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

        var block = _state.Layout.Blocks.FirstOrDefault(b => b.Id == _state.SelectedBlockId);
        if (block is null)
        {
            _state.StatusMessage = "Block nicht gefunden";
            return false;
        }

        if (overwriteExisting)
        {
            _state.Layout.Seats.RemoveAll(s => s.BlockId == block.Id);
        }

        const double padding = 10;
        const double gap = 6;
        const double seatSize = 28;

        var innerWidth = Math.Max(0, block.Width - (padding * 2));
        var innerHeight = Math.Max(0, block.Height - (padding * 2));

        var cols = Math.Max(1, (int)Math.Floor((innerWidth + gap) / (seatSize + gap)));
        var rows = Math.Max(1, (int)Math.Floor((innerHeight + gap) / (seatSize + gap)));

        var seatCount = rows * cols;
        var startOrder = _state.Layout.Seats.Count + 1;

        var created = 0;
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                created++;
                var x = block.X + padding + c * (seatSize + gap);
                var y = block.Y + padding + r * (seatSize + gap);

                _state.Layout.Seats.Add(new LayoutSeat
                {
                    BlockId = block.Id,
                    Label = $"S{created}",
                    X = x,
                    Y = y,
                    Width = seatSize,
                    Height = seatSize,
                    SortOrder = startOrder + created - 1,
                    PresetNumber = 0
                });
            }
        }

        ReassignUserPresets();
        _state.StatusMessage = $"Sitze erzeugt: {seatCount} (Presets neu berechnet)";
        return true;
    }

    public sealed class PresetDiagnostics
    {
        public required int PresetStart { get; init; }
        public required int PresetEnd { get; init; }
        public required int ValidAssignedSeatCount { get; init; }
        public required int InvalidSeatCount { get; init; }
        public required int ConflictSeatCount { get; init; }
        public required int ConflictPresetCount { get; init; }
        public required int DistinctValidPresetCount { get; init; }

        public int? SelectedBlockFirstPreset { get; init; }
        public int? SelectedBlockLastPreset { get; init; }
        public int SelectedBlockValidSeatCount { get; init; }
        public int SelectedBlockInvalidSeatCount { get; init; }
        public int SelectedBlockConflictSeatCount { get; init; }
    }

    public bool ReassignUserPresets()
    {
        if (_state.Layout is null)
        {
            _state.StatusMessage = "Kein Layout geladen";
            return false;
        }

        var orderedBlocks = _state.Layout.Blocks
            .OrderBy(b => b.PresetPriority)
            .ThenBy(b => b.Y)
            .ThenBy(b => b.X)
            .ThenBy(b => b.Name)
            .ToList();

        var presets = Enumerable.Range(UserPresetStart, UserPresetEnd - UserPresetStart + 1).GetEnumerator();

        foreach (var block in orderedBlocks)
        {
            var seats = _state.Layout.Seats
                .Where(s => s.BlockId == block.Id)
                .OrderBy(s => s.SortOrder)
                .ThenBy(s => s.Y)
                .ThenBy(s => s.X)
                .ThenBy(s => s.Label)
                .ToList();

            foreach (var seat in seats)
            {
                if (!presets.MoveNext())
                {
                    seat.PresetNumber = 0;
                    continue;
                }

                seat.PresetNumber = presets.Current;
            }
        }

        // Seats ohne BlockId oder mit unbekanntem BlockId werden bewusst nicht in die Vergabe einbezogen.
        // Sie behalten/erhalten PresetNumber = 0, damit sie im Designer als "nicht zugewiesen" erkennbar bleiben.
        foreach (var seat in _state.Layout.Seats.Where(s => string.IsNullOrWhiteSpace(s.BlockId) || !_state.Layout.Blocks.Any(b => b.Id == s.BlockId)))
        {
            seat.PresetNumber = 0;
        }

        _state.StatusMessage = "Presets neu berechnet";
        return true;
    }

    public PresetOverview? BuildPresetOverview(bool setAsCurrent = true)
    {
        if (_state.Layout is null)
        {
            CurrentPresetOverview = null;
            PresetOverviewText = string.Empty;
            return null;
        }

        static bool IsValidUserPreset(int preset) => preset >= UserPresetStart && preset <= UserPresetEnd;

        var seats = _state.Layout.Seats;
        var validSeats = seats.Where(s => IsValidUserPreset(s.PresetNumber)).ToList();
        var conflictPresetNumbers = validSeats
            .GroupBy(s => s.PresetNumber)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToHashSet();

        var orderedBlocks = _state.Layout.Blocks
            .OrderBy(b => b.PresetPriority)
            .ThenBy(b => b.Y)
            .ThenBy(b => b.X)
            .ThenBy(b => b.Name)
            .ToList();

        var blockNameById = orderedBlocks.ToDictionary(b => b.Id, b => b.Name);
        var blockById = orderedBlocks.ToDictionary(b => b.Id, b => b);

        var blockSummaries = orderedBlocks
            .Select(b =>
            {
                var blockValidPresets = seats
                    .Where(s => s.BlockId == b.Id && IsValidUserPreset(s.PresetNumber))
                    .Select(s => s.PresetNumber)
                    .Order()
                    .ToList();

                return new PresetBlockStatusSummary
                {
                    BlockId = b.Id,
                    BlockName = b.Name,
                    CameraType = b.CameraType,
                    PresetPriority = b.PresetPriority,
                    SeatCount = seats.Count(s => s.BlockId == b.Id),
                    FirstValidPreset = blockValidPresets.Count == 0 ? null : blockValidPresets.First(),
                    LastValidPreset = blockValidPresets.Count == 0 ? null : blockValidPresets.Last()
                };
            })
            .ToList();

        var seatEntries = new List<PresetSeatStatusEntry>();

        foreach (var block in orderedBlocks)
        {
            var blockSeats = seats
                .Where(s => s.BlockId == block.Id)
                .OrderBy(s => s.SortOrder)
                .ThenBy(s => s.Y)
                .ThenBy(s => s.X)
                .ThenBy(s => s.Label)
                .ToList();

            foreach (var seat in blockSeats)
            {
                var status = PresetSeatStatus.None;
                if (IsValidUserPreset(seat.PresetNumber))
                {
                    status = conflictPresetNumbers.Contains(seat.PresetNumber)
                        ? PresetSeatStatus.Conflict
                        : PresetSeatStatus.Valid;
                }
                else if (seat.PresetNumber != 0)
                {
                    status = PresetSeatStatus.Invalid;
                }

                seatEntries.Add(new PresetSeatStatusEntry
                {
                    SeatId = seat.Id,
                    SeatLabel = string.IsNullOrWhiteSpace(seat.Label) ? seat.Id : seat.Label,
                    BlockId = block.Id,
                    BlockName = block.Name,
                    BlockCameraType = block.CameraType,
                    BlockPresetPriority = block.PresetPriority,
                    PresetNumber = seat.PresetNumber,
                    Status = status
                });
            }
        }

        // Seats, die nicht sauber einem Block zugeordnet sind, trotzdem exportierbar aufführen.
        var unassignedSeats = seats
            .Where(s => string.IsNullOrWhiteSpace(s.BlockId) || !blockNameById.ContainsKey(s.BlockId))
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.Y)
            .ThenBy(s => s.X)
            .ThenBy(s => s.Label)
            .ToList();

        foreach (var seat in unassignedSeats)
        {
            var status = PresetSeatStatus.None;
            if (IsValidUserPreset(seat.PresetNumber))
            {
                status = conflictPresetNumbers.Contains(seat.PresetNumber)
                    ? PresetSeatStatus.Conflict
                    : PresetSeatStatus.Valid;
            }
            else if (seat.PresetNumber != 0)
            {
                status = PresetSeatStatus.Invalid;
            }

            var blockId = seat.BlockId ?? string.Empty;
            seatEntries.Add(new PresetSeatStatusEntry
            {
                SeatId = seat.Id,
                SeatLabel = string.IsNullOrWhiteSpace(seat.Label) ? seat.Id : seat.Label,
                BlockId = blockId,
                BlockName = string.IsNullOrWhiteSpace(blockId) ? "(ohne Block)" : "(unbekannter Block)",
                BlockCameraType = string.IsNullOrWhiteSpace(blockId) || !blockById.TryGetValue(blockId, out var b) ? null : b.CameraType,
                BlockPresetPriority = string.IsNullOrWhiteSpace(blockId) || !blockById.TryGetValue(blockId, out var b2) ? int.MaxValue : b2.PresetPriority,
                PresetNumber = seat.PresetNumber,
                Status = status
            });
        }

        var overview = new PresetOverview
        {
            PresetStart = UserPresetStart,
            PresetEnd = UserPresetEnd,
            Blocks = blockSummaries,
            Seats = seatEntries
        };

        var lines = new List<string>
        {
            $"Preset-Übersicht (Nutzerbereich {UserPresetStart}–{UserPresetEnd}, 0=nicht zugewiesen)",
            $"Layout: {_state.Layout.Name}",
            string.Empty,
            "Block | Sitz | Kameraart | Preset | Status"
        };

        foreach (var s in overview.Seats)
        {
            var cam = s.BlockCameraType switch
            {
                CameraType.AudienceV600 => "Zuschauer",
                CameraType.StageSmtavV60XL => "Bühne",
                _ => "(keine)"
            };

            lines.Add($"{s.BlockName} | {s.SeatLabel} | {cam} | {s.PresetNumber} | {s.Status}");
        }

        if (setAsCurrent)
        {
            CurrentPresetOverview = overview;
            PresetOverviewText = string.Join(Environment.NewLine, lines);
        }

        return overview;
    }

    public PresetDiagnostics GetPresetDiagnostics(string? selectedBlockId = null)
    {
        if (_state.Layout is null)
        {
            return new PresetDiagnostics
            {
                PresetStart = UserPresetStart,
                PresetEnd = UserPresetEnd,
                ValidAssignedSeatCount = 0,
                InvalidSeatCount = 0,
                ConflictSeatCount = 0,
                ConflictPresetCount = 0,
                DistinctValidPresetCount = 0,
                SelectedBlockValidSeatCount = 0,
                SelectedBlockInvalidSeatCount = 0,
                SelectedBlockConflictSeatCount = 0
            };
        }

        var seats = _state.Layout.Seats;
        static bool IsValidUserPreset(int preset) => preset >= UserPresetStart && preset <= UserPresetEnd;

        var validSeats = seats.Where(s => IsValidUserPreset(s.PresetNumber)).ToList();
        var invalidSeats = seats.Where(s => s.PresetNumber != 0 && !IsValidUserPreset(s.PresetNumber)).ToList();

        var duplicateGroups = validSeats.GroupBy(s => s.PresetNumber).Where(g => g.Count() > 1).ToList();
        var conflictSeatIds = duplicateGroups.SelectMany(g => g.Select(s => s.Id)).ToHashSet();

        int? selFirst = null;
        int? selLast = null;
        var selValidCount = 0;
        var selInvalidCount = 0;
        var selConflictCount = 0;

        if (!string.IsNullOrWhiteSpace(selectedBlockId))
        {
            var selSeats = seats.Where(s => s.BlockId == selectedBlockId).ToList();
            selValidCount = selSeats.Count(s => IsValidUserPreset(s.PresetNumber));
            selInvalidCount = selSeats.Count(s => s.PresetNumber != 0 && !IsValidUserPreset(s.PresetNumber));
            selConflictCount = selSeats.Count(s => conflictSeatIds.Contains(s.Id));

            var selValidPresets = selSeats.Where(s => IsValidUserPreset(s.PresetNumber)).Select(s => s.PresetNumber).Order().ToList();
            if (selValidPresets.Count > 0)
            {
                selFirst = selValidPresets.First();
                selLast = selValidPresets.Last();
            }
        }

        return new PresetDiagnostics
        {
            PresetStart = UserPresetStart,
            PresetEnd = UserPresetEnd,
            ValidAssignedSeatCount = validSeats.Count,
            InvalidSeatCount = invalidSeats.Count,
            ConflictSeatCount = conflictSeatIds.Count,
            ConflictPresetCount = duplicateGroups.Count,
            DistinctValidPresetCount = validSeats.Select(s => s.PresetNumber).Distinct().Count(),
            SelectedBlockFirstPreset = selFirst,
            SelectedBlockLastPreset = selLast,
            SelectedBlockValidSeatCount = selValidCount,
            SelectedBlockInvalidSeatCount = selInvalidCount,
            SelectedBlockConflictSeatCount = selConflictCount
        };
    }

    public bool SetSelectedBlockCameraType(CameraType cameraType)
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

        var block = _state.Layout.Blocks.FirstOrDefault(b => b.Id == _state.SelectedBlockId);
        if (block is null)
        {
            _state.StatusMessage = "Block nicht gefunden";
            return false;
        }

        block.CameraType = cameraType;
        _state.StatusMessage = cameraType == CameraType.AudienceV600
            ? "Block zugeordnet: Zuschauerkamera (V600)"
            : "Block zugeordnet: Bühnenkamera (SMTAV V60XL)";
        return true;
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
