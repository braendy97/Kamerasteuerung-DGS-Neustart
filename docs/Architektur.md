# Architektur

## Zielstruktur
- `Kamerasteuerung.DGS.Core`
  - Modelle
  - Kamera-Profile
  - VISCA-Logik
  - Datei-Services
- `Kamerasteuerung.DGS.App`
  - MAUI-UI
  - Navigation
  - Designer / Arbeitsfenster / Einstellungen

## Hauptprinzip
Die UI darf keine harte Fachlogik mehr enthalten.
Die Alt-App war stark formular- und buttonzentriert.
Der Neubau trennt:
- Darstellung
- Datenmodell
- Dateiverwaltung
- VISCA-Kommunikation

## Layout-Speicherung
Das Layout wird als Datei gespeichert.
Die App speichert nur:
- zuletzt verwendeten Layoutpfad
- lokale Einstellungswerte

## Mehrgeräte-Idee
Mehrere Geräte dürfen dieselbe Layoutdatei laden.
Gleichzeitiges Schreiben muss abgesichert werden.

## Kameraprofile
- V600
- SMTAV V60XL

Beide Profile teilen sich dieselbe Grundlogik, unterscheiden sich aber bei:
- manuellen Bewegungen
- Antwortverhalten
- Sonderfunktionen
- Tracking-Fähigkeiten
