using msGIS.ProApp_Common_FIWARE_3x;

using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Events;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Core.Events;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Events;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.ComponentModel;

namespace msGIS.ProApp_FiwareTest
{
    internal class Module_FiwareTest : Module
    {
        private readonly string m_ModuleName = "Module_FiwareTest";

        private SubscriptionToken m_STProjectOpened = null;
        private SubscriptionToken m_STProjectClosing = null;
        private SubscriptionToken m_STProjectClosed = null;
        private SubscriptionToken m_STMapViewInitialized = null;
        private SubscriptionToken m_STMapViewChanged = null;
        private SubscriptionToken m_STMapClosed = null;
        // private SubscriptionToken m_STMapMemberPropertiesChanged = null;

        private bool m_IsMapViewInitialized = false;
        private bool m_IsActiveMapMain = false;
        // private Layer m_LayerLeitungL = null;

        private static Module_FiwareTest _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static Module_FiwareTest Current => _this ??= (Module_FiwareTest)FrameworkApplication.FindModule("msGIS_FiwareTest_ModuleID_Main");

        #region Overrides

        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();
        }

        protected override bool Initialize()
        {
            bool init = base.Initialize();
            if (init)
                _ = InitAsync();
            return init;
        }

        #endregion Overrides

        #region Init

        private async Task InitAsync()
        {
            try
            {
                // 1.3.4/20220809/msGIS_ProApp_Common_rt_003: Common variables and functions must not be static.
                // 3.3.21/20230131/msGIS_ProApp_Common_rt_034: Helper_Op, Helper_Geom.
                // Common
                Fusion.m_Global = new Global();
                Fusion.m_Messages = new Messages(Fusion.m_Global);
                Fusion.m_General = new General(Fusion.m_Global, Fusion.m_Messages);
                Fusion.m_Helper_FindFeature = new Helper_FindFeature(Fusion.m_Global, Fusion.m_Messages);
                Fusion.m_Helper_Framework = new Helper_Framework(Fusion.m_Global, Fusion.m_Messages);
                Fusion.m_Helper_Image = new Helper_Image(Fusion.m_Global, Fusion.m_Messages);
                Fusion.m_Helper_Layer = new Helper_Layer(Fusion.m_Global, Fusion.m_Messages, Fusion.m_General, Fusion.m_Helper_Framework);
                Fusion.m_Helper_Search = new Helper_Search(Fusion.m_Messages);
                Fusion.m_Helper_Selection = new Helper_Selection(Fusion.m_Messages);
                Fusion.m_Helper_Zoom = new Helper_Zoom(Fusion.m_Messages, Fusion.m_Helper_Layer, Fusion.m_Helper_Search);
                Fusion.m_Helper_Uni = new Helper_Uni(Fusion.m_Global, Fusion.m_Messages);
                Fusion.m_Helper_Geom = new Helper_Geom(Fusion.m_Messages);
                Fusion.m_Helper_Op = new Helper_Op(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework);

                // ApplicationReadyEvent.Subscribe(OnApplicationReady);
                // ApplicationClosingEvent.Subscribe(OnApplicationClosing);
                ActiveWindowChangedEvent.Subscribe(OnActiveWindowChanged);
                m_STProjectOpened = ProjectOpenedAsyncEvent.Subscribe(OnProjectOpenedAsync);
                m_STProjectClosing = ProjectClosingEvent.Subscribe(OnProjectClosingAsync);
                m_STProjectClosed = ProjectClosedEvent.Subscribe(OnProjectClosed);

                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                Fusion.m_Global.AssemblyName = assembly.GetName();
                // 3.3.19/20230120/msGIS_ProApp_Common_rt_032: AssemblyName.Version 3x --> AddInInfo.Version.
                // string caption = $"{Fusion.m_Global.m_AssemblyName.Name} {Fusion.m_Global.m_AssemblyName.Version}";
                // (Caption to long)    // await Fusion.m_Helper_Framework.SetPlugInCaptionAsync(Fusion.m_GroupID_General, caption);
                await Fusion.m_General.SetAddInInfoAsync();

                Fusion.m_Global.m_StateID_IsInProgress = "msGIS_FiwareTest_StateID_IsInProgress";
                Fusion.m_Global.m_IdPrefix = "msGIS_FiwareTest_";
                await SetPlugInsIconsAsync();

                // if (System.Diagnostics.Debugger.IsAttached)
                Fusion.m_Global.m_StartState_RibbonTabs = true;      // (RibbonTabs T/F funktioniert)
                // Fusion.m_Global.m_StartState_Controls = true;     // (not working in all cases)
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "InitAsync");
            }
        }

        private async Task SetPlugInsIconsAsync()
        {
            try
            {
                // AddInContent --> Resource
                await Fusion.m_Helper_Framework.SetPlugInIconAsync(Fusion.m_ButtonID_ProjectInfo);

                // await Fusion.m_Helper_Framework.SetPlugInIconAsync(Fusion.m_ButtonID_TS_*Board);             // already done on Main - SetButtonToggleStateFromDockpaneAsync
                // await Fusion.m_Helper_Framework.SetPlugInIconAsync(Fusion.m_ButtonID_TS_EntityTypesBoard);   // already done on Main - SetButtonToggleStateFromDockpaneAsync
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "SetPlugInsIcons");
            }
        }

        #endregion Init

        #region Events

        /*
        private void OnApplicationReady(EventArgs obj)
        {
            // Ready to work with projects
            // System.Diagnostics.Debug.WriteLine("The application is ready.");
        }
        */

        /*
        private Task OnApplicationClosing(CancelEventArgs args)
        {
        }
        */

        private async void OnActiveWindowChanged(WindowEventArgs obj)
        {
            // Active window (e.g. after first map view changed, attribute window changes)
            // ArcGIS.Desktop.Editing.TablePaneViewModel
            // ArcGIS.Desktop.Core.ContentsDockPaneViewModel
            // ArcGIS.Desktop.Mapping.MapPaneViewModel
            // System.Diagnostics.Debug.WriteLine(obj.Window.ToString());
            try
            {
                bool testWindow = false;
                if ((testWindow) && (obj.Window != null))
                {
                    string windowTypeName = obj.Window.GetType().Name;
                    if (windowTypeName == "LayoutPaneViewModel")
                    {
                        ILayoutPane layoutPane = (ILayoutPane)obj.Window;
                        string lyoutCaption = layoutPane.Caption;
                        LayoutView layoutView = layoutPane.LayoutView;
                        MapFrame mapFrame = layoutView.ActiveMapFrame;
                        Map map = mapFrame.Map;
                        Layout layout = layoutView.Layout;
                        string name = layout.Name;
                    }
                    else if (windowTypeName == "MapPaneViewModel")
                    {
                        IMapPane mapPane = (IMapPane)obj.Window;
                        string mapCaption = mapPane.Caption;
                        MapView mapView = mapPane.MapView;
                        Map map = mapView.Map;
                    }
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "OnActiveWindowChanged");
            }
        }

        private async Task OnProjectClosingAsync(ProjectClosingEventArgs args)
        {
            try
            {
                // System.Diagnostics.Debug.WriteLine("The application is closing.");
                if ((Fusion.m_Global.m_IsProjectFitting) && (!args.Cancel))
                {
                    if ((Fusion.m_UserControl_EntityTypes != null) && (!await Fusion.m_UserControl_EntityTypes.CanChangeEntityTypesAsync()))
                    {
                        await Fusion.m_Helper_Framework.SwitchDockpaneAsync(Fusion.m_DockpaneID_EntityTypes, true);
                        args.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "OnProjectClosingAsync");
            }
        }

        private async Task EnableActiveBoardsAsync(bool isEnabled)
        {
            try
            {
                if ((Fusion.m_UserControl_EntityTypes != null) && (await Fusion.m_Helper_Framework.IsStateTurnedOnAsync(Fusion.m_StateID_TS_EntityTypes)))
                {
                    Fusion.m_UserControl_EntityTypes.IsEntityTypesEnabled = isEnabled;
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "EnableActiveBoardsAsync");
            }
        }

        private async Task RemoveDockpanesAsync()
        {
            try
            {
                // await Fusion.m_Helper_Framework.SwitchDockpaneAsync(Fusion.m_DockpaneID_*, false);
                await Fusion.m_Helper_Framework.SwitchDockpaneAsync(Fusion.m_DockpaneID_EntityTypes, false);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "RemoveDockpanesAsync");
            }
        }

        private async void OnProjectClosed(ProjectEventArgs args)
        {
            try
            {
                if (Fusion.m_Global.m_IsProjectFitting)
                {
                    m_IsMapViewInitialized = false;
                    m_IsActiveMapMain = false;

                    if (m_STMapViewInitialized != null)
                    {
                        MapViewInitializedEvent.Unsubscribe(m_STMapViewInitialized);
                        m_STMapViewInitialized = null;
                    }
                    if (m_STMapViewChanged != null)
                    {
                        ActiveMapViewChangedEvent.Unsubscribe(m_STMapViewChanged);
                        m_STMapViewChanged = null;
                    }
                    if (m_STMapClosed != null)
                    {
                        MapClosedEvent.Unsubscribe(m_STMapClosed);
                        m_STMapClosed = null;
                    }
                    /*
                    if (m_STMapMemberPropertiesChanged != null)
                    {
                        MapMemberPropertiesChangedEvent.Unsubscribe(m_STMapMemberPropertiesChanged);
                        m_STMapMemberPropertiesChanged = null;
                    }
                    */

                    await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_StateID_Main, false);
                    await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_StateID_Menu, false);
                    await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_Global.m_StateID_IsInProgress, false);
                    await EnableActiveBoardsAsync(false);
                    if (!Fusion.m_Global.m_StartState_Controls)
                        await RemoveDockpanesAsync();

                    await Fusion.CleanOneTimeRefsAsync();

                    // 1.3.8/20220915/msGIS_ProApp_Common_rt_007: Check OnProject_Opened/Closed.
                    // OnProjectClosed (async void) may be on working while OnProjectOpenedAsync (Task) is running?
                    // Fusion.m_Global.m_IsProjectFitting = false;
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "OnProjectClosed");
            }
        }

        private async Task OnProjectOpenedAsync(ProjectEventArgs args)
        {
            try
            {
                // System.Diagnostics.Debug.WriteLine($"The project {<arg.Project.Name>} has been opened.");
                if (args.Project.Name != Project.Current.Name)
                    throw new Exception($"Project name {args.Project.Name} does not fit current name {Project.Current.Name}");

                string projNameUpp = args.Project.Name.ToUpper();
                Fusion.m_Global.m_IsProjectFitting = ((projNameUpp.Contains(Fusion.m_ProjectMainPhrase.ToUpper())) && (projNameUpp.Contains(Fusion.m_ProjectSubPhrase.ToUpper())));
                if (!Fusion.m_Global.m_IsProjectFitting)
                {
                    await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_StateID_Main, false);
                    await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_StateID_Menu, false);
                }
                else
                {
                    // List<object> listEntityTypes = await Settings_EntityTypes.AcquireSettingsEntityTypesAsync();

                    if (m_STMapViewInitialized == null)
                        m_STMapViewInitialized = MapViewInitializedEvent.Subscribe(OnMapViewInitialized);
                    if (m_STMapViewChanged == null)
                        m_STMapViewChanged = ActiveMapViewChangedEvent.Subscribe(OnMapViewChanged);
                    if (m_STMapClosed == null)
                        m_STMapClosed = MapClosedEvent.Subscribe(OnMapClosed);
                    /*
                    if (m_STMapMemberPropertiesChanged == null)
                        m_STMapMemberPropertiesChanged = MapMemberPropertiesChangedEvent.Subscribe(OnMapMemberPropertiesChanged);
                    */

                    bool isOnDebug = System.Diagnostics.Debugger.IsAttached;
                    await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_StateID_Main, true);
                    await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_StateID_Menu, true);
                    await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_Global.m_StateID_IsInProgress, false);

                    await Fusion.CleanOneTimeRefsAsync();
                }

                return;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "OnProjectOpenedAsync");
                return;
            }
        }

        private async Task<bool> InitActiveAsync(MapView mapViewInit)
        {
            try
            {
                Map mapInit = mapViewInit.Map;
                m_IsActiveMapMain = await Fusion.m_General.IsIncomingMapMainAsync(mapInit, Fusion.m_MapTag);
                await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_StateID_Map, m_IsActiveMapMain);
                bool setState_Plaene = ((Fusion.m_Global.m_StartState_RibbonTabs) && (m_IsActiveMapMain));
                // await SetPlaeneState(setState_Plaene);
                if (!m_IsActiveMapMain)
                    return false;

                if (Fusion.m_Global.m_StartState_Controls)
                {
                    // Enable may not work due to not initialized controls (Fusion.m_UserControl_*).
                    await EnableActiveBoardsAsync(false);

                    // Restore Dockpanes (not to remove i.e. RemoveDockpanesAsync) requires valid layers status!
                    // Not implemented! (not working in all cases)
                    // await Fusion.m_ComboBox_PlanModus.InitActive_PlanModusAsync();
                }
                else
                {
                    await RemoveDockpanesAsync();
                }

                // await Fusion.m_Helper_Framework.SetButtonToggleStateFromDockpaneAsync(Fusion.m_ButtonID_TS_*Board, Fusion.m_StateID_TS_*, Fusion.m_DockpaneID_*);
                await Fusion.m_Helper_Framework.SetButtonToggleStateFromDockpaneAsync(Fusion.m_ButtonID_TS_EntityTypesBoard, Fusion.m_StateID_TS_EntityTypes, Fusion.m_DockpaneID_EntityTypes);

                // Check if the map has a specific layer.
                // Doing this may trigger OnMapViewChanged
                /*
                if (m_LayerLeitungL == null)
                {
                    m_LayerLeitungL = await Fusion.GetLayerLeitungLAsync();
                    if (m_LayerLeitungL == null)
                        return false;
                }
                */

                return true;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "InitActiveAsync");
                return false;
            }
        }

        private async void OnMapViewInitialized(MapViewEventArgs args)
        {
            try
            {
                // OnMapViewInitialized occurs prior to OnMapViewChanged.
                // OnMapViewInitialized MapViewEventArgs args.MapView may be present while MapView.Active is null!
                // OnMapViewChanged MapView.Active is null after closing the map.
                if ((Fusion.m_Global.m_IsProjectFitting) && (!m_IsMapViewInitialized))
                {
                    MapView mapViewInit = args.MapView;
                    if (mapViewInit != null)
                        m_IsMapViewInitialized = await InitActiveAsync(mapViewInit);
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "OnMapViewInitialized");
            }
        }

        private async void OnMapViewChanged(ActiveMapViewChangedEventArgs args)
        {
            if ((!Fusion.m_Global.m_IsProjectFitting) || (!m_IsMapViewInitialized))
                return;

            try
            {
                if (Fusion.m_General.m_OnStage)
                {
                    bool isReady = await Fusion.m_General.WaitUntilStageReadyAsync();
                    return;
                }
                Fusion.m_General.m_OnStage = true;

                // Occurs on every changed map, but also if project is starting and first map is opened.
                // May also occours while OnMapViewInitialized is on stage (with actual flags e.g. m_IsMapViewInitialized, m_IsActiveMapMain)
                // Sequence on opened project: OnMapViewChanged.OutgoingView (if any) - OnMapViewInitialized (for new map) - OnMapViewChanged.IncomingView
                // Sequence on reopened (menu): Both OnMapViewChanged.OutgoingView and OnMapViewChanged.IncomingView may be provided! (if activated map is not the first on tab list, Incoming only otherwise).
                MapView mapViewOutgoing = args.OutgoingView;
                MapView mapViewIncoming = args.IncomingView;

                if ((mapViewOutgoing != null) && (m_IsActiveMapMain))
                {
                    // For outgoing map LayoutView.Active = null, therefore IsIncomingMapMain(mapOutgoing) doesn't recognize Layout!
                    // Map mapOutgoing = mapViewOutgoing.Map;
                    m_IsActiveMapMain = false;

                    await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_StateID_Map, false);
                    await EnableActiveBoardsAsync(false);
                }

                if (mapViewIncoming != null)
                {
                    Map mapIncoming = mapViewIncoming.Map;
                    m_IsActiveMapMain = await Fusion.m_General.IsIncomingMapMainAsync(mapIncoming, Fusion.m_MapTag);

                    await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_StateID_Map, m_IsActiveMapMain);
                    await EnableActiveBoardsAsync(m_IsActiveMapMain);
                }

                // Test State
                bool testState = false;
                if (testState)
                {
                    bool isMapMainActive = await Fusion.m_Helper_Framework.IsStateTurnedOnAsync(Fusion.m_StateID_Map);
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "OnMapViewChanged");
            }
            finally
            {
                Fusion.m_General.m_OnStage = false;
            }
        }


        private async void OnMapClosed(MapClosedEventArgs args)
        {
            try
            {
                // OnMapClosed occurs after OnMapViewChanged.
                // OnMapViewChanged MapView.Active is null after closing the map.
                /*
                if (args.MapPane != null)
                {
                    MapView mapViewClosed = args.MapPane.MapView;
                    if (mapViewClosed != null)
                    {
                        // Map mapClosed = mapViewClosed.Map;
                        bool isClosedMapMain = m_IsActiveMapMain;
                        if (isClosedMapMain)
                        {
                            m_IsActiveMapMain = false;
                            await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_StateID_Map, false);
                            await EnableActiveBoardsAsync(false);
                        }
                    }
                }
                */
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "OnMapClosed");
            }
        }

        /*
        private async void OnMapMemberPropertiesChanged(MapMemberPropertiesChangedEventArgs args)
        {
            try
            {
                // OnMapMemberPropertiesChanged occurs when any property of layer or standalone table changed.
                if ((MapView.Active != null) && (args.MapMembers != null))
                {
                    MapMember mapMember = args.MapMembers.First<MapMember>();
                    if (mapMember.GetType() == typeof(FeatureLayer))
                    {
                        Layer layer = (Layer)mapMember;
                    }
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "OnMapMemberPropertiesChanged");
            }
        }
        */

        #endregion Events

        #region Toggle State

        /*
        internal async void OnCustomButtonClick_TS_*Board()
        {
            try
            {
                if ((Fusion.m_UserControl_* != null) && (await Fusion.m_Helper_Framework.IsStateTurnedOnAsync(Fusion.m_StateID_TS_*)) && (!await Fusion.m_UserControl_*.CanChange*Async()))
                    return;

                bool isActive = await Fusion.m_Helper_Framework.SetButtonToggleStateFromFrameworkAsync(Fusion.m_ButtonID_TS_*Board, Fusion.m_StateID_TS_*);
                await Fusion.m_Helper_Framework.SwitchDockpaneAsync(Fusion.m_DockpaneID_*, isActive);

                //if (m_UserControl_* != null)
                //    await m_UserControl_*.ActivateAsync(isActive);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "OnCustomButtonClick_TS_*Board");
            }
        }
        */

        internal async void OnCustomButtonClick_TS_EntityTypes()
        {
            try
            {
                if ((Fusion.m_UserControl_EntityTypes != null) && (await Fusion.m_Helper_Framework.IsStateTurnedOnAsync(Fusion.m_StateID_TS_EntityTypes)) && (!await Fusion.m_UserControl_EntityTypes.CanChangeEntityTypesAsync()))
                    return;

                bool isActive = await Fusion.m_Helper_Framework.SetButtonToggleStateFromFrameworkAsync(Fusion.m_ButtonID_TS_EntityTypesBoard, Fusion.m_StateID_TS_EntityTypes);
                await Fusion.m_Helper_Framework.SwitchDockpaneAsync(Fusion.m_DockpaneID_EntityTypes, isActive);

                //if (m_UserControl_EntityTypes != null)
                //    await m_UserControl_EntityTypes.ActivateAsync(isActive);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "OnCustomButtonClick_TS_EntityTypes");
            }
        }

        #endregion Toggle State

        #region Commands
        // Delegate command OnClick handler

        internal async void OnCustomButtonClick_ProjectInfo()
        {
            try
            {
                await Fusion.ProjectInfo();
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "OnCustomButtonClick_ProjectInfo");
            }
        }

        /*
        internal async void OnCustomButtonClick_Datasource()
        {
            try
            {

            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "OnCustomButtonClick_Datasource");
            }
        }
        */

        #endregion Commands

    }
}
