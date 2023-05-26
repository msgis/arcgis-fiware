using msGIS.ProApp_Common_MA31_GIS_3x;

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
        private static readonly string m_ModuleName = "Fusion";

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

        #endregion Common

        #region Links of [this]

        // internal static Dockpane_* m_Dockpane_* { get; set; }
        // internal static UserControl_* m_UserControl_* { get; set; }

        internal static Dockpane_EntityTypes m_Dockpane_EntityTypes { get; set; }
        internal static UserControl_EntityTypes m_UserControl_EntityTypes { get; set; }

        #endregion Links of [this]

        #region Def

        internal const string m_ProjectMainPhrase = "Fiware";
        internal const string m_ProjectSubPhrase = "Summit";

        // Settings
        // internal const string m_FileNameSettings = "msGIS_Settings_FiwareSummit.json";
        internal static string m_MapTag = "Fiware_Summit";

        internal const string m_StateID_Main = "msGIS_FiwareSummit_StateID_Main";
        internal const string m_StateID_Menu = "msGIS_FiwareSummit_StateID_Menu";
        internal const string m_StateID_Map = "msGIS_FiwareSummit_StateID_Map";

        internal const string m_StateID_TS_EntityTypes = "msGIS_FiwareSummit_StateID_EntityTypes";
        internal const string m_StateID_CanChangeEntityTypes = "msGIS_FiwareSummit_StateID_CanChangeEntityTypes";
        internal const string m_StateID_IsEnableEntityTypes = "msGIS_FiwareSummit_StateID_IsEnableEntityTypes";

        #endregion Def

        #region UI

        internal const string m_ButtonID_ProjectInfo = "msGIS_FiwareSummit_ButtonID_ProjectInfo";

        internal const string m_DockpaneID_EntityTypes = "msGIS_FiwareSummit_DockpaneID_EntityTypes";
        internal const string m_ButtonID_TS_EntityTypesBoard = "msGIS_FiwareSummit_ButtonID_TS_EntityTypes";

        #endregion UI

        #region Data

        // select * from nis.leitung_l
        // private static Layer m_LayerLeitungL = null;
        // internal static string m_LayerTagLeitungL = "LEITUNG_L";
        // internal static string m_LayerNameLeitungL = "LEITUNG_L";

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

                    /*
                    Layer layerLeitungL = await Fusion.GetLayerLeitungLAsync();
                    if (layerLeitungL == null)
                        return;
                    string connInfo = await Fusion.m_General.GetConnectionInfoAsync(layerLeitungL);
                    if (!string.IsNullOrEmpty(connInfo))
                        prjInfo += Environment.NewLine + Environment.NewLine + connInfo;
                    */

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

        /*
        internal static async Task<Layer> GetLayerLeitungLAsync() => await GetLayerLeitungLAsync(true);
        internal static async Task<Layer> GetLayerLeitungLAsync(bool queueNewTask)
        {
            try
            {
                if (m_LayerLeitungL == null)
                    m_LayerLeitungL = await Fusion.m_Helper_Layer.GetLayerByTagAsync(m_LayerTagLeitungL, queueNewTask);    // Fusion.m_Helper_Layer.GetLayerByNameAsync(m_LayerNameLeitungL);
                return m_LayerLeitungL;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "GetLayerLeitungLAsync");
                return null;
            }
        }
        */

        private static bool m_HasOneTimeRefsEntityTypes = false;
        internal static async Task CleanOneTimeRefsAsync()
        {
            try
            {
                // m_LayerLeitungL = null;

                m_HasOneTimeRefsEntityTypes = false;

                // 3.3.20/20230127/msGIS_ProApp_Common_rt_033: Get Layers/SaTables Tags.
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

                //progressCount++;
                //await helper_Progress.NotifyProgressAsync(Fusion.m_LayerLeitungL);
                //m_LayerLeitungL = await Fusion.GetLayerLeitungLAsync(queueNewTask);
                ////if (m_LayerLeitungL == null)
                ////    return false;

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

                uint maxRange = 0;
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
