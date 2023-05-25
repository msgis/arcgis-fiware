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
Settings
---------------------------------------------------------------------------------------------------
JSON-File aus https://fiwaredev.msgis.net/ngsi-ld/v1/types manuell erstellen.
z.B. mithilfe https://jsonlint.com/
Als "msGIS_Settings_FiwareSummit.json" im APRX-Folder ablegen.
z.B.
{
	"id": "urn:ngsi-ld:EntityTypeList:79576476-fa42-11ed-9e52-0242ac120003",
	"type": "EntityTypeList",
	"typeList": ["Hydrant", "Schwimmbad", "Trinkbrunnen"]
}

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

(Stand 2023.05.25)
