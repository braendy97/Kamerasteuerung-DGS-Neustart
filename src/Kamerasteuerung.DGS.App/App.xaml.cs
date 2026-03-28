namespace Kamerasteuerung.DGS.App;

public partial class App : Application
{
    private readonly Services.AppStartupService _startupService;

    public App(Services.AppStartupService startupService)
    {
        InitializeComponent();
        _startupService = startupService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        _ = Task.Run(() => _startupService.InitializeAsync());
        return new Window(new AppShell());
    }
}
