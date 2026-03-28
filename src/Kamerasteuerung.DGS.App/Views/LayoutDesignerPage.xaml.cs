using Kamerasteuerung.DGS.Core.Models;

namespace Kamerasteuerung.DGS.App.Views;

public partial class LayoutDesignerPage : ContentPage
{
    private readonly Services.AppSessionState _state;
    private readonly Services.LayoutSessionService _layoutSession;
    private readonly IDispatcherTimer _timer;

    private const string ConflictMarker = "⚠";
    private const string ValidMarker = "✓";

    public LayoutDesignerPage(Services.AppSessionState state, Services.LayoutSessionService layoutSession)
    {
        InitializeComponent();
        _state = state;
        _layoutSession = layoutSession;

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(250);
        _timer.Tick += (_, _) => Refresh();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Refresh();
        _timer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timer.Stop();
    }

    private void Refresh()
    {
        StatusLabel.Text = _state.StatusMessage;
        LayoutNameLabel.Text = _state.Layout is null ? "Kein Layout geladen" : $"Name: {_state.Layout.Name}";
        LayoutPathLabel.Text = string.IsNullOrWhiteSpace(_state.Settings.LastLayoutFilePath)
            ? string.Empty
            : $"Pfad: {_state.Settings.LastLayoutFilePath}";

        NoLayoutFrame.IsVisible = _state.Layout is null;
        LayoutSummaryFrame.IsVisible = _state.Layout is not null;
        BlocksFrame.IsVisible = _state.Layout is not null;
        WorkAreaFrame.IsVisible = _state.Layout is not null;

        if (_state.Layout is null)
        {
            return;
        }

        var canvasWidth = _state.Layout.Canvas.Width;
        var canvasHeight = _state.Layout.Canvas.Height;

        CanvasSizeLabel.Text = $"Canvas: {canvasWidth:0} × {canvasHeight:0}";
        CountsLabel.Text = $"Blöcke: {_state.Layout.Blocks.Count} | Sitze: {_state.Layout.Seats.Count} | Kameras: {_state.Layout.Cameras.Count} | Teilnehmer: {_state.Layout.Participants.Count}";

        var (displayWidth, displayHeight, scale) = ComputeWorkAreaSize(canvasWidth, canvasHeight);
        WorkAreaBorder.WidthRequest = displayWidth;
        WorkAreaBorder.HeightRequest = displayHeight;
        BlocksOverlay.WidthRequest = displayWidth;
        BlocksOverlay.HeightRequest = displayHeight;
        WorkAreaScaleLabel.Text = scale >= 1.0
            ? "Darstellung: 1:1"
            : $"Darstellung skaliert: {scale * 100:0}%";

        RefreshBlocksList(scale);
        RefreshSeatsSummary();
        RefreshPresetSummary();
        RefreshAssignmentOrder();
        RefreshPresetOverview();
        RefreshSelectedBlockCameraType();
        RefreshSelectedBlockPresetPriority();
        RefreshSeatsList();
        RefreshSelectedSeat();
        RenderBlocksAndSeats(scale);
    }

    private sealed class PresetOverviewListItem
    {
        public required string Block { get; init; }
        public required string Seat { get; init; }
        public required string Preset { get; init; }
        public required string Status { get; init; }
    }

    private void RefreshPresetOverview()
    {
        if (_state.Layout is null)
        {
            PresetOverviewSummaryLabel.Text = string.Empty;
            PresetOverviewCollectionView.ItemsSource = null;
            PresetOverviewTextEditor.Text = string.Empty;
            return;
        }

        var overview = _layoutSession.BuildPresetOverview(setAsCurrent: true);
        if (overview is null)
        {
            PresetOverviewSummaryLabel.Text = "(keine Daten)";
            PresetOverviewCollectionView.ItemsSource = null;
            PresetOverviewTextEditor.Text = string.Empty;
            return;
        }

        var items = overview.Seats
            .Select(s => new PresetOverviewListItem
            {
                Block = s.BlockName,
                Seat = s.SeatLabel,
                Preset = s.PresetNumber.ToString(),
                Status = s.Status switch
                {
                    Services.PresetSeatStatus.Valid => ValidMarker,
                    Services.PresetSeatStatus.Conflict => ConflictMarker,
                    Services.PresetSeatStatus.Invalid => "ungültig",
                    _ => ""
                }
            })
            .ToList();

        PresetOverviewCollectionView.ItemsSource = items;

        var validCount = overview.Seats.Count(s => s.Status == Services.PresetSeatStatus.Valid);
        var conflictCount = overview.Seats.Count(s => s.Status == Services.PresetSeatStatus.Conflict);
        var invalidCount = overview.Seats.Count(s => s.Status == Services.PresetSeatStatus.Invalid);
        var noneCount = overview.Seats.Count(s => s.Status == Services.PresetSeatStatus.None);

        PresetOverviewSummaryLabel.Text = $"Sitze: {overview.Seats.Count} | gültig: {validCount} | Konflikt: {conflictCount} | ungültig: {invalidCount} | 0/leer: {noneCount}";
        PresetOverviewTextEditor.Text = _layoutSession.PresetOverviewText;
    }

    private void RefreshAssignmentOrder()
    {
        if (_state.Layout is null)
        {
            AssignmentOrderLabel.Text = string.Empty;
            return;
        }

        static bool IsValidUserPreset(int p) => p >= Services.LayoutSessionService.UserPresetStart && p <= Services.LayoutSessionService.UserPresetEnd;

        var blocks = _state.Layout.Blocks
            .OrderBy(b => b.PresetPriority)
            .ThenBy(b => b.Y)
            .ThenBy(b => b.X)
            .ThenBy(b => b.Name)
            .Select(b =>
            {
                var presets = _state.Layout.Seats
                    .Where(s => s.BlockId == b.Id && IsValidUserPreset(s.PresetNumber))
                    .Select(s => s.PresetNumber)
                    .Order()
                    .ToList();

                var range = presets.Count == 0 ? "–" : $"{presets.First()}–{presets.Last()}";
                var cam = b.CameraType switch
                {
                    Core.Models.CameraType.AudienceV600 => "Zuschauer",
                    Core.Models.CameraType.StageSmtavV60XL => "Bühne",
                    _ => "(keine)"
                };

                return $"{b.PresetPriority}: {b.Name} [{cam}] ({range})";
            })
            .ToList();

        AssignmentOrderLabel.Text = blocks.Count == 0
            ? "(keine Blöcke)"
            : string.Join("\n", blocks);
    }

    private void RefreshSelectedBlockPresetPriority()
    {
        if (_state.Layout is null || string.IsNullOrWhiteSpace(_state.SelectedBlockId))
        {
            SelectedBlockPresetPriorityLabel.Text = "Kein Block ausgewählt";
            return;
        }

        var block = _state.Layout.Blocks.FirstOrDefault(b => b.Id == _state.SelectedBlockId);
        if (block is null)
        {
            SelectedBlockPresetPriorityLabel.Text = "Block nicht gefunden";
            return;
        }

        SelectedBlockPresetPriorityLabel.Text = $"Aktuell: {block.PresetPriority} (niedriger = früher)";
    }

    private sealed class SeatListItem
    {
        public required string Id { get; init; }
        public required string Name { get; init; }
        public required string Details { get; init; }
        public required string Marker { get; init; }
    }

    private void RefreshSeatsList()
    {
        if (_state.Layout is null)
        {
            NoBlockSelectedFrame.IsVisible = false;
            SeatsCollectionView.ItemsSource = null;
            return;
        }

        if (string.IsNullOrWhiteSpace(_state.SelectedBlockId))
        {
            NoBlockSelectedFrame.IsVisible = true;
            SeatsCollectionView.ItemsSource = null;
            return;
        }

        NoBlockSelectedFrame.IsVisible = false;

        var diag = _layoutSession.GetPresetDiagnostics(_state.SelectedBlockId);
        var conflictSeatIds = GetConflictSeatIds();

        var items = _state.Layout.Seats
            .Where(s => s.BlockId == _state.SelectedBlockId)
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.Y)
            .ThenBy(s => s.X)
            .ThenBy(s => s.Label)
            .Select(s => new SeatListItem
            {
                Id = s.Id,
                Name = string.IsNullOrWhiteSpace(s.Label) ? s.Id : s.Label,
                Details = $"Preset: {s.PresetNumber}",
                Marker = conflictSeatIds.Contains(s.Id)
                    ? ConflictMarker
                    : (s.PresetNumber == 0 ? "" : (s.PresetNumber >= Services.LayoutSessionService.UserPresetStart && s.PresetNumber <= Services.LayoutSessionService.UserPresetEnd ? "" : ConflictMarker))
            })
            .ToList();

        SeatsCollectionView.ItemsSource = items;
        SeatsCollectionView.SelectedItem = items.FirstOrDefault(i => i.Id == _state.SelectedSeatId);
    }

    private void RefreshSelectedSeat()
    {
        if (_state.Layout is null || string.IsNullOrWhiteSpace(_state.SelectedSeatId))
        {
            SelectedSeatLabel.Text = "Ausgewählter Sitz: (keiner)";
            return;
        }

        var seat = _state.Layout.Seats.FirstOrDefault(s => s.Id == _state.SelectedSeatId);
        if (seat is null)
        {
            SelectedSeatLabel.Text = "Ausgewählter Sitz: (nicht gefunden)";
            return;
        }

        SelectedSeatLabel.Text = $"Ausgewählter Sitz: {seat.Label} | Preset: {seat.PresetNumber}";
    }

    private HashSet<string> GetConflictSeatIds()
    {
        if (_state.Layout is null)
        {
            return [];
        }

        static bool IsValidUserPreset(int preset) => preset >= Services.LayoutSessionService.UserPresetStart && preset <= Services.LayoutSessionService.UserPresetEnd;

        var validSeats = _state.Layout.Seats.Where(s => IsValidUserPreset(s.PresetNumber)).ToList();
        return validSeats
            .GroupBy(s => s.PresetNumber)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g.Select(s => s.Id))
            .ToHashSet();
    }

    private void RefreshPresetSummary()
    {
        if (_state.Layout is null)
        {
            PresetRangeLabel.Text = string.Empty;
            PresetSummaryLabel.Text = string.Empty;
            SelectedBlockPresetSummaryLabel.Text = string.Empty;
            return;
        }

        var diag = _layoutSession.GetPresetDiagnostics(_state.SelectedBlockId);
        PresetRangeLabel.Text = $"Nutzerbereich: {diag.PresetStart}–{diag.PresetEnd} (0 = nicht zugewiesen)";

        PresetSummaryLabel.Text = $"Gültig belegt: {diag.DistinctValidPresetCount} Presets / {diag.ValidAssignedSeatCount} Sitze | Ungültig: {diag.InvalidSeatCount} | Konflikte: {diag.ConflictPresetCount} Presets ({diag.ConflictSeatCount} Sitze)";

        if (string.IsNullOrWhiteSpace(_state.SelectedBlockId))
        {
            SelectedBlockPresetSummaryLabel.Text = "Ausgewählter Block: (keiner)";
            return;
        }

        var rangeText = diag.SelectedBlockFirstPreset is null
            ? "Presetbereich: (keine gültigen Presets)"
            : $"Presetbereich: {diag.SelectedBlockFirstPreset}–{diag.SelectedBlockLastPreset}";

        var flags = new List<string>();
        if (diag.SelectedBlockInvalidSeatCount > 0)
        {
            flags.Add($"ungültig: {diag.SelectedBlockInvalidSeatCount}");
        }
        if (diag.SelectedBlockConflictSeatCount > 0)
        {
            flags.Add($"Konflikte: {diag.SelectedBlockConflictSeatCount}");
        }
        var flagsText = flags.Count == 0 ? string.Empty : " | " + string.Join(" | ", flags);

        SelectedBlockPresetSummaryLabel.Text = $"Ausgewählter Block: {rangeText} | gültige Sitze: {diag.SelectedBlockValidSeatCount}{flagsText}";
    }

    private void RefreshSelectedBlockCameraType()
    {
        if (_state.Layout is null || string.IsNullOrWhiteSpace(_state.SelectedBlockId))
        {
            SelectedBlockCameraTypeLabel.Text = "Kein Block ausgewählt";
            return;
        }

        var block = _state.Layout.Blocks.FirstOrDefault(b => b.Id == _state.SelectedBlockId);
        if (block is null)
        {
            SelectedBlockCameraTypeLabel.Text = "Block nicht gefunden";
            return;
        }

        SelectedBlockCameraTypeLabel.Text = block.CameraType switch
        {
            Core.Models.CameraType.AudienceV600 => "Aktuell: Zuschauerkamera (V600)",
            Core.Models.CameraType.StageSmtavV60XL => "Aktuell: Bühnenkamera (SMTAV V60XL)",
            _ => "Aktuell: (keine Kameraart zugeordnet)"
        };
    }

    private void RefreshSeatsSummary()
    {
        if (_state.Layout is null)
        {
            SeatsSummaryLabel.Text = string.Empty;
            return;
        }

        var total = _state.Layout.Seats.Count;
        if (string.IsNullOrWhiteSpace(_state.SelectedBlockId))
        {
            SeatsSummaryLabel.Text = $"Sitze gesamt: {total}";
            return;
        }

        var inBlock = _state.Layout.Seats.Count(s => s.BlockId == _state.SelectedBlockId);
        SeatsSummaryLabel.Text = $"Sitze im ausgewählten Block: {inBlock} | Sitze gesamt: {total}";
    }

    private sealed class BlockListItem
    {
        public required string Id { get; init; }
        public required string Name { get; init; }
        public required string Details { get; init; }
        public required string SelectedMarker { get; init; }
    }

    private void RefreshBlocksList(double scale)
    {
        if (_state.Layout is null)
        {
            BlocksCollectionView.ItemsSource = null;
            return;
        }

        var selectedId = _state.SelectedBlockId;
        var items = _state.Layout.Blocks
            .Select(b => new BlockListItem
            {
                Id = b.Id,
                Name = b.CameraType switch
                {
                    Core.Models.CameraType.AudienceV600 => $"{b.Name} (Zuschauer)",
                    Core.Models.CameraType.StageSmtavV60XL => $"{b.Name} (Bühne)",
                    _ => b.Name
                },
                Details = BuildBlockDetails(b),
                SelectedMarker = b.Id == selectedId ? "Ausgewählt" : string.Empty
            })
            .ToList();

        BlocksCollectionView.ItemsSource = items;
        BlocksCollectionView.SelectedItem = items.FirstOrDefault(i => i.Id == selectedId);
    }

    private string BuildBlockDetails(LayoutBlock b)
    {
        if (_state.Layout is null)
        {
            return $"Pos: {b.X:0},{b.Y:0} | Größe: {b.Width:0}×{b.Height:0}";
        }

        static bool IsValidUserPreset(int p) => p >= Services.LayoutSessionService.UserPresetStart && p <= Services.LayoutSessionService.UserPresetEnd;

        var presets = _state.Layout.Seats
            .Where(s => s.BlockId == b.Id && IsValidUserPreset(s.PresetNumber))
            .Select(s => s.PresetNumber)
            .Order()
            .ToList();

        var range = presets.Count == 0 ? "–" : $"{presets.First()}–{presets.Last()}";
        return $"Prio: {b.PresetPriority} | Presets: {range} | Pos: {b.X:0},{b.Y:0} | Größe: {b.Width:0}×{b.Height:0}";
    }

    private void RenderBlocksAndSeats(double scale)
    {
        BlocksOverlay.Children.Clear();

        if (_state.Layout is null)
        {
            return;
        }

        var selectedId = _state.SelectedBlockId;

        foreach (var block in _state.Layout.Blocks)
        {
            var background = block.CameraType switch
            {
                Core.Models.CameraType.StageSmtavV60XL => Color.FromArgb("#CCF3E5F5"),
                Core.Models.CameraType.AudienceV600 => Color.FromArgb("#CCE8F5E9"),
                _ => Color.FromArgb("#CCFFFFFF")
            };

            var border = new Border
            {
                Stroke = block.Id == selectedId ? Colors.Red : Colors.DarkSlateGray,
                StrokeThickness = block.Id == selectedId ? 3 : 1,
                BackgroundColor = background,
                Content = new Label
                {
                    Text = block.CameraType switch
                    {
                        Core.Models.CameraType.AudienceV600 => $"{block.Name}\n(Zuschauer)",
                        Core.Models.CameraType.StageSmtavV60XL => $"{block.Name}\n(Bühne)",
                        _ => block.Name
                    },
                    FontSize = 12,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    LineBreakMode = LineBreakMode.TailTruncation
                }
            };

            var x = block.X * scale;
            var y = block.Y * scale;
            var w = Math.Max(20, block.Width * scale);
            var h = Math.Max(20, block.Height * scale);

            AbsoluteLayout.SetLayoutBounds(border, new Rect(x, y, w, h));
            BlocksOverlay.Children.Add(border);

            foreach (var seat in _state.Layout.Seats.Where(s => s.BlockId == block.Id))
            {
                var seatBorder = new Border
                {
                    Stroke = seat.Id == _state.SelectedSeatId ? Colors.OrangeRed : Colors.DimGray,
                    StrokeThickness = seat.Id == _state.SelectedSeatId ? 2 : 1,
                    BackgroundColor = Color.FromArgb("#B3FFFDE7")
                };

                var sx = seat.X * scale;
                var sy = seat.Y * scale;
                var sw = Math.Max(8, seat.Width * scale);
                var sh = Math.Max(8, seat.Height * scale);

                AbsoluteLayout.SetLayoutBounds(seatBorder, new Rect(sx, sy, sw, sh));
                BlocksOverlay.Children.Add(seatBorder);
            }
        }
    }

    private static (double width, double height, double scale) ComputeWorkAreaSize(double canvasWidth, double canvasHeight)
    {
        const double maxSize = 800;
        if (canvasWidth <= 0 || canvasHeight <= 0)
        {
            return (400, 260, 0);
        }

        var scale = Math.Min(1.0, maxSize / Math.Max(canvasWidth, canvasHeight));
        return (Math.Max(50, canvasWidth * scale), Math.Max(50, canvasHeight * scale), scale);
    }

    private void OnNewLayoutClicked(object sender, EventArgs e)
    {
        _layoutSession.CreateNewLayout();
        Refresh();
    }

    private void OnSetAudienceCameraClicked(object sender, EventArgs e)
    {
        _layoutSession.SetSelectedBlockCameraType(Core.Models.CameraType.AudienceV600);
        Refresh();
    }

    private void OnSetStageCameraClicked(object sender, EventArgs e)
    {
        _layoutSession.SetSelectedBlockCameraType(Core.Models.CameraType.StageSmtavV60XL);
        Refresh();
    }

    private void OnGenerateSeatsClicked(object sender, EventArgs e)
    {
        _layoutSession.GenerateSeatsForSelectedBlock(overwriteExisting: true);
        Refresh();
    }

    private void OnReassignPresetsClicked(object sender, EventArgs e)
    {
        _layoutSession.ReassignUserPresets();
        Refresh();
    }

    private void OnRefreshPresetOverviewClicked(object sender, EventArgs e)
    {
        _layoutSession.BuildPresetOverview(setAsCurrent: true);
        _state.StatusMessage = "Preset-Übersicht aktualisiert";
        Refresh();
    }

    private void OnAddBlockClicked(object sender, EventArgs e)
    {
        _layoutSession.AddBlock();
        Refresh();
    }

    private void OnDeleteBlockClicked(object sender, EventArgs e)
    {
        _layoutSession.DeleteSelectedBlock();
        Refresh();
    }

    private void OnBlockSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is BlockListItem item)
        {
            _layoutSession.SelectBlock(item.Id);
            _state.SelectedSeatId = null;
        }
        else
        {
            _layoutSession.SelectBlock(null);
            _state.SelectedSeatId = null;
        }

        Refresh();
    }

    private void OnSeatSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is SeatListItem item)
        {
            _state.SelectedSeatId = item.Id;
        }
        else
        {
            _state.SelectedSeatId = null;
        }

        Refresh();
    }

    private void OnSetBlockPresetsClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(BlockPresetStartEntry.Text, out var startPreset))
        {
            _state.StatusMessage = "Start-Preset ungültig";
            Refresh();
            return;
        }

        _layoutSession.SetSelectedBlockPresetsFromStart(startPreset);
        Refresh();
    }

    private void OnSetSeatPresetClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_state.SelectedSeatId))
        {
            _state.StatusMessage = "Kein Sitz ausgewählt";
            Refresh();
            return;
        }

        if (!int.TryParse(SeatPresetEntry.Text, out var preset))
        {
            _state.StatusMessage = "Sitz-Preset ungültig";
            Refresh();
            return;
        }

        _layoutSession.SetSeatPreset(_state.SelectedSeatId, preset);
        Refresh();
    }

    private void OnSetBlockPresetPriorityClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(BlockPresetPriorityEntry.Text, out var prio))
        {
            _state.StatusMessage = "Preset-Priorität ungültig";
            Refresh();
            return;
        }

        _layoutSession.SetSelectedBlockPresetPriority(prio);
        Refresh();
    }

    private async void OnOpenLayoutClicked(object sender, EventArgs e)
    {
        await _layoutSession.OpenLayoutAsync();
        Refresh();
    }

    private async void OnSaveLayoutClicked(object sender, EventArgs e)
    {
        await _layoutSession.SaveLayoutAsync();
        Refresh();
    }
}
