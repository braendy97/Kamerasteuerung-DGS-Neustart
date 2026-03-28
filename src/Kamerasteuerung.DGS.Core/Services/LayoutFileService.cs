using System.Text.Json;
using System.Text.Json.Serialization;
using Kamerasteuerung.DGS.Core.Models;

namespace Kamerasteuerung.DGS.Core.Services;

public sealed class LayoutFileService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public LayoutDocument Load(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<LayoutDocument>(json, SerializerOptions)
               ?? throw new InvalidOperationException("Layoutdatei konnte nicht gelesen werden.");
    }

    public void Save(string path, LayoutDocument document)
    {
        document.LastUpdatedUtc = DateTime.UtcNow;

        var json = JsonSerializer.Serialize(document, SerializerOptions);
        var directory = Path.GetDirectoryName(path);

        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidOperationException("Ungültiger Speicherpfad.");
        }

        Directory.CreateDirectory(directory);

        var tempPath = path + ".tmp";
        var backupPath = path + ".bak";

        File.WriteAllText(tempPath, json);

        if (File.Exists(path))
        {
            File.Copy(path, backupPath, overwrite: true);
        }

        File.Move(tempPath, path, overwrite: true);
    }
}
