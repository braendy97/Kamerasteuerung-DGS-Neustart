using Kamerasteuerung.DGS.App.Services;
using Kamerasteuerung.DGS.Core.Models;
using Kamerasteuerung.DGS.Core.Services;

namespace Kamerasteuerung.DGS.App.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsFileService _settingsFileService;
    private AppSettings _settings = new();

    public SettingsPage()
    {
        InitializeComponent();
        _settingsFileService = new SettingsFileService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _settings = await _settingsFileService.LoadAppSettingsAsync(AppPaths.SettingsFilePath);
        PopulateUiFromSettings(_settings);
    }

    private void PopulateUiFromSettings(AppSettings settings)
    {
        LayoutPathEntry.Text = settings.LastLayoutFilePath;

        V600HostEntry.Text = settings.V600Host;
        V600PortEntry.Text = settings.V600Port.ToString();

        SmtavHostEntry.Text = settings.SmtavV60XlHost;
        SmtavPortEntry.Text = settings.SmtavV60XlPort.ToString();
    }

    private void PopulateSettingsFromUi(AppSettings settings)
    {
        settings.LastLayoutFilePath = string.IsNullOrWhiteSpace(LayoutPathEntry.Text) ? null : LayoutPathEntry.Text.Trim();

        settings.V600Host = string.IsNullOrWhiteSpace(V600HostEntry.Text) ? null : V600HostEntry.Text.Trim();
        settings.V600Port = int.TryParse(V600PortEntry.Text, out var v600Port) ? v600Port : settings.V600Port;

        settings.SmtavV60XlHost = string.IsNullOrWhiteSpace(SmtavHostEntry.Text) ? null : SmtavHostEntry.Text.Trim();
        settings.SmtavV60XlPort = int.TryParse(SmtavPortEntry.Text, out var smtavPort) ? smtavPort : settings.SmtavV60XlPort;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            PopulateSettingsFromUi(_settings);
            await _settingsFileService.SaveAppSettingsAsync(AppPaths.SettingsFilePath, _settings);
            await DisplayAlert("Gespeichert", "Einstellungen wurden gespeichert.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fehler", ex.Message, "OK");
        }
    }
}
