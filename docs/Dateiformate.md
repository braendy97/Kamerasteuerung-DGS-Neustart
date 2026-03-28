# Dateiformate

## Layoutdatei
Empfohlene Dateiendung:
- `.dgslayout.json`

Rolle:
- Externe, frei speicherbare Datei für ein konkretes Layout.
- Kann später zwischen Geräten geteilt werden (Mehrgeräte-Konflikte werden erst später behandelt).

Inhalt (Wurzelmodell: `LayoutDocument`):
- `Id`
- `DocumentVersion`
- `Name`
- `LastUpdatedUtc`
- `Canvas` (`Width`, `Height`, `Zoom`)
- `Cameras`
- `Blocks`
- `Seats`
- `Participants`

## Settings-Datei
Rolle:
- Lokale App-Datei (gerät-/nutzerbezogen), nicht zum Teilen gedacht.
- Merkt sich u. a. den zuletzt verwendeten Layout-Pfad.

Dateiname (aktuell in MAUI):
- `appsettings.json` (unter AppData)

Inhalt (Wurzelmodell: `AppSettings`):
- `LastLayoutFilePath` (Pfad zur zuletzt verwendeten Layoutdatei; später Grundlage für Auto-Load beim Start)
- optional vorbereitend: `LayoutsFolderPath`
- Kamera-Defaults:
  - `V600Host` / `V600Port`
  - `SmtavV60XlHost` / `SmtavV60XlPort`

## Speichersicherheit
Beim Speichern:
1. temporäre Datei schreiben
2. alte Datei sichern
3. neue Datei atomar ersetzen
