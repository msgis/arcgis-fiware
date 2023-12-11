using msGIS.ProApp_Common_FIWARE_3x;

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
using msGIS.ProPluginDatasource_FIWARE;

namespace msGIS.ProApp_FiwareSummit
{
    internal class Spring_EntityTypes
    {
        private readonly string m_ModuleName = "Spring_EntityTypes";

        private Grid Grid_EntityTypes;
        private ComboBox ComboBox_EntityTypes;
        private Label Label_Count;
        private Button Button_EntityToLayer;
        private Button Button_EntityToCSV;
        private Button Button_Datasource;
        private Button Button_CsvToLayer;

        private Layer m_LayerEntitiesPoints = null;

        private SubscriptionToken m_STMapViewChanged = null;
        private SubscriptionToken m_STMapMemberPropertiesChanged = null;

        private bool m_IsActivated_EntityTypes = false;

        internal bool m_CanChangeBoard_EntityTypes = true;
        // private bool m_SuspendControlsEvents = false;
        //private bool m_SuspendSetLayersChk = false;
        private bool m_HasSpecialEvents_EntityTypes = false;

        internal Spring_EntityTypes(Grid grid_EntityTypes, ComboBox comboBox_EntityTypes, Label label_Count, Button button_EntityToLayer, Button button_EntityToCSV, Button button_Datasource, Button button_CsvToLayer)
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

            Button_EntityToLayer = button_EntityToLayer;
            Button_EntityToLayer.IsEnabled = false;
            Button_EntityToLayer.Click += Button_EntityToLayer_Click;

            Button_EntityToCSV = button_EntityToCSV;
            Button_EntityToCSV.IsEnabled = false;
            Button_EntityToCSV.Click += Button_EntityToCSV_Click;

            Button_Datasource = button_Datasource;
            Button_Datasource.IsEnabled = true;
            Button_Datasource.Click += Button_Datasource_Click;

            Button_CsvToLayer = button_CsvToLayer;
            Button_CsvToLayer.IsEnabled = false;
            Button_CsvToLayer.Click += Button_CsvToLayer_Click;

            _ = CleanEntitiesCountAsync(false);
            Button_CsvToLayer = button_CsvToLayer;
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

                // Get entity types from JSON.
                List<string> listEntityTypes = await RestApi_Fiware.ReadEntityTypesFromRestApiAsync(Fusion.m_DatasourcePath);

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
                Label_Count.Content = (isWorking) ? "..." : " ";
                Label_Count.Visibility = (isWorking) ? Visibility.Visible : Visibility.Hidden;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "CleanEntitiesCountAsync");
            }
        }

        private async Task ShowCountAsync(int entitiesCount)
        {
            // Fusion.m_UserControl_EntityTypes
            try
            {
                Label_Count.Content = $"{entitiesCount}";
                Label_Count.Visibility = Visibility.Visible;
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
                Button_EntityToCSV.IsEnabled = false;
                Button_CsvToLayer.IsEnabled = false;
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

        private bool HasComboEntityTypeSelected
        {
            get
            {
                return ((ComboBox_EntityTypes != null) && (ComboBox_EntityTypes.Items.Count > 0) && (ComboBox_EntityTypes.SelectedIndex >= 0));
            }
        }


        private async Task EntitiesToFeaturesAsync()
        {
            Helper_Progress m_Helper_Progress = null;
            try
            {
                if (!HasComboEntityTypeSelected)
                    throw new Exception("No entity type selected!");
                if (m_LayerEntitiesPoints == null)
                    throw new Exception($"Layer {Fusion.m_LayerTagEntitiesPoints} is not acquired!");
                string entityType = ComboBox_EntityTypes.SelectedItem.ToString();
                if (string.IsNullOrEmpty(entityType))
                    throw new Exception("Empty entity type!");

                await CleanEntitiesCountAsync(true);

                bool isProgressCancelable = false;
                m_Helper_Progress = new Helper_Progress(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework, isProgressCancelable);
                await m_Helper_Progress.ShowProgressAsync("GetEntitiesFromRestApiAsync", 900000, false);
                JArray jArrayEntities = await QueuedTask.Run(async () =>
                {
                    return await RestApi_Fiware.GetEntitiesFromRestApiAsync(Fusion.m_DatasourcePath, entityType);
                }, m_Helper_Progress.ProgressAssistant);

                if (jArrayEntities == null)
                    return;
                if (jArrayEntities.Count == 0)
                    await Fusion.m_Messages.AlertAsyncMsg("No entities acquired!", "EntitiesToFeaturesAsync");
                await ShowCountAsync(jArrayEntities.Count);

                m_Helper_Progress = new Helper_Progress(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework, isProgressCancelable);
                await m_Helper_Progress.ShowProgressAsync("BuildFeaturesFromJsonEntitiesAsync", (uint)jArrayEntities.Count, true);
                List<MapPoint> listFeatures = await QueuedTask.Run(async () =>
                {
                    return await RestApi_Fiware.BuildFeaturesFromJsonEntitiesAsync(jArrayEntities);
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

                await Fusion.m_Messages.AlertAsyncMsg($"{entityType} was exported to Layer.", m_LayerEntitiesPoints.Name, "Entities --> Layer Features");
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "EntitiesToFeaturesAsync");
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

        private async Task EntitiesToCsvAsync()
        {
            Helper_Progress m_Helper_Progress = null;
            try
            {
                if (!HasComboEntityTypeSelected)
                    throw new Exception("No entity type selected!");
                if (m_LayerEntitiesPoints == null)
                    throw new Exception($"Layer {Fusion.m_LayerTagEntitiesPoints} is not acquired!");
                string entityType = ComboBox_EntityTypes.SelectedItem.ToString();
                if (string.IsNullOrEmpty(entityType))
                    throw new Exception("Empty entity type!");

                await CleanEntitiesCountAsync(true);

                bool isProgressCancelable = false;
                m_Helper_Progress = new Helper_Progress(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework, isProgressCancelable);
                await m_Helper_Progress.ShowProgressAsync("GetEntitiesFromRestApiAsync", 900000, false);
                JArray jArrayEntities = await QueuedTask.Run(async () =>
                {
                    return await RestApi_Fiware.GetEntitiesFromRestApiAsync(Fusion.m_DatasourcePath, entityType);
                }, m_Helper_Progress.ProgressAssistant);

                if (jArrayEntities == null)
                    return;
                if (jArrayEntities.Count == 0)
                    await Fusion.m_Messages.AlertAsyncMsg("No entities acquired!", "EntitiesToCsvAsync");
                await ShowCountAsync(jArrayEntities.Count);

                m_Helper_Progress = new Helper_Progress(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework, isProgressCancelable);
                await m_Helper_Progress.ShowProgressAsync("BuildFeaturesFromJsonEntitiesAsync", (uint)jArrayEntities.Count, true);
                List<MapPoint> listFeatures = await QueuedTask.Run(async () =>
                {
                    return await RestApi_Fiware.BuildFeaturesFromJsonEntitiesAsync(jArrayEntities);
                }, m_Helper_Progress.ProgressAssistant);
                if (listFeatures == null)
                    return;

                // 3.3.05/20231205/msGIS_FIWARE_rt_003: EntitiesToCsv for use with SimplePointPlugin.
                string filePathCsv = Path.Combine(Fusion.m_PathCsvEntities, $"{entityType}.csv");
                if (File.Exists(filePathCsv))
                {
                    if (!await Fusion.m_Messages.MsAskAsync($"File already exists!{Environment.NewLine}{filePathCsv}{Environment.NewLine}Overwrite CSV?", "Entities --> CSV"))
                        return;
                    File.Delete(filePathCsv);
                }
                // FileStream fileStreamCsv = File.Create(filePathCsv);

                using (StreamWriter writer = new StreamWriter(filePathCsv))
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

                await Fusion.m_Messages.AlertAsyncMsg($"{entityType} was exported to CSV.", filePathCsv, "Entities --> CSV");
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "EntitiesToCsvAsync");
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

        private async void ComboBox_EntityTypes_DropDownClosed(object sender, EventArgs e)
        {
            await CleanEntitiesCountAsync(false);
            Button_EntityToLayer.IsEnabled = HasComboEntityTypeSelected;
            Button_EntityToCSV.IsEnabled = HasComboEntityTypeSelected;
            Button_CsvToLayer.IsEnabled = HasComboEntityTypeSelected;
        }

        private async void Button_EntityToLayer_Click(object sender, RoutedEventArgs e)
        {
            await EntitiesToFeaturesAsync();
        }

        private async void Button_EntityToCSV_Click(object sender, RoutedEventArgs e)
        {
            await EntitiesToCsvAsync();
        }

        private async void Button_CsvToLayer_Click(object sender, RoutedEventArgs e)
        {
            await CsvToLayerAsync();
        }

        private async void Button_Datasource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
                // 3.3.05/20231201/msGIS_FIWARE_rt_002: Nicht überwindbare Komplikation auf HttpClient mittels GetAsync(apiUrl) aus der abstrakten Klasse ArcPro PluginDatasourceTemplate zuzugreifen.
                // 3.3.05/20231206/msGIS_FIWARE_rt_004: Expertise FIWARE Integration ArcGIS PRO.

                bool evalPluginTables = false;
                if (evalPluginTables)
                {
                    await Fusion.m_Messages.MsNotImplementedAsync();

                    if (!await ProPluginDatasource_FIWARE.Fusion.InitAsync(Fusion.m_DatasourcePath))
                        return;

                    // Types: /ngsi-ld/v1/types
                    // Entities: /ngsi-ld/v1/entities?type={entityType}&offset={offset}&limit={limit}
                    // Refresh: /ngsi-proxy/eventsource/e9e01390-fae3-11ed-926f-1bdc1977e2d3
                    Uri uriDatasourcePath = new Uri(Fusion.m_DatasourcePath);

                    await QueuedTask.Run(() =>
                    {
                        // PluginDatasourceConnectionPath : Connector
                        // Plugin identifier is corresponding to ProPluginDatasource Config.xml PluginDatasource ID
                        using (PluginDatastore pluginDatastore = new PluginDatastore(
                         new PluginDatasourceConnectionPath(Fusion.m_ProPluginDatasourceID_Entities, uriDatasourcePath)))
                        {
                            if (pluginDatastore != null)
                            {
                                IReadOnlyList<string> tableNames = pluginDatastore.GetTableNames();
                                if (tableNames != null)
                                {
                                    foreach (var table_name in tableNames)
                                    {
                                        System.Diagnostics.Debug.Write($"Table: {table_name}\r\n");
                                        //open each table....use the returned table name
                                        //or just pass in the name of a csv file in the workspace folder
                                        using (var table = pluginDatastore.OpenTable(table_name))
                                        {
                                            //get information about the table
                                            using (var def = table.GetDefinition() as FeatureClassDefinition)
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });

                }

                // 3.3.05/20231207/msGIS_FIWARE_rt_005: ProPluginDatasource integration for SimplePoint CSV.
                bool evalCSV = true;
                if (!evalCSV)
                {
                    await Fusion.m_Messages.AlertAsyncMsg("Use SimplePointPlugin to evaluate CSV Datastore!", "Pro Datasource");
                    return;
                }
                else
                {
                    string csv_path = Fusion.m_PathCsvEntities;

                    await QueuedTask.Run(() =>
                    {

                        using (PluginDatastore pluginws = new PluginDatastore(
                             new PluginDatasourceConnectionPath(Fusion.m_ProPluginDatasourceID_Entities,
                                   new Uri(csv_path, UriKind.Absolute))))
                        {
                            System.Diagnostics.Debug.Write("==========================\r\n");
                            foreach (var table_name in pluginws.GetTableNames())
                            {
                                System.Diagnostics.Debug.Write($"Table: {table_name}\r\n");
                                //open each table....use the returned table name
                                //or just pass in the name of a csv file in the workspace folder
                                using (var table = pluginws.OpenTable(table_name))
                                {
                                    //get information about the table
                                    using (var def = table.GetDefinition() as FeatureClassDefinition)
                                    {

                                    }
                                    //query and return all rows
                                    //TODO - use a QueryFilter and Whereclause
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
                                }
                            }
                        }


                    });

                    await Fusion.m_Messages.AlertAsyncMsg($"ArcGIS Pro Datasource Framework has been prepared to support CSV type.", $"{Fusion.m_ProPluginDatasourceID_Entities}{Environment.NewLine}{csv_path}", "Plugin Datasource CSV");
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "Button_Datasource_Click");
            }
        }

        private async Task CsvToLayerAsync()
        {
            try
            {
                if (!HasComboEntityTypeSelected)
                    throw new Exception("No entity type selected!");
                if (m_LayerEntitiesPoints == null)
                    throw new Exception($"Layer {Fusion.m_LayerTagEntitiesPoints} is not acquired!");
                string entityType = ComboBox_EntityTypes.SelectedItem.ToString();
                if (string.IsNullOrEmpty(entityType))
                    throw new Exception("Empty entity type!");

                // 3.3.05/20231207/msGIS_FIWARE_rt_005: ProPluginDatasource integration for SimplePoint CSV.
                string csv_path = Fusion.m_PathCsvEntities;

                await QueuedTask.Run(async() =>
                {
                    using (PluginDatastore pluginws = new PluginDatastore(
                         new PluginDatasourceConnectionPath(Fusion.m_ProPluginDatasourceID_Entities,
                               new Uri(csv_path, UriKind.Absolute))))
                    {
                        //open each table....use the returned table name
                        /*
                        System.Diagnostics.Debug.Write("==========================\r\n");
                        foreach (string table_name in pluginws.GetTableNames())
                        {
                        }
                        */

                        //or just pass in the name of a csv file in the workspace folder
                        string table_name = $"{entityType}.csv";
                        string table_path = Path.Combine(csv_path, table_name);
                        if (!File.Exists(table_path))
                        {
                            await Fusion.m_Messages.AlertAsyncMsg($"Entity data not found!", table_path, "CSV-Layer");
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
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "CsvToLayerAsync");
            }
        }

    }
}
