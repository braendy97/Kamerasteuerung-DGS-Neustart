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

    public AppSettings Load(string path)
    {
        if (!File.Exists(path))
        {
            return new AppSettings();
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<AppSettings>(json, SerializerOptions)
               ?? new AppSettings();
    }

    public void Save(string path, AppSettings settings)
    {
        var directory = Path.GetDirectoryName(path);

        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidOperationException("Ungültiger Settings-Pfad.");
        }

        Directory.CreateDirectory(directory);
        var json = JsonSerializer.Serialize(settings, SerializerOptions);
        File.WriteAllText(path, json);
    }
}
