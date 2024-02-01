===================================================================================================
Pro FiwareSummit

Project:
https://app.asana.com/0/1204285422260806/1204530723943675/
https://github.com/msgis/arcgis-fiware.git

Author:
mailto:roman.trojan@msgis.com

===================================================================================================

---------------------------------------------------------------------------------------------------
Entity-Types
---------------------------------------------------------------------------------------------------
Die Daten werden aus Rest-Api übernommen (als JSON-Objekte).

---------------------------------------------------------------------------------------------------
APRX
---------------------------------------------------------------------------------------------------
APRX muss "Fiware" und "Test" im Dateinamen beinhalten.
z.B. "Smart City Wien Fiware_Test.aprx"
Map muss den Tag "Fiware_Test" beinhalten.
Map muss einen Layer mit Tag "Entities_Points" und Typ Point beinhalten.

---------------------------------------------------------------------------------------------------
esriAddInX
---------------------------------------------------------------------------------------------------
AddInX "msGIS.ProApp_FiwareTest.esriAddinX" installieren.
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
Wenn alles korrekt abläuft müsste sich im Ribbon rechts eine Tab "FiwareSummit" zeigen.
Tab "FiwareSummit" klicken, es müsste sich die Gruppe "FiwareSummit" mit weiteren Controls zeigen.
Button "Info FiwareSummit" ist nur informativ.
Button "Entity Types" klicken, es müsste sich rechts eine Dockpane "FiwareSummit-Board" zeigen.
Empty default! - user has to know path to the server e.g. "https://fiwaredev.msgis.net";

Nun kann per ComboBox "Entity type" ausgewählt werden und die Daten entsprechend Entities in ArcGIS Pro dargestellt werden.
Je nach Version der Entwicklung kann es verschiedene Abweichungen oder Erweiterungen geben.
