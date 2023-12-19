using msGIS.ProApp_Common_FIWARE_3x;

using ArcGIS.Core.Data;
using ArcGIS.Core.Events;
using ArcGIS.Desktop.Core;
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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace msGIS.ProApp_FiwareTest
{
    /// <summary>
    /// Interaction logic for UserControl_EntityTypes.xaml
    /// </summary>
    public partial class UserControl_EntityTypes : UserControl
    {
        private readonly string m_ModuleName = "UserControl_EntityTypes";

        internal Spring_EntityTypes m_Spring_EntityTypes;

        public UserControl_EntityTypes()
        {
            try
            {
                InitializeComponent();

                // Link this
                Fusion.m_UserControl_EntityTypes = this;

                // Images
                _ = InitImagesAsync();

                // Spring code
                m_Spring_EntityTypes = new Spring_EntityTypes(Grid_EntityTypes, ComboBox_EntityTypes, Label_Count, Button_EntityToLayer, TextBox_DataPath, Button_EntityToFile, Button_FileToDsfLayer, Button_EntityToDsfLayer);
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "UserControl_EntityTypes");
            }
        }

        private async Task InitImagesAsync()
        {
            try
            {
                //await Fusion.m_Helper_Framework.SetControlImageAsync(Button_RefreshLiefernummern, Image_Refresh);
                //await Fusion.m_Helper_Framework.SetControlImageAsync(Button_ZoomBaustelle, Image_ZoomRange);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "InitImagesAsync");
            }
        }

        internal bool IsEntityTypesEnabled
        {
            get
            {
                try
                {
                    bool isEnabled = false;
                    // The calling thread cannot access this object because a different thread owns it.
                    this.Dispatcher.Invoke(() =>
                    {
                        isEnabled = Grid_EntityTypes.IsEnabled;
                    });
                    return isEnabled;
                }
                catch (Exception ex)
                {
                    Fusion.m_Messages.PushEx(ex, m_ModuleName, "IsEntityTypesEnabled - get");
                    return false;
                }
            }
            set
            {
                try
                {
                    // The calling thread cannot access this object because a different thread owns it.
                    this.Dispatcher.Invoke(() =>
                    {
                        Grid_EntityTypes.IsEnabled = value;
                    });
                }
                catch (Exception ex)
                {
                    Fusion.m_Messages.PushEx(ex, m_ModuleName, "IsEntityTypesEnabled - set");
                }
            }
        }

        internal async Task SetEntityTypesStatusAsync(bool canChangeEntityTypes)
        {
            try
            {
                await Fusion.m_Helper_Framework.SetPlugInStateAsync(Fusion.m_StateID_CanChangeEntityTypes, canChangeEntityTypes);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "SetEntityTypesStatusAsync");
            }
        }

        internal async Task<bool> CanChangeEntityTypesAsync()
        {
            try
            {
                bool canChangeEntityTypes = (m_Spring_EntityTypes.m_CanChangeBoard_EntityTypes);
                if (!canChangeEntityTypes)
                {
                    string Msg = $"Das Board befindet sich in der Bearbeitung!{Environment.NewLine}Bitte Änderungen speichern oder verwerfen.";
                    await Fusion.m_Messages.AlertAsyncMsg(Msg, Fusion.m_Dockpane_EntityTypes.Caption);

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "CanChangeEntityTypesAsync");
                return false;
            }
        }

    }
}
