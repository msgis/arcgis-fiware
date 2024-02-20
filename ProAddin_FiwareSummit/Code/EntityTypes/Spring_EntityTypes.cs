using msGIS.ProApp_Common_FIWARE_3x;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ArcGIS.Core.Data;
using ArcGIS.Core.Data.PluginDatastore;
using ArcGIS.Core.Data.UtilityNetwork.Trace;
using ArcGIS.Core.Events;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Editing.Attributes;
using ArcGIS.Desktop.Framework;
// using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Events;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Input;

namespace msGIS.ProApp_FiwareSummit
{
    internal class Spring_EntityTypes
    {
        // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
        // 3.3.05/20231205/msGIS_FIWARE_rt_003: EntitiesToFile for use with SimplePointPlugin.
        // 3.3.05/20231206/msGIS_FIWARE_rt_004: Expertise FIWARE Integration ArcGIS PRO.
        // 3.3.05/20231207/msGIS_FIWARE_rt_005: ProPluginDatasource Integration for SimplePoint File-Format.
        // 3.3.06/20231221/msGIS_FIWARE_rt_008: Datasource URI.
        // 3.3.08/20240109/msGIS_FIWARE_rt_012: Init Fiware_RestApi_NetHttpClient before Plugin Datasource OpenTable/GetTableNames.
        // 3.3.09/20240110/msGIS_FIWARE_rt_014: Configurable URI.
        // 3.3.10/20240118/msGIS_FIWARE_rt_015: Restructuring AddInX.
        // 3.4.11/20240131/msGIS_FiwareSummit_rt_001: Separately Summit/Lab.
        private readonly string m_ModuleName = "Spring_EntityTypes";

        private Grid Grid_EntityTypes;
        private TextBox TextBox_ConnDs;
        private Button Button_GetEntities;
        private ComboBox ComboBox_EntityTypes;
        private Label Label_Count;
        private Button Button_EntityToLayer;

        private Layer m_LayerEntitiesPoints = null;

        private SubscriptionToken m_STMapViewChanged = null;
        private SubscriptionToken m_STMapMemberPropertiesChanged = null;

        private bool m_IsActivated_EntityTypes = false;

        internal bool m_CanChangeBoard_EntityTypes = true;
        // private bool m_SuspendControlsEvents = false;
        //private bool m_SuspendSetLayersChk = false;
        private bool m_HasSpecialEvents_EntityTypes = false;

        internal Spring_EntityTypes(Grid grid_EntityTypes, TextBox textBox_ConnDs, Button button_GetEntities, ComboBox comboBox_EntityTypes, Label label_Count, Button button_EntityToLayer)
        {
            Grid_EntityTypes = grid_EntityTypes;
            Grid_EntityTypes.IsEnabled = false;

            if (m_STMapViewChanged == null)
                m_STMapViewChanged = ActiveMapViewChangedEvent.Subscribe(OnMapViewChanged);

            TextBox_ConnDs = textBox_ConnDs;
            TextBox_ConnDs.IsEnabled = true;
            TextBox_ConnDs.Text = "";
            TextBox_ConnDs.TextChanged += TextBox_ConnDs_TextChanged;

            Button_GetEntities = button_GetEntities;
            Button_GetEntities.IsEnabled = false;
            Button_GetEntities.Click += Button_GetEntities_Click;

            ComboBox_EntityTypes = comboBox_EntityTypes;
            ComboBox_EntityTypes.IsEnabled = false;
            ComboBox_EntityTypes.DropDownClosed += ComboBox_EntityTypes_DropDownClosed;
            Label_Count = label_Count;

            Button_EntityToLayer = button_EntityToLayer;
            Button_EntityToLayer.IsEnabled = false;
            Button_EntityToLayer.Click += Button_EntityToLayer_Click;

            if (MapView.Active != null)
            {
                _ = InitActive_EntityTypesAsync("Spring_EntityTypes");
            }

        }

        private async void OnMapViewChanged(ActiveMapViewChangedEventArgs args)
        {
            Grid_EntityTypes.IsEnabled = false;
            try
            {
                if (await Fusion.m_General.WaitUntilMapViewReadyAsync(m_ModuleName))
                {
                    await InitActive_EntityTypesAsync("OnMapViewChanged");
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "OnMapViewChanged");
            }
        }

        private async void OnMapMemberPropertiesChanged(MapMemberPropertiesChangedEventArgs args)
        {
            // Occurs when any property of layer or standalone table changed.
            // e.g. after Fusion.m_Helper_Layer.SetLayerVisAsync
            try
            {
                if (m_STMapMemberPropertiesChanged == null)
                    throw new Exception("Miscarried MapMemberPropertiesChangedEvent subscription!");

                if ((args.MapMembers == null) || (MapView.Active == null))
                {
                    return;
                }
                else
                {
                    // await SetLayersChk_EntityTypesAsync();
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "OnMapMemberPropertiesChanged");
            }
        }

        private async Task Deactivate_EntityTypesAsync()
        {
            if ((!Fusion.m_Global.m_IsProjectFitting) || (!string.IsNullOrEmpty(m_InitActiveOnStage_EntityTypes)))
                return;

            try
            {
                if (m_IsActivated_EntityTypes)
                    m_IsActivated_EntityTypes = false;

                m_CanChangeBoard_EntityTypes = true;
                await Fusion.m_UserControl_EntityTypes.SetEntityTypesStatusAsync(true);
                await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_StateID_IsEnableEntityTypes, false);

                if (m_STMapMemberPropertiesChanged != null)
                {
                    MapMemberPropertiesChangedEvent.Unsubscribe(m_STMapMemberPropertiesChanged);
                    m_STMapMemberPropertiesChanged = null;
                }

                //Fusion.m_UserControl_EntityTypes.Dispatcher.Invoke(() =>
                //{
                //});
                Grid_EntityTypes.IsEnabled = false;

                m_HasSpecialEvents_EntityTypes = false;

                // await Task.Delay(0);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "Deactivate_EntityTypesAsync");
            }
        }

        internal async Task Activate_EntityTypesAsync(bool activate)
        {
            try
            {
                if (!activate)
                {
                    await Deactivate_EntityTypesAsync();
                }
                else
                {
                    if (!m_IsActivated_EntityTypes)
                    {
                        await InitActive_EntityTypesAsync("Activate_EntityTypesAsync");
                    }
                }

                // ActivateTool
                if ((!Fusion.m_Global.m_IsProjectFitting) || (!string.IsNullOrEmpty(m_InitActiveOnStage_EntityTypes)))
                    return;
                // (There is any coherent tool for this board)
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "Activate_EntityTypesAsync");
            }
        }

        private async Task SetDependentVisibilitiesAsync(Layer layer)
        {
            try
            {
                if (!layer.IsVisible)
                {
                    try
                    {
                        //m_SuspendSetLayersChk = true;
                        await Fusion.m_Helper_Layer.SetLayerVisAsync(layer, true);
                    }
                    finally
                    {
                        //m_SuspendSetLayersChk = false;
                    }
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "SetDependentVisibilitiesAsync");
            }
        }

        private string m_InitActiveOnStage_EntityTypes = "";
        private async Task InitActive_EntityTypesAsync(string callingRoutine)
        {
            if ((!Fusion.m_Global.m_IsProjectFitting) || (!string.IsNullOrEmpty(m_InitActiveOnStage_EntityTypes)))
                return;

            try
            {
                await Deactivate_EntityTypesAsync();
                if (m_IsActivated_EntityTypes)
                    throw new Exception("The tool is already activated!");
                m_InitActiveOnStage_EntityTypes = callingRoutine;

                if (!await Fusion.GetOneTimeRefsEntityTypesAsync())
                    return;

                m_LayerEntitiesPoints = await Fusion.GetLayerEntitiesPointsAsync();
                if (m_LayerEntitiesPoints == null)
                    return;
                if (!await Fusion.m_Helper_Layer.HasLayerFieldAsync(m_LayerEntitiesPoints, Fusion.m_FieldNameEntitiesPoints_OBJECTID, FieldType.OID))
                    return;
                if (!await Fusion.m_Helper_Layer.HasLayerFieldAsync(m_LayerEntitiesPoints, Fusion.m_FieldNameEntitiesPoints_SHAPE, FieldType.Geometry))
                    return;

                // Sichtbarkeit für die verwendeten Layers setzen.
                await SetDependentVisibilitiesAsync(m_LayerEntitiesPoints);

                // m_SuspendControlsEvents = true;

                if (m_STMapViewChanged != null)
                {
                    ActiveMapViewChangedEvent.Unsubscribe(OnMapViewChanged);
                    m_STMapViewChanged = null;
                }

                if (m_STMapMemberPropertiesChanged == null)
                {
                    // Occurs when any property of layer or standalone table changed.
                    m_STMapMemberPropertiesChanged = MapMemberPropertiesChangedEvent.Subscribe(OnMapMemberPropertiesChanged);
                }

                // ConnectionPath of override PluginDatasourceTemplate.URI.Open may be used for delivery complex parameters to proceed with tasks on tables and entries.
                // 3.3.10/20240118/msGIS_FIWARE_rt_015: Restructuring AddInX.
                Fiware_RestApi_NetHttpClient.ConnDatasource connDatasource = new Fiware_RestApi_NetHttpClient.ConnDatasource
                {
                    version = Fusion.m_Datasource_Version,
                    path = null,                                            // Empty default! - user has to know the server e.g. "https://fiwaredev.msgis.net";
                    types = Fusion.m_DatasourceTypes,
                    entities = Fusion.m_DatasourceEntities,
                    eventsource = Fusion.m_DatasourceEventsource,
                    sr_default = Fusion.m_DatasourceSR,
                    limit = Fusion.m_DatasourceLimit,
                    offset = Fusion.m_DatasourceOffset,
                    tableName = "",                                         // Empty default! - table name has to be known or selected after data query!
                };

                // Let the connection to be adopted by user.
                await PrepareConnectionStringAsync(connDatasource);

                // Set activated.
                await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_StateID_IsEnableEntityTypes, true);
                m_IsActivated_EntityTypes = true;
                Grid_EntityTypes.IsEnabled = true;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "InitActive_EntityTypesAsync");
            }
            finally
            {
                if (Fusion.m_Global.m_IsProjectFitting)
                {
                    if (m_IsActivated_EntityTypes)
                    {
                        if (m_HasSpecialEvents_EntityTypes)
                            throw new Exception("Special events are already active!");
                        m_HasSpecialEvents_EntityTypes = true;
                    }

                    if (!string.IsNullOrEmpty(m_InitActiveOnStage_EntityTypes))
                        m_InitActiveOnStage_EntityTypes = "";

                    // m_SuspendControlsEvents = false;
                }
            }
        }

        private async Task EnableControlsAsync(bool hasConn, bool hasTable)
        {
            try
            {
                bool enableCreate = hasConn && hasTable;
                ComboBox_EntityTypes.IsEnabled = hasConn;
                Button_GetEntities.IsEnabled = hasConn;
                Button_EntityToLayer.IsEnabled = enableCreate;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "EnableControlsAsync");
            }
        }
        private async Task EnableControlsAsync()
        {
            await EnableControlsAsync(false, false);
        }
        private async Task EnableControlsAsync(Fiware_RestApi_NetHttpClient.ConnDatasource connDatasource)
        {
            try
            {
                // 3.3.10/20240118/msGIS_FIWARE_rt_015: Restructuring AddInX.
                // bool enable = ((!string.IsNullOrEmpty(TextBox_ConnDs.Text));
                string connPath = await Fusion.m_Fiware_RestApi_NetHttpClient.GetConnectionPathAsync(connDatasource);
                bool hasConn = (!string.IsNullOrEmpty(connPath));
                bool hasTable = (!string.IsNullOrEmpty(connDatasource.tableName));
                await EnableControlsAsync(hasConn, hasTable);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "EnableControlsAsync");
            }
        }

        private async Task CleanEntitiesCountAsync(bool isWorking)
        {
            try
            {
                // The calling thread cannot access this object because a different thread owns it.
                if (Label_Count != null)
                {
                    Fusion.m_UserControl_EntityTypes.Dispatcher.Invoke(() =>
                    {
                        Label_Count.Content = (isWorking) ? "..." : " ";
                        Label_Count.Visibility = (isWorking) ? Visibility.Visible : Visibility.Hidden;
                    });
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "CleanEntitiesCountAsync");
            }
        }

        private async Task ShowFeaturesCountAsync(long featuresCount)
        {
            // Fusion.m_UserControl_EntityTypes
            try
            {
                // The calling thread cannot access this object because a different thread owns it.
                Fusion.m_UserControl_EntityTypes.Dispatcher.Invoke(() =>
                {
                    Label_Count.Content = $"Count = {featuresCount}";
                    Label_Count.Visibility = Visibility.Visible;
                });
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ShowFeaturesCountAsync");
            }
        }

        private async Task PrepareConnectionStringAsync(Fiware_RestApi_NetHttpClient.ConnDatasource connDatasource)
        {
            try
            {
                // Clean
                await CleanEntitiesCountAsync(false);
                await EnableControlsAsync();

                // Let the connection to be adopted by user.
                string jsonStrConnPath = await Fusion.m_Fiware_RestApi_NetHttpClient.BuildConnDsAsync(connDatasource);
                TextBox_ConnDs.Text = jsonStrConnPath;

                await EnableControlsAsync(connDatasource);

                return;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "PrepareConnectionStringAsync");
            }
        }

        private async Task<Tuple<bool, Fiware_RestApi_NetHttpClient.ConnDatasource>> ExtractConnDsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(TextBox_ConnDs.Text))
                {
                    await Fusion.m_Messages.AlertAsyncMsg("Empty datasource connection string!", "ExtractConnDsAsync");
                    return null;
                }

                return await Fusion.m_Fiware_RestApi_NetHttpClient.ExtractConnDsAsync(TextBox_ConnDs.Text);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ExtractConnDsAsync");
                return null;
            }
        }

        private async Task SetEntityTypeAsync(string entityType)
        {
            try
            {
                if (!HasComboEntityTypeSelected)
                    throw new Exception("No entity type selected!");
                if (string.IsNullOrEmpty(entityType))
                    throw new Exception("Empty entity type!");

                // Pull existing connection parameters.
                Tuple<bool, Fiware_RestApi_NetHttpClient.ConnDatasource> tupleConn = await ExtractConnDsAsync();
                if ((tupleConn == null) || (!tupleConn.Item1))
                    return;
                Fiware_RestApi_NetHttpClient.ConnDatasource connDatasource = tupleConn.Item2;
                if (connDatasource.path == null)
                    return;

                // Set table name to entity type and build the connection.
                connDatasource.tableName = entityType;

                // Let the connection to be adopted by user.
                await PrepareConnectionStringAsync(connDatasource);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "SetEntityTypeAsync");
            }
        }

        private async Task ChangeConnDsAsync()
        {
            try
            {
                await EnableControlsAsync();

                // Pull existing connection parameters.
                Tuple<bool, Fiware_RestApi_NetHttpClient.ConnDatasource> tupleConn = await ExtractConnDsAsync();
                if ((tupleConn == null) || (!tupleConn.Item1))
                    return;
                Fiware_RestApi_NetHttpClient.ConnDatasource connDatasource = tupleConn.Item2;

                await EnableControlsAsync(connDatasource);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "SetEntityTypeAsync");
            }
        }

        private async Task PopulateEntityTypesAsync(List<string> listEntityTypes)
        {
            try
            {
                ComboBox_EntityTypes.Items.Clear();
                ComboBox_EntityTypes.IsEnabled = false;

                if (listEntityTypes != null)
                {
                    foreach (object entityType in listEntityTypes)
                    {
                        ComboBox_EntityTypes.Items.Add(entityType);
                    }

                    bool enable = (ComboBox_EntityTypes.Items.Count > 0);
                    if (enable)
                    {
                        ComboBox_EntityTypes.IsEnabled = true;
                        await Fusion.m_Messages.AlertAsyncMsg($"{ComboBox_EntityTypes.Items.Count} entities found.", "Select desired entity type from combo box to modify the Datasource Connection.", "ExtractConnDsAsync");
                    }
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "PopulateEntityTypesAsync");
            }
        }

        private async Task PopulateTableNamesAsync()
        {
            try
            {
                Tuple<bool, Fiware_RestApi_NetHttpClient.ConnDatasource> tupleConn = await ExtractConnDsAsync();
                if ((tupleConn == null) || (!tupleConn.Item1))
                    return;
                Fiware_RestApi_NetHttpClient.ConnDatasource connDatasource = tupleConn.Item2;
                if (connDatasource.path == null)
                    return;

                // Get entity types.
                List<string> listEntityTypes = await Fusion.m_Fiware_RestApi_NetHttpClient.ReadEntityTypesFromRestApiAsync(connDatasource);

                // Populate combo wih entity types.
                await PopulateEntityTypesAsync(listEntityTypes);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "PopulateTableNamesAsync");
            }
        }

        #region Controls

        private async void TextBox_ConnDs_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m_IsActivated_EntityTypes)
                    await ChangeConnDsAsync();
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "TextBox_ConnDs_TextChanged");
            }
        }

        private bool HasComboEntityTypeSelected
        {
            get
            {
                try
                {
                    return ((ComboBox_EntityTypes != null) && (ComboBox_EntityTypes.Items.Count > 0) && (ComboBox_EntityTypes.SelectedIndex >= 0) && (!string.IsNullOrEmpty(ComboBox_EntityTypes.SelectedItem.ToString())));
                }
                catch (Exception ex)
                {
                    Fusion.m_Messages.PushEx(ex, m_ModuleName, "HasComboEntityTypeSelected");
                    return false;
                }
            }
        }

        private async void ComboBox_EntityTypes_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                await CleanEntitiesCountAsync(false);

                if (HasComboEntityTypeSelected)
                {
                    string entityType = ComboBox_EntityTypes.SelectedItem.ToString();
                    await SetEntityTypeAsync(entityType);
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ComboBox_EntityTypes_DropDownClosed");
            }
        }

        private async void Button_GetEntities_Click(object sender, RoutedEventArgs e)
        {
            await PopulateTableNamesAsync();
        }

        private async void Button_EntityToLayer_Click(object sender, RoutedEventArgs e)
        {
            await EntityToLayerAsync();
        }

        #endregion Controls


        #region Processing

        private async Task EntityToLayerAsync()
        {
            Helper_Progress m_Helper_Progress = null;
            try
            {
                Tuple<bool, Fiware_RestApi_NetHttpClient.ConnDatasource> tupleConn = await ExtractConnDsAsync();
                if ((tupleConn == null) || (!tupleConn.Item1))
                    return;
                Fiware_RestApi_NetHttpClient.ConnDatasource connDatasource = tupleConn.Item2;
                if (connDatasource.path == null)
                    return;

                if (string.IsNullOrEmpty(connDatasource.tableName))
                    throw new Exception("Empty table name!");

                await CleanEntitiesCountAsync(true);

                bool isProgressCancelable = false;
                m_Helper_Progress = new Helper_Progress(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework, isProgressCancelable);
                await m_Helper_Progress.ShowProgressAsync("GetEntitiesFromRestApiAsync", 900000, false);
                JArray jArrayEntities = await QueuedTask.Run(async () =>
                {
                    return await Fusion.m_Fiware_RestApi_NetHttpClient.GetEntitiesFromRestApiAsync(connDatasource);
                }, m_Helper_Progress.ProgressAssistant);

                if (jArrayEntities == null)
                    return;
                if (jArrayEntities.Count == 0)
                    await Fusion.m_Messages.AlertAsyncMsg("No entities acquired!", "EntityToLayerAsync");

                m_Helper_Progress = new Helper_Progress(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework, isProgressCancelable);
                await m_Helper_Progress.ShowProgressAsync("BuildFeaturesFromJsonEntitiesAsync", (uint)jArrayEntities.Count, true);
                List<MapPoint> listFeatures = await QueuedTask.Run(async () =>
                {
                    return await Fusion.m_Fiware_RestApi_NetHttpClient.BuildFeaturesFromJsonEntitiesAsync(jArrayEntities, connDatasource.tableName);
                }, m_Helper_Progress.ProgressAssistant);
                if (listFeatures == null)
                    return;


                // Op - Prepare
                string opName = $"BuildFeaturesFromJsonEntitiesAsync {connDatasource.tableName}";
                bool selectNewFeatures = false;
                EditOperation editOperation = await Fusion.m_Helper_Op.PrepareOpAsync(opName, selectNewFeatures);
                if (editOperation == null)
                    return;

                string fieldId = $"{Fusion.m_FieldNameEntitiesPoints_OBJECTID}";
                string subFields = $"{Fusion.m_FieldNameEntitiesPoints_OBJECTID}";
                List<Object> listIds = await Fusion.m_Helper_Search.GetLayerFieldValuesAsync(m_LayerEntitiesPoints, fieldId, subFields);
                List<long> listIdsLong = new List<long>();
                foreach (object id in listIds)
                {
                    if (id != null)
                    {
                        long idLong = Convert.ToInt64(id);
                        listIdsLong.Add(idLong);
                    }
                }
                IEnumerable<long> oids = listIdsLong;
                editOperation.Delete(m_LayerEntitiesPoints, oids);

                // EntitiesToFeatures
                foreach (MapPoint mapPoint in listFeatures)
                {
                    editOperation.Create(m_LayerEntitiesPoints, mapPoint);
                }

                if (!await Fusion.m_Helper_Op.ExecOpAsync(editOperation))
                    return;

                await ShowFeaturesCountAsync(listFeatures.Count);
                await Fusion.m_Messages.AlertAsyncMsg($"{connDatasource.tableName} was exported to Layer.", m_LayerEntitiesPoints.Name, "Entities --> Layer Features");
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "EntityToLayerAsync");
            }
            finally
            {
                if (m_Helper_Progress != null)
                {
                    await m_Helper_Progress.FinishProgressAsync();
                    m_Helper_Progress = null;
                }
            }
        }

        #endregion Processing

    }
}
