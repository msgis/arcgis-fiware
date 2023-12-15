using msGIS.ProApp_Common_FIWARE_3x;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msGIS.ProPluginDatasource_EntityFile
{
    public static class Fusion
    {
        // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
        private static readonly string m_ModuleName = "Fusion";

        #region Common

        internal static Global m_Global { get; set; }
        internal static Messages m_Messages { get; set; }
        internal static Fiware_RestApi_NetHttpClient m_Fiware_RestApi_NetHttpClient { get; set; }

        #endregion Common

        public static bool m_IsInitialized { get; set; }
        internal static string m_DatasourcePath { get; set; }
        internal static List<string> m_ListEntityTypes { get; set; }

        public const string m_FileSuffix = "fiw";     // "csv";

        public static async Task<bool> InitAsync(string datasourcePath)
        {
            try
            {
                // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
                if (m_IsInitialized)
                {
                    if (!await Fusion.m_Messages.MsAskAsync($"The ProPluginDatasource is initialized already!{Environment.NewLine}Repeat initialisation?", "FIWARE"))
                        return false;
                }

                Fusion.m_Global = new Global();
                Fusion.m_Messages = new Messages(Fusion.m_Global);
                Fusion.m_Fiware_RestApi_NetHttpClient = new Fiware_RestApi_NetHttpClient(Fusion.m_Global, Fusion.m_Messages);

                // Get entity types from JSON.
                // 3.3.05/20231201/msGIS_FIWARE_rt_002: Nicht überwindbare Komplikation auf HttpClient mittels GetAsync(apiUrl) aus der abstrakten Klasse ArcPro PluginDatasourceTemplate zuzugreifen.
                m_ListEntityTypes = await Fusion.m_Fiware_RestApi_NetHttpClient.ReadEntityTypesFromRestApiAsync(datasourcePath);
                if (m_ListEntityTypes != null)
                {
                    m_DatasourcePath = datasourcePath;
                    m_IsInitialized = true;
                }

                return m_IsInitialized;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "InitAsync");
                return false;
            }
        }
    }
}
