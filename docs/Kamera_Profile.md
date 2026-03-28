# Kamera-Profile

## Gemeinsame Basis
- VISCA-basierte Steuerung
- PTZ-Steuerung über TCP
- Preset-Adressierung über parametrisierte Befehle
- Nutzerbereich für Presets: 10–210

## V600
- Zuschauerkamera
- klassische PTZ-Konferenzkamera
- ohne geplante Tracking-Sonderlogik im ersten Ausbauschritt
- toleranteres Kommunikationsprofil bei manuellen Bewegungen vorsehen

## SMTAV V60XL
- Bühnenkamera
- PTZ + AI-Tracking
- Tracking-Funktionen später per Feature-Flag aktivierbar
- strengeres Antwort-/ACK-Verhalten vorsehen

## Wichtige Projektregel
Die App nutzt keine fest codierten Einzelbefehle pro Presetplatz.
Stattdessen erzeugt der Kamera-Core den Befehl aus:
- Kameraprofil
- Befehlstyp
- Presetnummer
