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

## [0.8.0] - 2026-03-28
### Hinzugefügt
- Erste feste Layout-Blöcke im Designer:
  - Block hinzufügen (Default-Position/Größe, automatische Namen)
  - Blockliste mit Auswahl
  - einfache Darstellung als Rechtecke im Arbeitsfeld (skalierungs-respektierend)
  - optionales Löschen des ausgewählten Blocks

## [0.9.0] - 2026-03-28
### Hinzugefügt
- Sitzraster innerhalb eines Blocks:
  - Sitze für ausgewählten Block generieren (überschreibt bestehende Sitze des Blocks)
  - Sitze werden im Arbeitsfeld innerhalb der Blockfläche sichtbar angezeigt
  - Sitzanzahl: ausgewählter Block + Gesamt

## [0.10.0] - 2026-03-28
### Hinzugefügt
- Block-Kameraart im Layout-Designer:
  - `LayoutBlock.CameraType` (nullable) zur fachlichen Unterscheidung Bühne vs. Zuschauer
  - UI-Buttons zur Zuordnung am ausgewählten Block (Zuschauerkamera V600 / Bühnenkamera SMTAV V60XL)
  - visuelle Kennzeichnung von Blöcken nach Kameraart (Label + dezente Hintergrundfarbe)
- Preset-Vorbereitung beim Sitzgenerieren:
  - `PresetNumber` wird beim Generieren fortlaufend im Nutzerbereich 10–210 gesetzt (außerhalb: 0)

## [0.11.0] - 2026-03-28
### Hinzugefügt
- Zentrale Preset-Vergabelogik im `LayoutSessionService`:
  - stabil/reproduzierbar über Block- und Sitzreihenfolge (Blocks: Y/X/Name; Seats: SortOrder/Y/X/Label)
  - gültiger Nutzerbereich fest: 10–210 (`0` = nicht zugewiesen)
  - manuelle Aktion „Presets neu berechnen“ im Designer
- Preset-Diagnostik (ohne VISCA):
  - globale Zusammenfassung (gültige Belegung, ungültige Werte, Konflikte/Dubletten)
  - Block-bezogene Zusammenfassung (Bereich/Counts)

### Geändert
- Sitzgenerierung delegiert Presetvergabe an die zentrale Neuvergabe (keine lokale Nebenlogik mehr)

## [0.12.0] - 2026-03-28
### Hinzugefügt
- Manuelle Preset-Korrektur im Designer:
  - Sitzliste für den ausgewählten Block (zeigt Label + Preset; Konfliktmarker bei Dubletten/ungültigen Presets)
  - Blockweise Neubelegung ab Startnummer (nur ausgewählter Block; Überlauf → 0)
  - manuelles Setzen eines einzelnen Sitz-Presets (akzeptiert 0 oder 10–210; keine automatische Konfliktbereinigung)
- Minimaler Sitzauswahl-State in `AppSessionState` (`SelectedSeatId`)

## [0.13.0] - 2026-03-28
### Hinzugefügt
- Block-Priorität für Preset-Neuvergabe:
  - `LayoutBlock.PresetPriority` (int, Default 100; niedrigere Zahl = früher)
  - globale Preset-Neuvergabe sortiert Blöcke primär nach `PresetPriority` (Fallback: Y/X/Name)
- Designer: effektive Vergabereihenfolge sichtbar (Liste + Blockdetails inkl. Prio und Preset-Range)
- Setzen der Priorität am ausgewählten Block (ohne automatische Neuvergabe)

## [0.14.0] - 2026-03-28
### Hinzugefügt
- Exportierbarer Preset-Status (ohne VISCA):
  - zentrale Erzeugung einer Preset-Übersicht aus dem aktuellen Layout (Blocks + Seats inkl. Status: gültig/ungültig/Konflikt)
  - sortiert nach effektiver Vergabereihenfolge (Block: Priorität/Y/X/Name, Sitze: SortOrder/Y/X/Label)
- Designer: sachliche Preset-Übersicht (Liste + Textvorschau) + Button zum Aktualisieren

## [0.15.0] - 2026-03-28
### Hinzugefügt
- VISCA-Core (ohne Netzwerk):
  - kleine Grundtypen für Preset-Aktionen und Build-Result (`ViscaPresetAction`, `ViscaCommandBuildResult`, `ViscaPresetCommandRequest`)
  - zentrale Erzeugung von VISCA Preset-Befehlen aus Kameraprofil + Aktion + Presetnummer (`ViscaPresetCommandService`)
  - Hex-Ausgabehilfe für Debug (`ViscaHex.ToHexString`)

## [0.16.0] - 2026-03-28
### Hinzugefügt
- VISCA-Core TCP-Transport (ohne UI):
  - `ViscaTcpTransportService` (TcpClient, connect/write/read mit Timeouts)
  - `ViscaTransportOptions` (Connect/Write/Read-Timeouts, Response an/aus, MaxResponseBytes)
  - `ViscaTransportResult` (Success/Failure + Hex-Diagnose für Send/Response)
  - kombinierter Sendepfad `ViscaPresetCommandSender.SendPresetCommandAsync(...)` (Befehl bauen + senden + optionale Antwort)

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
