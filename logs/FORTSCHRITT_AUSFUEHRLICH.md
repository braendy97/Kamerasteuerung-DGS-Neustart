# Fortschrittslog ausführlich

## 2026-03-28 – Projektstart Neubau
### Ausgangslage
- Altprojekt ist eine WinForms/VB.NET-Anwendung mit starker UI-Kopplung.
- Ziel ist eine flexible Layout-App für DGS-Meetings.
- Sitzpläne sollen nicht mehr über starre Buttons abgebildet werden.

### Fachliche Grundentscheidungen
- Neubau statt harter Weiterentwicklung der Alt-App
- MAUI als erster produktiver UI-Client
- Shared Core für spätere Wiederverwendung
- Layoutdatei frei wählbar speicherbar und wieder ladbar
- automatisches Laden beim Start über gespeicherten Pfad
- Mehrgeräte-Nutzung durch gemeinsame Layoutdatei berücksichtigen

### Kamerabezogene Entscheidungen
- Zuschauerkamera: V600
- Bühnenkamera: SMTAV V60XL
- gemeinsamer Nutzerbereich für Presets: 10–210
- kameraabhängige Kommunikationsprofile vorsehen
- SMTAV-spezifische Tracking-Funktionen nicht im Grundgerüst hart verdrahten, aber architektonisch vorbereiten

### Technische Grundstruktur
- Solution angelegt
- Shared Core angelegt
- erstes MAUI-Projekt angelegt
- Dokumentation angelegt
- Beispiel-Dateien angelegt

### Offene Punkte für den nächsten Block
- Layout-Designer technisch konkret beginnen
- MAUI-Navigation auf echte Seiten ausbauen
- Datei-Services an App-Lebenszyklus anbinden
- VISCA-Kommunikation als erste echte Runtime-Implementierung ergänzen

## 2026-03-28 – Block 1: Buildfähige Projektbasis herstellen
### Ausgangslage
- Solution und Projektdateien waren angelegt, aber die MAUI-App war nicht buildfähig
- Fehlende PackageReference für Microsoft.Maui.Controls (MA002 ab .NET 10)
- Fehlende Plattform-Entry-Points (Platforms/Windows, Platforms/Android)
- XML-Entity-Fehler in HomePage.xaml
- Veraltete MainPage-API in App.xaml.cs
- Nur 3 von 5 Navigationsseiten vorhanden

### Durchgeführte Änderungen
- `Models/CameraType.cs` als fachliches Enum ergänzt
- `Views/LiveControlPage` und `Views/ParticipantsPage` als neue Platzhalter-Seiten angelegt
- AppShell auf 5 Tabs erweitert (Start, Layout, Live, Teilnehmer, Einstellungen)
- SettingsPage mit echten Eingabefeldern für Layout-Pfad und Kamera-Verbindungen
- Plattform-Entry-Points für Windows und Android angelegt
- PackageReference für Microsoft.Maui.Controls ergänzt
- XML-Entity-Fehler und veraltete API behoben

### Ergebnis
- Solution baut erfolgreich (CLI und Visual Studio)
- Alle 5 Navigationsseiten vorhanden und testbar
- Core und App sauber verknüpft
- Fachliche Grundstruktur erkennbar

### Offene Punkte für den nächsten Block
- Layout-Designer technisch konkret beginnen
- Datei-Services an App-Lebenszyklus anbinden
- SettingsPage mit echter Persistenz verdrahten
- VISCA-Kommunikation als erste echte Runtime-Implementierung ergänzen

## 2026-03-28 – Block 1b: CameraProfile und erster Remote-Commit
### Ausgangslage
- Lokale Projektbasis vollständig und buildfähig, aber noch nie ins Remote-Repository committed
- `Models/CameraProfile.cs` fehlte als explizit gefordertes Kamera-Konfigurationsmodell
- `CameraDefinition.cs` hatte keine Preset-Bereiche und kein SupportsTracking
- Veraltete Root-Duplikate der Dokumentationsdateien vorhanden

### Durchgeführte Änderungen
- `Models/CameraProfile.cs` mit Name, Host, Port, PresetStart, PresetEnd, SupportsTracking, CameraType angelegt
- `CameraDefinition.cs` entfernt, `LayoutDocument` auf `CameraProfile` umgestellt
- Root-Duplikate entfernt
- Gesamte Projektbasis erstmals ins Remote-Repository committed

### Ergebnis
- Build erfolgreich (CLI + Visual Studio, 0 Fehler, 0 Warnungen)
- Alle angeforderten Models vorhanden: CameraType, CameraProfile, LayoutDocument, LayoutBlock, LayoutSeat, ParticipantEntry, AppSettings
- 5-Tab-Navigation mit Platzhalterseiten komplett
- SettingsPage mit Eingabefeldern für Layout-Pfad und Kameraverbindungen
- Remote-Repository auf aktuellem Stand

### Offene Punkte für den nächsten Block
- Layout-Designer technisch konkret beginnen
- Datei-Services an App-Lebenszyklus anbinden
- SettingsPage mit echter Persistenz verdrahten
- VISCA-Kommunikation als erste echte Runtime-Implementierung ergänzen
