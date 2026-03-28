namespace Kamerasteuerung.DGS.App.Views;

public partial class HomePage : ContentPage
{
    private readonly Services.AppSessionState _state;
    private readonly IDispatcherTimer _timer;

    public HomePage(Services.AppSessionState state)
    {
        InitializeComponent();
        _state = state;

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
    }
}
