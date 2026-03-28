namespace Kamerasteuerung.DGS.App.Views;

public partial class HomePage : ContentPage
{
    private readonly Services.AppSessionState _state;
    private readonly Services.LayoutSessionService _layoutSession;
    private readonly IDispatcherTimer _timer;

    public HomePage(Services.AppSessionState state, Services.LayoutSessionService layoutSession)
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
        StartupStatusLabel.Text = _state.StatusMessage;
        LayoutNameLabel.Text = _state.Layout is null ? "Kein Layout geladen" : $"Aktuelles Layout: {_state.Layout.Name}";
        LayoutPathLabel.Text = string.IsNullOrWhiteSpace(_state.Settings.LastLayoutFilePath)
            ? string.Empty
            : $"Pfad: {_state.Settings.LastLayoutFilePath}";
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
