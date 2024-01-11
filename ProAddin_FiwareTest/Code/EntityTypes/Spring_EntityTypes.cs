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

namespace msGIS.ProApp_FiwareTest
{
    internal class Spring_EntityTypes
    {
        // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
        // 3.3.05/20231206/msGIS_FIWARE_rt_004: Expertise FIWARE Integration ArcGIS PRO.
        private readonly string m_ModuleName = "Spring_EntityTypes";

        private Grid Grid_EntityTypes;
        private ComboBox ComboBox_EntityTypes;
        private Label Label_Count;
        private Button Button_EntityToLayer;
        private TextBox TextBox_DataPath;
        private Button Button_EntityToFile;
        private Button Button_FileToDsfLayer;
        private Button Button_EntityToDsfLayer;

        private Layer m_LayerEntitiesPoints = null;

        private SubscriptionToken m_STMapViewChanged = null;
        private SubscriptionToken m_STMapMemberPropertiesChanged = null;

        private bool m_IsActivated_EntityTypes = false;

        // 3.3.06/20231221/msGIS_FIWARE_rt_008: Datasource URI.
        private Fiware_RestApi_NetHttpClient.UriDatasource m_UriDatasource;

        internal bool m_CanChangeBoard_EntityTypes = true;
        // private bool m_SuspendControlsEvents = false;
        //private bool m_SuspendSetLayersChk = false;
        private bool m_HasSpecialEvents_EntityTypes = false;

        internal Spring_EntityTypes(Grid grid_EntityTypes, ComboBox comboBox_EntityTypes, Label label_Count, Button button_EntityToLayer,
            TextBox textBox_DataPath, Button button_EntityToFile, Button button_FileToDsfLayer, Button button_EntityToDsfLayer)
        {
            Grid_EntityTypes = grid_EntityTypes;
            Grid_EntityTypes.IsEnabled = false;

            if (m_STMapViewChanged == null)
                m_STMapViewChanged = ActiveMapViewChangedEvent.Subscribe(OnMapViewChanged);

            if (MapView.Active != null)
            {
                _ = InitActive_EntityTypesAsync("Spring_EntityTypes");
            }

            ComboBox_EntityTypes = comboBox_EntityTypes;
            ComboBox_EntityTypes.DropDownClosed += ComboBox_EntityTypes_DropDownClosed;
            Label_Count = label_Count;
            _ = CleanEntitiesCountAsync(false);

            Button_EntityToLayer = button_EntityToLayer;
            Button_EntityToLayer.IsEnabled = false;
            Button_EntityToLayer.Click += Button_EntityToLayer_Click;

            TextBox_DataPath = textBox_DataPath;
            TextBox_DataPath.IsEnabled = false;
            // TextBox_DataPath.TextChanged
            TextBox_DataPath.Text = Fusion.m_PathEntities;

            Button_EntityToFile = button_EntityToFile;
            Button_EntityToFile.IsEnabled = false;
            Button_EntityToFile.Click += Button_EntityToFile_Click;

            Button_FileToDsfLayer = button_FileToDsfLayer;
            Button_FileToDsfLayer.IsEnabled = false;
            Button_FileToDsfLayer.Click += Button_FileToDsfLayer_Click;

            Button_EntityToDsfLayer = button_EntityToDsfLayer;
            Button_EntityToDsfLayer.IsEnabled = false;
            Button_EntityToDsfLayer.Click += Button_EntityToDsfLayer_Click;
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

                // 3.3.06/20231221/msGIS_FIWARE_rt_008: Datasource URI.
                // 3.3.08/20240109/msGIS_FIWARE_rt_012: Init Fiware_RestApi_NetHttpClient before Plugin Datasource OpenTable/GetTableNames.
                // 3.3.09/20240110/msGIS_FIWARE_rt_014: Configurable URI.
                m_UriDatasource = new Fiware_RestApi_NetHttpClient.UriDatasource
                {
                    path = new Uri(Fusion.m_DatasourcePath, UriKind.Absolute),
                    types = Fusion.m_DatasourceTypes,
                    entities = Fusion.m_DatasourceEntities,
                    eventsource = Fusion.m_DatasourceEventsource,
                    spatialReference = SpatialReferences.WGS84
                };
                // Get entity types from JSON.
                List<string> listEntityTypes = await Fusion.m_Fiware_RestApi_NetHttpClient.ReadEntityTypesFromRestApiAsync(m_UriDatasource);

                // Populate combo wih entity types.
                await PopulateEntityTypesAsync(listEntityTypes);

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

        private async Task CleanEntitiesCountAsync(bool isWorking)
        {
            try
            {
                // The calling thread cannot access this object because a different thread owns it.
                Fusion.m_UserControl_EntityTypes.Dispatcher.Invoke(() =>
                {
                    Label_Count.Content = (isWorking) ? "..." : " ";
                    Label_Count.Visibility = (isWorking) ? Visibility.Visible : Visibility.Hidden;
                });
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "CleanEntitiesCountAsync");
            }
        }

        private async Task ShowCountAsync(long entitiesCount)
        {
            // Fusion.m_UserControl_EntityTypes
            try
            {
                // The calling thread cannot access this object because a different thread owns it.
                Fusion.m_UserControl_EntityTypes.Dispatcher.Invoke(() =>
                {
                    Label_Count.Content = $"{entitiesCount}";
                    Label_Count.Visibility = Visibility.Visible;
                });
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ShowCountAsync");
            }
        }

        private async Task CleanEntitiesAsync()
        {
            try
            {
                ComboBox_EntityTypes.Items.Clear();

                Button_EntityToLayer.IsEnabled = false;
                TextBox_DataPath.IsEnabled = false;
                Button_EntityToFile.IsEnabled = false;
                Button_FileToDsfLayer.IsEnabled = false;
                Button_EntityToDsfLayer.IsEnabled = false;
                await CleanEntitiesCountAsync(false);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "CleanEntitiesAsync");
            }
        }

        private async Task PopulateEntityTypesAsync(List<string> listEntityTypes)
        {
            try
            {
                await CleanEntitiesAsync();

                if (listEntityTypes != null)
                {
                    foreach (object entityType in listEntityTypes)
                    {
                        ComboBox_EntityTypes.Items.Add(entityType);
                    }
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "PopulateEntityTypesAsync");
            }
        }

        #region Controls

        private bool HasComboEntityTypeSelected
        {
            get
            {
                return ((ComboBox_EntityTypes != null) && (ComboBox_EntityTypes.Items.Count > 0) && (ComboBox_EntityTypes.SelectedIndex >= 0) && (!string.IsNullOrEmpty(ComboBox_EntityTypes.SelectedItem.ToString())));
            }
        }

        private async void ComboBox_EntityTypes_DropDownClosed(object sender, EventArgs e)
        {
            await CleanEntitiesCountAsync(false);
            Button_EntityToLayer.IsEnabled = HasComboEntityTypeSelected;
            TextBox_DataPath.IsEnabled = HasComboEntityTypeSelected;
            Button_EntityToFile.IsEnabled = HasComboEntityTypeSelected;
            Button_FileToDsfLayer.IsEnabled = HasComboEntityTypeSelected;
            Button_EntityToDsfLayer.IsEnabled = HasComboEntityTypeSelected;
        }

        private async void Button_EntityToLayer_Click(object sender, RoutedEventArgs e)
        {
            await EntityToLayerAsync();
        }

        private async void Button_EntityToFile_Click(object sender, RoutedEventArgs e)
        {
            await EntityToFileAsync();
        }

        private async void Button_FileToDsfLayer_Click(object sender, RoutedEventArgs e)
        {
            await FileToDsfLayerAsync();
        }

        private async void Button_EntityToDsfLayer_Click(object sender, RoutedEventArgs e)
        {
            await EntityToDsfLayerAsync();
        }

        #endregion Controls


        #region Processing

        private async Task EntityToLayerAsync()
        {
            Helper_Progress m_Helper_Progress = null;
            try
            {
                if (!HasComboEntityTypeSelected)
                    throw new Exception("No entity type selected!");
                string entityType = ComboBox_EntityTypes.SelectedItem.ToString();
                if (string.IsNullOrEmpty(entityType))
                    throw new Exception("Empty entity type!");

                await CleanEntitiesCountAsync(true);

                bool isProgressCancelable = false;
                m_Helper_Progress = new Helper_Progress(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework, isProgressCancelable);
                await m_Helper_Progress.ShowProgressAsync("GetEntitiesFromRestApiAsync", 900000, false);
                JArray jArrayEntities = await QueuedTask.Run(async () =>
                {
                    return await Fusion.m_Fiware_RestApi_NetHttpClient.GetEntitiesFromRestApiAsync(m_UriDatasource, entityType);
                }, m_Helper_Progress.ProgressAssistant);

                if (jArrayEntities == null)
                    return;
                if (jArrayEntities.Count == 0)
                    await Fusion.m_Messages.AlertAsyncMsg("No entities acquired!", "EntityToLayerAsync");

                m_Helper_Progress = new Helper_Progress(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework, isProgressCancelable);
                await m_Helper_Progress.ShowProgressAsync("BuildFeaturesFromJsonEntitiesAsync", (uint)jArrayEntities.Count, true);
                List<MapPoint> listFeatures = await QueuedTask.Run(async () =>
                {
                    return await Fusion.m_Fiware_RestApi_NetHttpClient.BuildFeaturesFromJsonEntitiesAsync(jArrayEntities);
                }, m_Helper_Progress.ProgressAssistant);
                if (listFeatures == null)
                    return;


                // Op - Prepare
                string opName = $"BuildFeaturesFromJsonEntitiesAsync {entityType}";
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

                await ShowCountAsync(listFeatures.Count);
                await Fusion.m_Messages.AlertAsyncMsg($"{entityType} was exported to Layer.", m_LayerEntitiesPoints.Name, "Entities --> Layer Features");
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

        private async Task EntityToFileAsync()
        {
            // 3.3.05/20231205/msGIS_FIWARE_rt_003: EntitiesToFile for use with SimplePointPlugin.
            Helper_Progress m_Helper_Progress = null;
            try
            {
                if (!HasComboEntityTypeSelected)
                    throw new Exception("No entity type selected!");
                string entityType = ComboBox_EntityTypes.SelectedItem.ToString();
                if (string.IsNullOrEmpty(entityType))
                    throw new Exception("Empty entity type!");

                await CleanEntitiesCountAsync(true);

                bool isProgressCancelable = false;
                m_Helper_Progress = new Helper_Progress(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework, isProgressCancelable);
                await m_Helper_Progress.ShowProgressAsync("GetEntitiesFromRestApiAsync", 900000, false);
                JArray jArrayEntities = await QueuedTask.Run(async () =>
                {
                    return await Fusion.m_Fiware_RestApi_NetHttpClient.GetEntitiesFromRestApiAsync(m_UriDatasource, entityType);
                }, m_Helper_Progress.ProgressAssistant);

                if (jArrayEntities == null)
                    return;
                if (jArrayEntities.Count == 0)
                    await Fusion.m_Messages.AlertAsyncMsg("No entities acquired!", "EntityToFileAsync");

                m_Helper_Progress = new Helper_Progress(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework, isProgressCancelable);
                await m_Helper_Progress.ShowProgressAsync("BuildFeaturesFromJsonEntitiesAsync", (uint)jArrayEntities.Count, true);
                List<MapPoint> listFeatures = await QueuedTask.Run(async () =>
                {
                    return await Fusion.m_Fiware_RestApi_NetHttpClient.BuildFeaturesFromJsonEntitiesAsync(jArrayEntities);
                }, m_Helper_Progress.ProgressAssistant);
                if (listFeatures == null)
                    return;

                string dirPath = TextBox_DataPath.Text;
                if (!Directory.Exists(dirPath))
                {
                    await Fusion.m_Messages.AlertAsyncMsg($"Datasource path not found!", dirPath, "Pro Datasource");
                    return;
                }
                string filePath = Path.Combine(dirPath, $"{entityType}.{ProPluginDatasource_EntityFile.Fusion.m_FileSuffix}");
                if (File.Exists(filePath))
                {
                    if (!await Fusion.m_Messages.MsAskAsync($"File already exists!{Environment.NewLine}{filePath}{Environment.NewLine}Overwrite File?", "Entities --> File"))
                        return;
                    File.Delete(filePath);
                }
                // FileStream fileStream = File.Create(filePath);

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write headers
                    writer.WriteLine("POINT_X,POINT_Y,NAME");

                    // Write data entries
                    int ind = 0;
                    foreach (MapPoint mapPoint in listFeatures)
                    {
                        writer.WriteLine($"{Convert.ToString(mapPoint.X, CultureInfo.InvariantCulture)},{Convert.ToString(mapPoint.Y, CultureInfo.InvariantCulture)},{ind}");
                        ind++;
                    }
                }

                await ShowCountAsync(listFeatures.Count);
                await Fusion.m_Messages.AlertAsyncMsg($"{entityType} was exported to File.", filePath, "Entities --> File");
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "EntityToFileAsync");
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

        private async Task FileToDsfLayerAsync()
        {
            try
            {
                // 3.3.05/20231207/msGIS_FIWARE_rt_005: ProPluginDatasource Integration for SimplePoint File-Format.
                if (!HasComboEntityTypeSelected)
                    throw new Exception("No entity type selected!");
                string entityType = ComboBox_EntityTypes.SelectedItem.ToString();
                if (string.IsNullOrEmpty(entityType))
                    throw new Exception("Empty entity type!");

                string dirPath = TextBox_DataPath.Text;
                if (!Directory.Exists(dirPath))
                {
                    await Fusion.m_Messages.AlertAsyncMsg($"Datasource path not found!", dirPath, "File-Layer");
                    return;
                }

                await QueuedTask.Run(async() =>
                {
                    using (PluginDatastore pluginws = new PluginDatastore(
                         new PluginDatasourceConnectionPath(Fusion.m_ProPluginDatasourceID_EntityFile,
                               new Uri(dirPath, UriKind.Absolute))))
                    {
                        //open each table....use the returned table name
                        /*
                        System.Diagnostics.Debug.Write("==========================\r\n");
                        foreach (string table_name in pluginws.GetTableNames())
                        {
                        }
                        */

                        //or just pass in the name of a csv file in the workspace folder
                        string table_name = $"{entityType}.{ProPluginDatasource_EntityFile.Fusion.m_FileSuffix}";
                        string table_path = Path.Combine(dirPath, table_name);
                        if (!File.Exists(table_path))
                        {
                            await Fusion.m_Messages.AlertAsyncMsg($"Entity data not found!", table_path, "File-Layer");
                        }
                        else
                        {
                            using (var table = pluginws.OpenTable(table_name))
                            {
                                //Add as a layer to the active map or scene
                                LayerFactory.Instance.CreateLayer<FeatureLayer>(new FeatureLayerCreationParams((FeatureClass)table), MapView.Active.Map);
                            }
                        }
                    }

                });


            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "FileToDsfLayerAsync");
            }
        }

        private async Task EntityToDsfLayerAsync()
        {
            try
            {
                // 3.3.05/20231207/msGIS_FIWARE_rt_005: ProPluginDatasource Integration for SimplePoint File-Format.
                // await Fusion.m_Messages.MsNotImplementedAsync();

                if (!HasComboEntityTypeSelected)
                    throw new Exception("No entity type selected!");
                string entityType = ComboBox_EntityTypes.SelectedItem.ToString();
                if (string.IsNullOrEmpty(entityType))
                    throw new Exception("Empty entity type!");

                await CleanEntitiesCountAsync(true);

                // 3.3.06/20231221/msGIS_FIWARE_rt_008: Datasource URI.
                // 3.3.08/20240109/msGIS_FIWARE_rt_012: Init Fiware_RestApi_NetHttpClient before Plugin Datasource OpenTable/GetTableNames.
                // types: /ngsi-ld/v1/types
                // entities: /ngsi-ld/v1/entities?type={entityType}&offset={offset}&limit={limit}
                // eventsource: /ngsi-proxy/eventsource/e9e01390-fae3-11ed-926f-1bdc1977e2d3
                // ConnectionPath of override PluginDatasourceTemplate.URI.Open is not suitable for FIWARE due to complexly build URI with parameters set while proceeding tasks on tables and entries.
                if (!await ProPluginDatasource_FiwareHttpClient.Fusion.InitAsync())
                    return;

                // 3.3.09/20240110/msGIS_FIWARE_rt_014: Configurable URI.
                Uri connectionPath = m_UriDatasource.path;
                connectionPath = new Uri(connectionPath.OriginalString, UriKind.Absolute);
                connectionPath = new Uri($"{connectionPath.OriginalString}/TableName={entityType}", UriKind.Absolute);


                await QueuedTask.Run(async() =>
                {
                    // PluginDatasourceConnectionPath : Connector
                    // Plugin identifier is corresponding to ProPluginDatasource Config.xml PluginDatasource ID
                    using (PluginDatastore pluginDatastore = new PluginDatastore(
                     new PluginDatasourceConnectionPath(Fusion.m_ProPluginDatasourceID_FiwareHttpClient, connectionPath)))
                    {
                        if (pluginDatastore != null)
                        {
                            IReadOnlyList<string> tableNames = pluginDatastore.GetTableNames();
                            if ((tableNames != null) && (tableNames.Count > 0))
                            {
                                // await ShowCountAsync(tableNames.Count);
                                bool isDeveloping = false;
                                if (isDeveloping)
                                {
                                    string tables = "";
                                    foreach (var table_name in tableNames)
                                    {
                                        if (!string.IsNullOrEmpty(tables))
                                            tables += Environment.NewLine;
                                        tables += table_name;
                                    }
                                    await Fusion.m_Messages.AlertAsyncMsg(tables, "GetTableNames");
                                    await Fusion.m_Messages.MsNotImplementedAsync();
                                    return;
                                }

                                foreach (var table_name in tableNames)
                                {
                                    System.Diagnostics.Debug.Write($"Table: {table_name}\r\n");
                                    // open each table....use the returned table name
                                    // or just pass in the name of a table in the workspace folder
                                    if (table_name != entityType)
                                        continue;

                                    // 3.3.07/20231222/msGIS_FIWARE_rt_010: Open Plugin table and read the data.
                                    using (var table = pluginDatastore.OpenTable(table_name))
                                    {
                                        if (table == null)
                                            throw new Exception("Empty table!");
                                        if (table.Type != DatasetType.FeatureClass)
                                            throw new Exception("Table type is not a feature class!");

                                        // get information about the table
                                        using (var def = table.GetDefinition() as FeatureClassDefinition)
                                        {
                                            if (def == null)
                                                throw new Exception("Empty table definition!");
                                            if (def.DatasetType != DatasetType.FeatureClass)
                                                throw new Exception("Dataset type is not a feature class!");
                                            string name = def.GetName();
                                            string aliasName = def.GetAliasName();
                                        }

                                        // query and return all rows
                                        // TODO - use a QueryFilter and Whereclause
                                        //var qf = new QueryFilter()
                                        //{
                                        //  WhereClause = "OBJECTID > 0"
                                        //};
                                        //var rc = table.Search(qf);

                                        using (var rc = table.Search(null))
                                        {
                                            while (rc.MoveNext())
                                            {
                                                using (var feat = rc.Current as Feature)
                                                {
                                                    if (feat != null)
                                                    {
                                                        //Get information from the feature
                                                        var oid = feat.GetObjectID();
                                                        var shape = feat.GetShape();

                                                        //Access all the values
                                                        var count = feat.GetFields().Count();
                                                        for (int i = 0; i < count; i++)
                                                        {
                                                            var val = feat[i];
                                                            //TODO use the value(s)
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        long rowsCount = table.GetCount();
                                        await ShowCountAsync(rowsCount);

                                        // Add as a layer to the active map or scene
                                        LayerFactory.Instance.CreateLayer<FeatureLayer>(new FeatureLayerCreationParams((FeatureClass)table), MapView.Active.Map);
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "EntityToDsfLayerAsync");
            }
        }

        #endregion Processing

    }
}
