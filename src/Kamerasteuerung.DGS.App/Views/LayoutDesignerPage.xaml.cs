namespace Kamerasteuerung.DGS.App.Views;

public partial class LayoutDesignerPage : ContentPage
{
    private readonly Services.AppSessionState _state;
    private readonly Services.LayoutSessionService _layoutSession;
    private readonly IDispatcherTimer _timer;

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
        RenderBlocksAndSeats(scale);
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
                Name = b.Name,
                Details = $"Pos: {b.X:0},{b.Y:0} | Größe: {b.Width:0}×{b.Height:0}",
                SelectedMarker = b.Id == selectedId ? "Ausgewählt" : string.Empty
            })
            .ToList();

        BlocksCollectionView.ItemsSource = items;
        BlocksCollectionView.SelectedItem = items.FirstOrDefault(i => i.Id == selectedId);
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
            var border = new Border
            {
                Stroke = block.Id == selectedId ? Colors.Red : Colors.DarkSlateGray,
                StrokeThickness = block.Id == selectedId ? 3 : 1,
                BackgroundColor = Color.FromArgb("#CCFFFFFF"),
                Content = new Label
                {
                    Text = block.Name,
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
                    Stroke = Colors.DimGray,
                    StrokeThickness = 1,
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

    private void OnGenerateSeatsClicked(object sender, EventArgs e)
    {
        _layoutSession.GenerateSeatsForSelectedBlock(overwriteExisting: true);
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
        }
        else
        {
            _layoutSession.SelectBlock(null);
        }

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
