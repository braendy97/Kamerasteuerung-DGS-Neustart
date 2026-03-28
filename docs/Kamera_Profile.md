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

## Technischer Stand (VISCA-Core)
Aktuell existieren im Repository zwei Ebenen von "Profilen":
- Layout-/App-Ebene: `Kamerasteuerung.DGS.Core.Models.CameraProfile` (Host/Port, PresetStart/End, SupportsTracking, `CameraType`)
- VISCA-Core-Ebene: `Kamerasteuerung.DGS.Core.Profiles.CameraProfile` + `CameraDeviceType` (geräte-/protokollbezogene Fähigkeiten)

Für die spätere Kamerakommunikation gilt:
- Preset-Nummern werden fachlich im Nutzerbereich 10–210 validiert.
- Aus einem (Model-)Kameraprofil + Preset-Aktion wird zentral ein VISCA-Befehl erzeugt.
- TCP-Senden/Antwortlesen ist im Core als kleiner Transportservice vorbereitet (ohne UI-Anbindung).

Transport-Defaults (aktuell):
- ConnectTimeout: 2s
- WriteTimeout: 2s
- ReadTimeout: ~400ms (V600) / ~600ms (SMTAV V60XL)
- Antwortlesen ist standardmäßig aktiv (kann pro Options deaktiviert werden)
