using msGIS.ProApp_Base_FIWARE;
using msGIS.ProApp_Vault_FIWARE;

using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Editing.Templates;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msGIS.ProApp_FiwareSummit
{
    internal static class Fusion
    {
        // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
        // 3.3.06/20231214/msGIS_FIWARE_rt_006: Rename FiwareSummit to FiwareSummit/EntityFile (Project, Namespace, Module, ID, Plugin).
        // 3.4.11/20240131/msGIS_FiwareSummit_rt_001: Separately Summit/Lab.
        private static readonly string m_ModuleName = "Fusion";
        public static readonly string m_AppropriateBaseVersion = "3.3.41";

        #region Common

        internal static Global m_Global { get; set; }
        internal static Messages m_Messages { get; set; }
        internal static General m_General { get; set; }
        internal static Helper_FindFeature m_Helper_FindFeature { get; set; }
        internal static Helper_Framework m_Helper_Framework { get; set; }
        internal static Helper_Image m_Helper_Image { get; set; }
        internal static Helper_Layer m_Helper_Layer { get; set; }
        internal static Helper_Search m_Helper_Search { get; set; }
        internal static Helper_Selection m_Helper_Selection { get; set; }
        internal static Helper_Zoom m_Helper_Zoom { get; set; }
        internal static Helper_Uni m_Helper_Uni { get; set; }
        internal static Helper_Geom m_Helper_Geom { get; set; }
        internal static Helper_Op m_Helper_Op { get; set; }
        internal static Fiware_RestApi_NetHttpClient m_Fiware_RestApi_NetHttpClient { get; set; }

        #endregion Common

        #region Links of [this]

        // internal static Dockpane_* m_Dockpane_* { get; set; }
        // internal static UserControl_* m_UserControl_* { get; set; }

        internal static Dockpane_EntityTypes m_Dockpane_EntityTypes { get; set; }
        internal static UserControl_EntityTypes m_UserControl_EntityTypes { get; set; }

        #endregion Links of [this]

        #region Def

        // 3.4.12/20240102/msGIS_FiwareSummit_rt_002: APRX SubPhrase "Test" --> "Summit" (2x APRX Summit/Lab).
        internal const string m_ProjectMainPhrase = "Fiware";
        internal const string m_ProjectSubPhrase = "Summit";

        // Settings
        // internal const string m_FileNameSettings = "msGIS_Settings_FIWARE.json";
        internal static string m_MapTag = "Fiware_Summit";

        internal const string m_StateID_Main = "msGIS_FiwareSummit_StateID_Main";
        internal const string m_StateID_Menu = "msGIS_FiwareSummit_StateID_Menu";
        internal const string m_StateID_Map = "msGIS_FiwareSummit_StateID_Map";

        internal const string m_StateID_TS_EntityTypes = "msGIS_FiwareSummit_StateID_EntityTypes";
        internal const string m_StateID_CanChangeEntityTypes = "msGIS_FiwareSummit_StateID_CanChangeEntityTypes";
        internal const string m_StateID_IsEnableEntityTypes = "msGIS_FiwareSummit_StateID_IsEnableEntityTypes";

        // 3.3.08/20240109/msGIS_FIWARE_rt_012: Init Fiware_RestApi_NetHttpClient before Plugin Datasource OpenTable/GetTableNames.
        // 3.3.09/20240110/msGIS_FIWARE_rt_014: Configurable URI.
        // Get all entities:
        // https://fiwaredev.msgis.net/ngsi-ld/v1/types 
        // {"id":"urn:ngsi-ld:EntityTypeList:490ad7f0-9f18-11ee-91d3-0242ac120003","type":"EntityTypeList","typeList":["Hydrant","NgsiProxyConfig","Schwimmbad","Trinkbrunnen"]}
        // Get entity type:
        // https://fiwaredev.msgis.net/ngsi-ld/v1/entities?type=Hydrant&offset=0&limit=1
        // [{"id":"urn:ngsi-ld:Hydrant:HYDRANTOGD.36612499","type":"Hydrant","OBJECTID":{"type":"Property","value":36612499},"location":{"type":"GeoProperty","value":{"type":"Point","coordinates":[16.45231,48.157012,161.78]}}}]
        // Empty default! - user has to know path to the server e.g. "https://fiwaredev.msgis.net"
        internal const int m_Datasource_Version = 3;
        internal const string m_DatasourcePath = "https://fiwaredev.msgis.net";                 // Example for the test - user can set path to any server! e.g. "https://fiwaredev.msgis.net"
        internal const string m_DatasourceTypes = "/ngsi-ld/v1/types";
        internal const string m_DatasourceEntities = "/ngsi-ld/v1/entities?";
        internal const string m_DatasourceEventsource = "/ngsi-proxy/eventsource/<GUID>";       // Place holder only! - will be exchanged from ProxyConfig.
        internal static SpatialReference m_DatasourceSR = SpatialReferences.WGS84;
        internal const int m_DatasourceLimit = 960;                                             // Limit > 0 max 2000 e.g. 60 test 960 ok > 1200 error
        internal const int m_DatasourceOffset = 0;                                              // Start from e.g. 0 max <= limit
        internal const string m_DatasourceUpdateOId = "<OID>";                                  // Place holder only! - will be exchanged for Table/Update OID name (SE_SDO_ROWID, OBJECTID, ?)

        #endregion Def

        #region UI

        internal const string m_ButtonID_ProjectInfo = "msGIS_FiwareSummit_ButtonID_ProjectInfo";

        internal const string m_DockpaneID_EntityTypes = "msGIS_FiwareSummit_DockpaneID_EntityTypes";
        internal const string m_ButtonID_TS_EntityTypesBoard = "msGIS_FiwareSummit_ButtonID_TS_EntityTypes";

        #endregion UI

        #region Data

        private static Layer m_LayerEntitiesPoints = null;
        internal static string m_LayerTagEntitiesPoints = "Entities_Points";
        //internal static string m_LayerNameEntitiesPoints = "Entities_Points";
        internal static string m_FieldNameEntitiesPoints_OBJECTID = "OBJECTID";                     // OBJECTID
        internal static string m_FieldNameEntitiesPoints_SHAPE = "SHAPE";                           // Geometry
        internal static string m_FieldNameEntitiesPoints_NAME = "NAME";                             // NAME (Entity OId)

        #endregion Data

        #region Info

        internal static async Task ProjectInfo()
        {
            try
            {
                string prjInfo = await Fusion.m_General.GetProjectInfoAsync();
                if (!string.IsNullOrEmpty(prjInfo))
                {
                    /*
                    string settingsPath = Settings_EntityTypes.SettingsPath;
                    if (!string.IsNullOrEmpty(settingsPath))
                        prjInfo += Environment.NewLine + $"Settings: {settingsPath}";
                    */

                    Layer layerEntitiesPoints = await Fusion.GetLayerEntitiesPointsAsync();
                    if (layerEntitiesPoints == null)
                        return;
                    string connInfo = await Fusion.m_General.GetConnectionInfoAsync(layerEntitiesPoints);
                    if (!string.IsNullOrEmpty(connInfo))
                        prjInfo += Environment.NewLine + Environment.NewLine + connInfo;

                    await Fusion.m_Messages.AlertAsyncMsg(prjInfo, "Project Info");
                }

            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ProjectInfo");
            }
        }

        #endregion Info

        #region One-time reference

        internal static async Task<Layer> GetLayerEntitiesPointsAsync() => await GetLayerEntitiesPointsAsync(true);
        internal static async Task<Layer> GetLayerEntitiesPointsAsync(bool queueNewTask)
        {
            try
            {
                if (m_LayerEntitiesPoints == null)
                    m_LayerEntitiesPoints = await Fusion.m_Helper_Layer.GetLayerByTagAsync(m_LayerTagEntitiesPoints, queueNewTask);    // Fusion.m_Helper_Layer.GetLayerByNameAsync(m_LayerNameEntitiesPoints);
                return m_LayerEntitiesPoints;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "GetLayerEntitiesPointsAsync");
                return null;
            }
        }

        private static bool m_HasOneTimeRefsEntityTypes = false;
        internal static async Task CleanOneTimeRefsAsync()
        {
            try
            {
                m_LayerEntitiesPoints = null;

                m_HasOneTimeRefsEntityTypes = false;

                await m_Helper_Layer.CleanOneTimeRefs();
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "CleanOneTimeRefsAsync");
            }
        }

        private static async Task<bool> GetOneTimeRefsEntityTypesAsync(bool queueNewTask, Helper_Progress helper_Progress, uint maxRange)
        {
            try
            {
                int progressCount = 0;

                progressCount++;
                await helper_Progress.NotifyProgressAsync(m_LayerTagEntitiesPoints);
                m_LayerEntitiesPoints = await Fusion.GetLayerEntitiesPointsAsync(queueNewTask);
                //if (m_LayerEntitiesPoints == null)
                //    return false;

                if (progressCount != maxRange)
                    throw new Exception($"Progress count={progressCount} <> maxRange={maxRange}!");

                return true;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "GetOneTimeRefsEntityTypesAsync");
                return false;
            }
        }
        internal static async Task<bool> GetOneTimeRefsEntityTypesAsync()
        {
            Helper_Working helper_Working = null;
            try
            {
                if (m_HasOneTimeRefsEntityTypes)
                    return true;

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    helper_Working = new Helper_Working(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework);
                    await helper_Working.ShowDelayedWorkingAsync("EntityTypes - Vorbereitung der Anzeige");
                }

                uint maxRange = 1;
                Helper_Progress helper_Progress = null;
                bool queueNewTask = false;
                // Task with progress
                // LayerPlanung is known already
                helper_Progress = new Helper_Progress(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework, false);
                await helper_Progress.ShowProgressAsync("EntityTypes - Vorbereitung der Anzeige", maxRange, true);
                m_HasOneTimeRefsEntityTypes = await QueuedTask.Run(async () =>
                {
                    bool result = await GetOneTimeRefsEntityTypesAsync(queueNewTask, helper_Progress, maxRange);
                    return result;
                }, helper_Progress.ProgressAssistant);
                await helper_Progress.FinishProgressAsync();

                return m_HasOneTimeRefsEntityTypes;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "GetOneTimeRefsEntityTypesAsync");
                return false;
            }
            finally
            {
                if (helper_Working != null)
                    await helper_Working.FinishWorkingAsync();
            }
        }

        #endregion One-time reference

    }
}
