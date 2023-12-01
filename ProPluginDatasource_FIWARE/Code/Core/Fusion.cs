using msGIS.ProApp_Common_FIWARE_3x;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msGIS.ProPluginDatasource_FIWARE
{
    public static class Fusion
    {
        private static readonly string m_ModuleName = "Fusion";

        internal static bool m_IsInitialized { get; set; }

        #region Common

        internal static Global m_Global { get; set; }
        internal static Messages m_Messages { get; set; }

        #endregion Common

        public static async Task InitAsync()
        {
            try
            {
                if (!m_IsInitialized)
                {
                    Fusion.m_Global = new Global();
                    Fusion.m_Messages = new Messages(Fusion.m_Global);

                    m_IsInitialized = true;
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "InitAsync");
            }
        }
    }
}
