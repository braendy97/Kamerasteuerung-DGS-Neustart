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
        WorkAreaScaleLabel.Text = scale >= 1.0
            ? "Darstellung: 1:1"
            : $"Darstellung skaliert: {scale * 100:0}%";
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
