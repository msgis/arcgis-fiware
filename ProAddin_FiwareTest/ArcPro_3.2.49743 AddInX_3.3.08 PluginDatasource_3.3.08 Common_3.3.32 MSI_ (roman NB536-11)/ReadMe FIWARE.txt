===================================================================================================
Pro Fiware

Project:
https://app.asana.com/0/1204285422260806/1204530723943675/
old: https://bitbucket.org/msgis/fiwaresummit-arcgispro
https://github.com/msgis/arcgis-fiware.git

Author:
mailto:roman.trojan@msgis.com
Time:
[0141] Fiware.Summit _ms.GIS.Intern PRJ.Fiware_Summit
[0200] LAB _ms.GIS.Intern LAB.FIWARE

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
esriPlugin
---------------------------------------------------------------------------------------------------
Plugin "msGIS.ProPluginDatasource_EntityFile.esriPlugin" installieren.
Plugin "msGIS.ProPluginDatasource_FiwareHttpClient.esriPlugin" installieren.
(doppelklicken, aus ArcPro, per GUID, mit MSBuild oder Visual Studio)
Sollte in folgendem Ordner ersichtlich sein:
C:\Users\[user]\Documents\ArcGIS\AddIns\ArcGISPro3.[x]
Nach einem Start von ArcPro sollten die Komponenten ersichtlich werden:
C:\Users\[user]\AppData\Local\ESRI\ArcGISPro3.[x]\AssemblyCache

---------------------------------------------------------------------------------------------------
ArcPro
---------------------------------------------------------------------------------------------------
ArcPro starten.
(aprx doppelklicken oder ArcGISPro.exe und aprx auswählen)
Wenn alles korrekt abläuft müsste sich im Ribbon rechts eine Tab "FiwareTest" zeigen.
Tab "FiwareTest" klicken, es müsste sich die Gruppe "FiwareTest" mit weiteren Controls zeigen.
Button "Info FiwareTest" ist nur informativ.
Button "Entity Types" klicken, es müsste sich rechts eine Dockpane "FiwareTest-Board" zeigen.

Nun kann per ComboBox "Entity type" ausgewählt werden und die Daten entsprechend Entities in ArcGIS Pro dargestellt werden.
Je nach Version der Entwicklung kann es verschiedene Abweichungen oder Erweiterungen geben.
