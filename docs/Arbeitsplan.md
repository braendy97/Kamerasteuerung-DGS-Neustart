# Arbeitsplan

Stand: 2026-03-28

## Ziel
Neubau einer flexiblen Kamerasteuerungs-App für DGS-Meetings mit:
- freiem Layout-Designer
- speicherbarer Layout-Datei
- Teilnehmerliste mit Drag & Drop
- gemeinsamer VISCA-Fachlogik
- MAUI als erste UI-Basis
- optional später WPF mit gemeinsamem Core

## Phase 1 – Projektgrundlage ✔
- Solution angelegt
- Shared Core angelegt (Models, Profiles, Constants, Services)
- MAUI-App als erster Client angelegt (5 Navigationsseiten)
- Plattform-Entry-Points für Windows und Android
- Logdateien und Dokumentation angelegt
- Kamera-Profile V600 / SMTAV V60XL festgezogen
- Build erfolgreich (CLI + Visual Studio)

## Phase 2 – Datenmodell und Dateiablage
- Layout-Dateiformat festziehen (`*.dgslayout.json`)
- App-Settings-Datei festziehen
- frei wählbaren Layoutpfad in Einstellungen vorsehen
- automatisches Laden der letzten Layoutdatei beim Start
- Import / Export / Speichern unter vorbereiten
- Konfliktregel für Mehrgeräte-Nutzung definieren

## Phase 3 – Kamera-Core
- VISCA-Basis kapseln
- Kamera-Profile für V600 und SMTAV V60XL
- Preset-Bereich für Nutzer auf 10–210 begrenzen
- kamerainterne Spezialbereiche schützen
- ACK-/Timeout-Strategie pro Kameratyp einbauen
- spätere Tracking-Erweiterungen nur für SMTAV aktivierbar machen

## Phase 4 – Layout-Designer
- freie Arbeitsfläche statt starrem Raster
- optionales Snap-to-Grid
- Plätze frei platzierbar
- Blöcke gruppierbar
- Blöcke drehbar
- Bühne / Zuschauerkamera vor dem Setzen auswählbar
- nach Speichern automatische Übernahme ins Arbeitsfenster

## Phase 5 – Live-Arbeitsfenster
- gespeichertes Layout laden
- Plätze klickbar
- Preset-Aufruf je Platz
- Kamera 1 / Kamera 2 als flexible Bereiche anzeigen
- Fokus auf schnelle Bedienung im Meetingbetrieb

## Phase 6 – Teilnehmerliste
- Namen hinzufügen / ändern / löschen
- Liste nach jeder Änderung alphabetisch sortieren
- Drag & Drop:
  - Liste → Platz
  - Platz → Platz
  - Platz → Liste
- Zielplatz belegt:
  - bestehender Name zurück in die Liste
  - neuer Name übernimmt den Platz

## Phase 7 – Mehrgeräte-Nutzung
- gemeinsames Laden derselben Layoutdatei erlauben
- Schreibkonflikte absichern
- Backup-Datei beim Speichern
- Extern-Änderung erkennen
- Nutzerabfrage bei Konflikt

## Phase 8 – Polishing und zweite UI
- Bedienfluss verfeinern
- WPF-Client nur bei echtem Mehrwert ergänzen
- Kamera-Preview / Diagnose optional planen
- reale Kameratests und Preset-Validierung

## Reihenfolge der nächsten Umsetzungsblöcke
1. Shared Core fachlich vervollständigen
2. Settings- und Layout-Datei-Service fertigstellen
3. MAUI-Navigation und leere Arbeitsseiten aufbauen
4. Layout-Designer minimal nutzbar machen
5. Teilnehmerliste und Drag & Drop ergänzen
6. VISCA-Kommunikation real anschließen
