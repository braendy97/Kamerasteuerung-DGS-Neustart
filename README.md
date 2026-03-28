# Kamerasteuerung DGS – Neustart

Dieses Projekt ist ein **sauberer Neustart** für die Kamerasteuerung bei Meetings in Gebärdensprache.

Zielbild:
- frei gestaltbares Raumlayout statt fester Buttonmatrix
- getrennte Kamera-Profile für **Zuschauerkamera (V600)** und **Bühnenkamera (SMTAV V60XL)**
- gemeinsame Fachlogik für spätere parallele Entwicklung in **MAUI** und optional **WPF**
- Layout-Datei frei speicherbar und wieder ladbar
- automatisches Laden des zuletzt konfigurierten Layouts beim App-Start
- Grundlage für Mehrgeräte-Nutzung mit gemeinsamer Layoutdatei

## Aktueller Stand
Dieses Grundgerüst enthält:
- eine **Solution**
- ein **Shared-Core-Projekt**
- ein erstes **MAUI-Projekt**
- einen **Arbeitsplan**
- zwei **Logdateien**
- Beispiel-Dateien für Settings und Layout

## Wichtige Architekturentscheidung
Die bisherige Alt-App dient nur noch als **fachliche Referenz** für:
- VISCA-Steuerung
- Preset speichern / abrufen
- Kamera-spezifische Unterschiede

Die neue Oberfläche und das Datenmodell werden **neu aufgebaut**.

## Geplante Kernmodule
1. Layout-Designer
2. Live-Steuerung
3. Teilnehmerliste mit Drag & Drop
4. Kamera-/Preset-Engine
5. Datei- und Settings-Verwaltung

## Hinweise
- Das Grundgerüst wurde hier als Startstruktur erstellt.
- Der Code ist als **Projektstart** gedacht, nicht als fertig umgesetzte Fachanwendung.
- Vor der ersten lokalen Nutzung bitte die MAUI-Workloads und das lokale SDK prüfen.

## Einstieg
1. Solution in Visual Studio öffnen
2. Shared Core prüfen
3. MAUI-Projekt als Startprojekt verwenden
4. mit `docs/Arbeitsplan.md` weiterarbeiten
5. die nächsten Umsetzungsblöcke über die Copilot-Prompts steuern
