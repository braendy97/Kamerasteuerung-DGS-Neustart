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

    public LayoutDocument Load(string path) => LoadLayoutAsync(path).GetAwaiter().GetResult();

    public void Save(string path, LayoutDocument document) => SaveLayoutAsync(path, document).GetAwaiter().GetResult();

    public async Task<LayoutDocument> LoadLayoutAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<LayoutDocument>(json, SerializerOptions)
                   ?? throw new InvalidOperationException("Layoutdatei konnte nicht gelesen werden.");
        }
        catch (FileNotFoundException ex)
        {
            throw new InvalidOperationException("Layoutdatei wurde nicht gefunden.", ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Layoutdatei ist kein gültiges JSON.", ex);
        }
    }

    public async Task SaveLayoutAsync(string path, LayoutDocument document, CancellationToken cancellationToken = default)
    {
        document.LastUpdatedUtc = DateTime.UtcNow;

        var directory = Path.GetDirectoryName(path);

        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidOperationException("Ungültiger Speicherpfad.");
        }

        Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(document, SerializerOptions);
        var tempPath = path + ".tmp";
        var backupPath = path + ".bak";

        await File.WriteAllTextAsync(tempPath, json, System.Text.Encoding.UTF8, cancellationToken).ConfigureAwait(false);

        if (File.Exists(path))
        {
            File.Copy(path, backupPath, overwrite: true);
        }

        File.Move(tempPath, path, overwrite: true);
    }
}
