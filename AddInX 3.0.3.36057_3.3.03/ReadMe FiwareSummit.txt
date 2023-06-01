===================================================================================================
FiwareSummit

Install:
P:\MS\link\roman\FiwareSummit
C:\Users\[user]\Documents\ArcGIS\AddIns\ArcGISPro

Project:
https://app.asana.com/0/1204285422260806/1204530723943675/

Author:
mailto:roman.trojan@msgis.com
Time:
[0141] Fiware.Summit _ms.GIS.Intern PRJ.Fiware_Summit

AddinX:
https://bitbucket.org/msgis/fiwaresummit-arcgispro/src/master/
https://app.asana.com/0/1204285422260806/1204530723943675/

===================================================================================================

---------------------------------------------------------------------------------------------------
Entity-Types
---------------------------------------------------------------------------------------------------
Die Daten werden als JSON-Objekt aus Rest-Api übernommen.
Alternativ könnten die Daten über JSON-File transferiert werden.

---------------------------------------------------------------------------------------------------
APRX
---------------------------------------------------------------------------------------------------
APRX muss "Fiware" und "Summit" im Dateinamen beinhalten.
z.B. "RT Wien Fiware_Summit.aprx"
Map in APRX muss den Tag "Fiware_Summit" beinhalten.

---------------------------------------------------------------------------------------------------
AddInX
---------------------------------------------------------------------------------------------------
AddInX "msGIS.ProApp_FiwareSummit.esriAddinX" installieren.
(doppelklicken, aus ArcPro oder per GUID)

---------------------------------------------------------------------------------------------------
ArcPro
---------------------------------------------------------------------------------------------------
ArcPro starten.
(aprx doppelklicken oder ArcGISPro.exe und aprx auswählen)
Wenn alles korrekt abläuft müsste sich im Ribbon rechts eine Tab "FiwareSummit" zeigen.
Tab "FiwareSummit" klicken, es müsste sich die Gruppe "FiwareSummit" mit weiteren Controls zeigen.
Button "Info FiwareSummit" ist nur informativ.
Button "Entity Types" klicken, es müsste sich rechts eine Dockpane "FiwareSummit-Board" zeigen.

Nun kann per ComboBox "Entity type" ausgewählt werden.
Mit Klick auf Button "Entities zeigen" werden laut ausgewähltem Typ die Entities per REST-Api
in ein JArray geladen und zu einem vordefinierten Layer hinzugefügt.
Alle zuvor vorhandenen Objekte am Layer werden gelöscht.

(Stand 2023.05.26 Version 3.3.03)

Geplant:
Layer Objekte mit Attribut "Typ" versehen, damit nur die entsprechenden Objekte gelöscht werden und auch später besser identifiziert werden können.
Löschen der Objekte entsprechend "Typ".
Löschen aller Objekte (Reset).
Dynamischer Update anhand per REST-Api. 
