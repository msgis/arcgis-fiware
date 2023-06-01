using msGIS.ProApp_Common_MA31_GIS_3x;

using ArcGIS.Desktop.Framework.Contracts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msGIS.ProApp_FiwareSummit
{
    internal class Button_ProjectInfo : Button
    {
        private readonly string m_ModuleName = "Button_ProjectInfo";

        internal Button_ProjectInfo()
        {
            try
            {
                IsChecked = false;
                Caption = "Info";

                /*
                Uri uriSource = null;
                // uriSource = new Uri("pack://application:,,,/ArcGIS.Desktop.Resources;component/Images/GenericLockNoColor16.png");
                uriSource = new Uri("pack://application:,,,/{Fusion.m_Global.m_AssemblyName.Name};component/Images/GenericButtonBlue16.png");
                SmallImage = new BitmapImage(uriSource);
                */
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "Button_ProjectInfo");
            }
        }

        protected override async void OnClick()
        {
            try
            {
                // string ButtonID = this.ID;
                // System.Diagnostics.Debug.WriteLine("Button ID: " + ButtonID);

                await Fusion.ProjectInfo();
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ProjectInfo");
            }
        }

        protected override async void OnUpdate()
        {
            try
            {
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ProjectInfo (Update)");
            }
        }
    }

}
