using System.Text.Json;
using System.Text.Json.Serialization;
using Kamerasteuerung.DGS.Core.Models;

namespace Kamerasteuerung.DGS.Core.Services;

public sealed class SettingsFileService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public AppSettings Load(string path) => LoadAppSettingsAsync(path).GetAwaiter().GetResult();

    public void Save(string path, AppSettings settings) => SaveAppSettingsAsync(path, settings).GetAwaiter().GetResult();

    public async Task<AppSettings> LoadAppSettingsAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(path))
        {
            return new AppSettings();
        }

        try
        {
            var json = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<AppSettings>(json, SerializerOptions)
                   ?? new AppSettings();
        }
        catch (JsonException)
        {
            return new AppSettings();
        }
    }

    public async Task SaveAppSettingsAsync(string path, AppSettings settings, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(path);

        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidOperationException("Ungültiger Settings-Pfad.");
        }

        Directory.CreateDirectory(directory);
        var json = JsonSerializer.Serialize(settings, SerializerOptions);
        await File.WriteAllTextAsync(path, json, System.Text.Encoding.UTF8, cancellationToken).ConfigureAwait(false);
    }
}
