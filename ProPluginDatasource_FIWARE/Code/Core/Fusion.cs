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
        // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
        private static readonly string m_ModuleName = "Fusion";

        #region Common

        internal static Global m_Global { get; set; }
        internal static Messages m_Messages { get; set; }

        #endregion Common

        public static bool m_IsInitialized { get; set; }
        internal static string m_DatasourcePath { get; set; }
        internal static List<string> m_ListEntityTypes { get; set; }

        public static async Task InitAsync(string datasourcePath)
        {
            try
            {
                // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
                if (m_IsInitialized)
                    throw new Exception("The ProPluginDatasource_FIWARE is initialized already!");

                Fusion.m_Global = new Global();
                Fusion.m_Messages = new Messages(Fusion.m_Global);

                // Get entity types from JSON.
                // 3.3.05/20231201/msGIS_FIWARE_rt_002: Nicht überwindbare Komplikation auf HttpClient mittels GetAsync(apiUrl) aus der abstrakten Klasse ArcPro PluginDatasourceTemplate zuzugreifen.
                m_ListEntityTypes = await RestApi_Fiware.ReadEntityTypesFromRestApiAsync(datasourcePath);
                if (m_ListEntityTypes != null)
                {
                    m_DatasourcePath = datasourcePath;
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
