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

## 2026-03-28 – Block 2: JSON-Dateiformat + Settings-Persistenz (Grundlage)
### Ausgangslage
- Buildfähige Basis vorhanden, aber:
  - keine echte Persistenz für App-Settings
  - kein konsistentes, dokumentiertes JSON-Layoutformat (nur Modell vorhanden)
  - SettingsPage war nur UI ohne Speicherfunktion

### Durchgeführte Änderungen
- Core-Modelle gezielt ergänzt:
  - `LayoutDocument`: `Id` ergänzt sowie Alias-Properties für Version/Canvas/LastModified (ohne State-Duplikate)
  - `AppSettings`: Last-Layout-Pfad + vorbereitende Kamera-Defaults (V600/SMTAV Host+Port)
- Core-Services erweitert:
  - `LayoutFileService`: `LoadLayoutAsync` / `SaveLayoutAsync` (UTF-8, WriteIndented, Temp+Backup)
  - `SettingsFileService`: `LoadAppSettingsAsync` / `SaveAppSettingsAsync` (UTF-8, WriteIndented)
- MAUI minimal angebunden:
  - `SettingsPage` lädt/speichert `appsettings.json` in `FileSystem.AppDataDirectory`
  - keine Dateiauswahl und kein Auto-Load beim Start fest verdrahtet (nur vorbereitet)
- Dokumentation aktualisiert: klare Trennung Layout-Datei vs. App-Settings-Datei

### Ergebnis
- Technische Grundlage für externe Layoutdatei (JSON) vorhanden und dokumentiert
- App-Settings können in der MAUI-App gespeichert und wieder geladen werden
- Kein Layout-Designer/Editor und keine Kamera-Runtime implementiert (bewusst)

### Offene Punkte für den nächsten Block
- Auto-Load beim App-Start (auf Basis `LastLayoutFilePath`) verdrahten
- Layout-Import/Export/"Speichern unter" vorbereiten
- Layout-Designer technisch beginnen

## 2026-03-28 – Block 3: App-Startfluss mit Auto-Load + sichtbarer Status
### Ausgangslage
- App-Settings konnten gespeichert werden und Layout-Persistenz war vorhanden.
- Beim Start wurde jedoch noch nichts automatisch geladen.
- Kein sichtbarer Fallback-/Fehlerstatus auf der Startseite.

### Durchgeführte Änderungen
- Minimaler App-Zustand eingeführt (`AppSessionState`):
  - geladene `AppSettings`
  - aktuelles `LayoutDocument` oder `null`
  - `StatusMessage` für UI
- Startlogik ergänzt (`AppStartupService`):
  - lädt `appsettings.json`
  - prüft `LastLayoutFilePath`
  - lädt Layout nur wenn Pfad gesetzt und Datei vorhanden
  - behandelt Leerfall/Fehlerfall ohne App-Absturz und setzt klaren Status
- App-Start verdrahtet (`App.xaml.cs`): Initialisierung wird beim Fensterstart asynchron angestoßen.
- `HomePage` zeigt Status und Layoutname sichtbar an.
- `SettingsPage` nutzt DI-Services/State, damit gespeicherte Werte konsistent im App-Zustand landen.

### Ergebnis
- App startet immer sauber.
- Zustände sind unterscheidbar sichtbar:
  - „Kein Layoutpfad konfiguriert“
  - „Layoutdatei nicht gefunden“
  - „Layout konnte nicht geladen werden“
  - „Layout erfolgreich geladen: <Name>“

### Offene Punkte für den nächsten Block
- Auto-Load so erweitern, dass auch ein UI-Refresh ohne Timer möglich ist (optional, später)
- Dateiauswahl-UI für Layoutpfad
- Import/Export/„Speichern unter“

## 2026-03-28 – Block 4: Manuelles Layout öffnen/speichern + neues Layout
### Ausgangslage
- Auto-Load beim Start funktionierte, aber es gab noch keine Benutzerführung für manuelles Öffnen/Speichern.
- Es war nicht möglich, ein neues leeres Layout zu erzeugen.

### Durchgeführte Änderungen
- `LayoutSessionService` ergänzt:
  - `CreateNewLayout()` erzeugt ein leeres Default-`LayoutDocument` und setzt Status
  - `OpenLayoutAsync()` öffnet eine JSON-Datei via `FilePicker` und lädt sie über `LayoutFileService`
  - `SaveLayoutAsync()` speichert das aktuelle Layout (direkt in `LastLayoutFilePath` oder minimal nach AppData `layouts/`)
  - nach Öffnen/Speichern wird `LastLayoutFilePath` aktualisiert und `appsettings.json` gespeichert
- UI ergänzt:
  - `HomePage` und `LayoutDesignerPage` zeigen Buttons „Neues Layout“, „Layout öffnen“, „Layout speichern“
  - Anzeige von Layoutname, Layoutpfad und Statusmeldung

### Ergebnis
- Manuelles Öffnen/Speichern funktioniert ohne Layout-Designer.
- App-Zustand (`AppSessionState`) wird nach Neu/Öffnen/Speichern konsistent aktualisiert.
- `LastLayoutFilePath` wird nach Öffnen/Speichern persistiert und kann beim nächsten Start wieder für Auto-Load verwendet werden.

### Offene Punkte für den nächsten Block
- Echter Save-Picker / „Speichern unter“ (optional)
- Layout-Designer (Bearbeitung) beginnen

## 2026-03-28 – Block 5: LayoutDesignerPage als Designer-Grundzustand
### Ausgangslage
- Layout konnte bereits geladen/gespeichert werden, aber der "Designer" war faktisch nur Text.
- Es fehlte eine sichtbare Arbeitsfläche und eine sachliche Layout-Zusammenfassung.

### Durchgeführte Änderungen
- `LayoutDesignerPage` ausgebaut:
  - klarer Leerzustand (Hinweisfläche, wenn kein Layout geladen ist)
  - Layout-Zusammenfassung: Name, Pfad, Canvas-Größe, Counts (Blöcke/Sitze/Kameras/Teilnehmer)
  - sichtbare Arbeitsfläche mit Rand (Breite/Höhe aus `LayoutDocument.Canvas` abgeleitet, bei Bedarf skaliert)
- Kleine Hilfslogik im Code-Behind (`ComputeWorkAreaSize`) zur skalierenden Darstellung ohne zusätzliche Libraries.

### Ergebnis
- Benutzer sieht erstmals eine echte, erkennbare Layout-Arbeitsfläche.
- Die Arbeitsfläche basiert fachlich auf der Layout-Canvas-Größe.
- Geladener Zustand und leerer Zustand sind klar unterscheidbar.

### Offene Punkte für den nächsten Block
- Erste echte Platzierungs-/Bearbeitungslogik (z. B. Blöcke/Sitze anzeigen)
- Drag & Drop / freie Positionierung (später)

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
