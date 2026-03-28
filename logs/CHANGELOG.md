# Änderungslog

## [0.4.0] - 2026-03-28
### Hinzugefügt
- JSON-Persistenzgrundlage im Core: Async Load/Save für Layout und App-Settings (System.Text.Json, UTF-8, WriteIndented)
- MAUI: `SettingsPage` kann Einstellungen laden/speichern (app-local `appsettings.json` in AppData)

### Geändert
- `AppSettings` erweitert (LastLayoutFilePath, LayoutsFolderPath, V600/SMTAV Host+Port) und kompatible Alias-Properties beibehalten
- `LayoutDocument` um minimale Metadaten ergänzt (`Id`) und Alias-Properties für die Doku ergänzt

## [0.5.0] - 2026-03-28
### Hinzugefügt
- App-Startfluss: Settings laden und optionales Auto-Load des letzten Layouts über `LastLayoutFilePath`
- Minimaler App-Zustand (`AppSessionState`) für Settings/Layout/Status
- Startseite zeigt Lade-/Fehlerstatus sowie Layoutname sichtbar an

## [0.6.0] - 2026-03-28
### Hinzugefügt
- Manuelle Layout-Dateiverwaltung in der UI: Neues Layout / Öffnen / Speichern
- Nach Öffnen/Speichern wird `LastLayoutFilePath` aktualisiert und in `appsettings.json` persistiert

## [0.7.0] - 2026-03-28
### Hinzugefügt
- `LayoutDesignerPage` als echter Designer-Grundzustand:
  - sichtbare Arbeitsfläche mit Rand (größe aus `LayoutDocument.Canvas` abgeleitet, ggf. skaliert)
  - leerer Zustand vs. geladener Zustand klar unterscheidbar
  - sachliche Layout-Zusammenfassung (Name, Größe, Counts)

## [0.3.0] - 2026-03-28
### Hinzugefügt
- `Models/CameraProfile.cs` – Kamera-Konfigurationsmodell mit Name, Host, Port, PresetStart, PresetEnd, SupportsTracking, CameraType

### Geändert
- `LayoutDocument.cs` – referenziert jetzt `CameraProfile` statt `CameraDefinition`

### Entfernt
- `Models/CameraDefinition.cs` – durch `Models/CameraProfile.cs` ersetzt
- veraltete Root-Duplikate (`Arbeitsplan.md`, `CHANGELOG.md`, `FORTSCHRITT_AUSFUEHRLICH.md`)

### Sonstiges
- Erster vollständiger Commit mit gesamter Projektbasis ins Remote-Repository

## [0.2.0] - 2026-03-28
### Hinzugefügt
- `Models/CameraType.cs` (Enum: AudienceV600, StageSmtavV60XL)
- `Views/LiveControlPage.xaml` mit Platzhalter-UI
- `Views/ParticipantsPage.xaml` mit Platzhalter-UI
- `Platforms/Windows/App.xaml(.cs)` – WinUI-Entry-Point
- `Platforms/Android/MainActivity.cs`, `MainApplication.cs`, `AndroidManifest.xml`

### Geändert
- `AppShell.xaml` – Navigation auf 5 Tabs erweitert (Start, Layout, Live, Teilnehmer, Einstellungen)
- `MauiProgram.cs` – alle 5 Seiten registriert
- `SettingsPage.xaml` – echte Eingabefelder für Layout-Pfad, V600 Host/Port, SMTAV V60XL Host/Port
- `HomePage.xaml` – XML-Entity-Fehler behoben (`&` → `&amp;`)
- `App.xaml.cs` – `MainPage`-Setter durch `CreateWindow`-Override ersetzt (.NET 10)
- `Kamerasteuerung.DGS.App.csproj` – `Microsoft.Maui.Controls` PackageReference ergänzt (MA002)

### Behoben
- MAUI-App war nicht buildfähig (fehlende PackageReference, fehlende Plattform-Entry-Points, XML-Entity-Fehler, veraltete MainPage-API)

## [0.1.0] - 2026-03-28
### Hinzugefügt
- neues Projektgrundgerüst für Kamerasteuerung DGS
- Shared Core für Layout-, Teilnehmer- und Kamera-Grundstruktur
- erstes MAUI-App-Grundgerüst
- Arbeitsplan und Architekturdokumente
- Beispiel-Layoutdatei und Beispiel-Settings
- Kamera-Profile für V600 und SMTAV V60XL als Grundlage dokumentiert

### Geändert
- keine fachlichen Altdateien übernommen
- Altprojekt wird nur noch als Referenz für Steuerlogik betrachtet

### Behoben
- nicht zutreffend

### Entfernt
- feste Altstruktur mit starren Buttonannahmen wird nicht fortgeführt
