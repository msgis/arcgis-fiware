using msGIS.ProApp_Common_FIWARE;
using msGIS.ProApp_Vault_FIWARE;

using ArcGIS.Desktop.Framework.Contracts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msGIS.ProApp_FiwareSummit
{
    class Dockpane_EntityTypes : DockPane
    {
        private readonly string m_ModuleName = "Dockpane_EntityTypes";

        protected Dockpane_EntityTypes()
        {
            try
            {
                // Link this
                Fusion.m_Dockpane_EntityTypes = this;

                // setup the lists and sync between background and UI
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "Dockpane_EntityTypes");
            }
        }

        #region Overrides

        /// <summary>
        /// Override to implement custom initialization code for this dockpane
        /// </summary>
        /// <returns></returns>
        protected override Task InitializeAsync()
        {
            return base.InitializeAsync();
        }

        protected override async void OnShow(bool isVisible)
        {
            if (isVisible)
                await SetTS_EntityTypesAsync(true);
        }

        protected override async void OnHidden()
        {
            await SetTS_EntityTypesAsync(false);
        }

        #endregion Overrides

        private async Task SetTS_EntityTypesAsync(bool isChecked)
        {
            try
            {
                await Fusion.m_Helper_Framework.SetPlugInModeAsync(Fusion.m_ButtonID_TS_EntityTypesBoard, Fusion.m_StateID_TS_EntityTypes, isChecked);

                if ((!isChecked) && (Fusion.m_UserControl_EntityTypes != null) && (!await Fusion.m_UserControl_EntityTypes.CanChangeEntityTypesAsync()))
                {
                    await Fusion.m_Helper_Framework.SwitchDockpaneAsync(Fusion.m_DockpaneID_EntityTypes, true);
                }
                else
                {
                    if (Fusion.m_UserControl_EntityTypes != null)
                    {
                        await Fusion.m_UserControl_EntityTypes.m_Spring_EntityTypes.Activate_EntityTypesAsync(isChecked);
                    }
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "SetTS_EntityTypesAsync");
            }
        }
    }
}
