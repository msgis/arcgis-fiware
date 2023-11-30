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
        private readonly string m_ModuleName = "Spring_EntityTypes";

        private Grid Grid_EntityTypes;
        private ComboBox ComboBox_EntityTypes;
        private Button Button_EntityToLayer;
        private Button Button_Datasource;
        private Label Label_Count;

        private Layer m_LayerEntitiesPoints = null;

        private SubscriptionToken m_STMapViewChanged = null;
        private SubscriptionToken m_STMapMemberPropertiesChanged = null;

        private bool m_IsActivated_EntityTypes = false;
        private bool m_IsInitialized_ProPluginDatasource = false;

        internal bool m_CanChangeBoard_EntityTypes = true;
        // private bool m_SuspendControlsEvents = false;
        //private bool m_SuspendSetLayersChk = false;
        private bool m_HasSpecialEvents_EntityTypes = false;

        internal Spring_EntityTypes(Grid grid_EntityTypes, ComboBox comboBox_EntityTypes, Button button_EntityToLayer, Button button_Datasource, Label label_Count)
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
            Button_EntityToLayer = button_EntityToLayer;
            Button_Datasource = button_Datasource;
            ComboBox_EntityTypes.DropDownClosed += ComboBox_EntityTypes_DropDownClosed;

            Button_EntityToLayer.IsEnabled = false;
            Button_EntityToLayer.Click += Button_EntityToLayer_Click;

            Button_Datasource.IsEnabled = true;
            Button_Datasource.Click += Button_Datasource_Click;

            Label_Count = label_Count;
            _ = CleanEntitiesCountAsync(false);
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
                List<object> listEntityTypes = await RestApi_Entities.ReadSettingsFromRestApiAsync();

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
                Label_Count.Content = (isWorking) ? "Anzahl = ..." : " ";
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
                Label_Count.Content = $"Anzahl = {entitiesCount}";
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
                await CleanEntitiesCountAsync(false);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "CleanEntitiesAsync");
            }
        }

        private async Task PopulateEntityTypesAsync(List<object> listEntityTypes)
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

                await RestApi_Entities.StopUpdateAsync();

                bool isProgressCancelable = false;
                m_Helper_Progress = new Helper_Progress(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework, isProgressCancelable);
                await m_Helper_Progress.ShowProgressAsync("GetEntitiesFromRestApiAsync", 900000, false);
                JArray jArrayEntities = await QueuedTask.Run(async () =>
                {
                    return await RestApi_Entities.GetEntitiesFromRestApiAsync(entityType);
                }, m_Helper_Progress.ProgressAssistant);

                if (jArrayEntities == null)
                    return;
                if (jArrayEntities.Count == 0)
                    await Fusion.m_Messages.AlertAsyncMsg("No entities acquired!", "EntitiesToFeaturesAsync");
                await ShowCountAsync(jArrayEntities.Count);

                // Op - Prepare
                string opName = $"BuildFeaturesFromJsonEntitiesAsync {entityType}";
                bool selectNewFeatures = false;
                EditOperation editOperation = await Fusion.m_Helper_Op.PrepareOpAsync(opName, selectNewFeatures);
                if (editOperation == null)
                    return;

                string fieldId = $"{Fusion.m_FieldNameEntitiesPoints_OBJECTID}";
                string subFields = $"{Fusion.m_FieldNameEntitiesPoints_OBJECTID}";
                List<Object> listIds = await Fusion.m_Helper_Search.GetLayerFieldValuesAsync(m_LayerEntitiesPoints, fieldId, subFields);

                List<long> listEntities = new List<long>();
                foreach (System.Int32 entityId in listIds)
                {
                    listEntities.Add(entityId);
                }
                IEnumerable<long> oids = listEntities;
                editOperation.Delete(m_LayerEntitiesPoints, oids);

                // EntitiesToFeatures
                m_Helper_Progress = new Helper_Progress(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework, isProgressCancelable);
                await m_Helper_Progress.ShowProgressAsync("BuildFeaturesFromJsonEntitiesAsync", (uint)jArrayEntities.Count, true);
                bool taskResult = await QueuedTask.Run(async () =>
                {
                    return await RestApi_Entities.BuildFeaturesFromJsonEntitiesAsync(m_LayerEntitiesPoints, editOperation, jArrayEntities);
                }, m_Helper_Progress.ProgressAssistant);

                if (taskResult)
                {
                    if (!await Fusion.m_Helper_Op.ExecOpAsync(editOperation))
                        return;

                    await RestApi_Entities.StartUpdateAsync(entityType);
                }
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


        private async void ComboBox_EntityTypes_DropDownClosed(object sender, EventArgs e)
        {
            await CleanEntitiesCountAsync(false);
            Button_EntityToLayer.IsEnabled = HasComboEntityTypeSelected;
        }

        private async void Button_EntityToLayer_Click(object sender, RoutedEventArgs e)
        {
            await EntitiesToFeaturesAsync();
        }

        private async void Button_Datasource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 3.3.05/20231128/msGIS_FIWARE_rt_001: [FIWARE] Integration ArcGIS PRO.
                bool evalPlugInDatastore = false;
                if ((!evalPlugInDatastore) || (m_IsInitialized_ProPluginDatasource))
                    return;

                string linkTypes = "https://fiwaredev.msgis.net/ngsi-ld/v1/types";
                Uri uriTypes = new Uri(linkTypes);

                await QueuedTask.Run(() =>
                {
                    // Plugin identifier is corresponding to ProPluginDatasource Config.xml PluginDatasource ID
                    using (PluginDatastore pluginDatastore = new PluginDatastore(
                     new PluginDatasourceConnectionPath(Fusion.m_ProPluginDatasourceID_Entities, uriTypes)))
                    {
                        foreach (var table_name in pluginDatastore.GetTableNames())
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
                });

                m_IsInitialized_ProPluginDatasource = true;

                return;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "Button_Datasource_Click");
                return;
            }
        }

    }
}
