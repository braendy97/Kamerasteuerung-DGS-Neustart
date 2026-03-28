# Änderungslog

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
