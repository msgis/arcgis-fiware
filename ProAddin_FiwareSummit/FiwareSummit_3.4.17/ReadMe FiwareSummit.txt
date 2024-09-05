===================================================================================================
Pro FIWARE Summit

Project:
https://app.asana.com/0/1204285422260806/1204530723943675/
https://github.com/msgis/arcgis-fiware.git

Author:
mailto:roman.trojan@msgis.com

===================================================================================================

---------------------------------------------------------------------------------------------------
APRX
---------------------------------------------------------------------------------------------------
APRX muss "Fiware" und "Summit" im Dateinamen beinhalten.
z.B. "Smart City Wien Fiware_Summit.aprx"
Map muss den Tag "Fiware_Summit" beinhalten.
Map muss einen Layer mit Tag "Entities_Points" und Typ Point beinhalten.

---------------------------------------------------------------------------------------------------
esriAddInX
---------------------------------------------------------------------------------------------------
AddInX "msGIS.ProApp_FiwareReader.esriAddinX" installieren.
(doppelklicken, aus ArcPro, per GUID, mit MSBuild oder Visual Studio)
Sollte in folgendem Ordner ersichtlich sein:
C:\Users\[user]\Documents\ArcGIS\AddIns\ArcGISPro
Nach einem Start von ArcPro sollten die Komponenten ersichtlich werden:
C:\Users\[user]\AppData\Local\ESRI\ArcGISPro\AssemblyCache

---------------------------------------------------------------------------------------------------
ArcPro
---------------------------------------------------------------------------------------------------
ArcPro starten.
(aprx doppelklicken oder ArcGISPro.exe und aprx auswählen)
Der Layer mit Tag "Entities_Points" und Typ Point wird mit vorhandenen Daten geladen.
Die Daten werden aus Rest-Api übernommen (JSON-Objekte).

Fiware Daten können nun im Layer ausgetauscht werden.
Button "Info FiwareSummit" ist nur informativ.
Im Tab "FiwareSummit" Button "Entity Types" klicken, es müsste sich rechts eine Dockpane "FiwareSummit-Board" zeigen.
Server "Path" einstellen ("https://fiwaredev.msgis.net").
Nun kann per ComboBox "Entity type" ausgewählt werden.

Je nach Version der Entwicklung kann es verschiedene Abweichungen oder Erweiterungen geben.
