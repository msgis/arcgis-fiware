using msGIS.ProApp_Common_FIWARE_3x;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msGIS.ProPluginDatasource_FiwareHttpClient
{
    public static class Fusion
    {
        // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
        private static readonly string m_ModuleName = "Fusion";

        #region Common

        internal static Global m_Global { get; set; }
        internal static Messages m_Messages { get; set; }
        internal static Helper_Framework m_Helper_Framework { get; set; }
        internal static Fiware_RestApi_NetHttpClient m_Fiware_RestApi_NetHttpClient { get; set; }

        #endregion Common

        internal static Fiware_RestApi_NetHttpClient.UriDatasource m_UriDatasource;
        private static bool m_IsInitialized { get; set; }

        // 3.3.08/20240109/msGIS_FIWARE_rt_012: Init Fiware_RestApi_NetHttpClient before Plugin Datasource OpenTable/GetTableNames.
        // 3.3.09/20240110/msGIS_FIWARE_rt_014: Configurable URI.
        internal const string m_DatasourcePath = "https://fiwaredev.msgis.net";
        internal const string m_DatasourceTypes = "/ngsi-ld/v1/types";
        internal const string m_DatasourceEntities = "/ngsi-ld/v1/entities?";
        internal const string m_DatasourceEventsource = "/ngsi-proxy/eventsource/e9e01390-fae3-11ed-926f-1bdc1977e2d3";

        internal const string m_DataColumn_ID = "OBJECTID";
        internal const string m_DataColumn_Geom = "SHAPE";
        internal const string m_DataColumn_X = "POINT_X";
        internal const string m_DataColumn_Y = "POINT_Y";

        public static async Task<bool> InitAsync()
        {
            try
            {
                if (!m_IsInitialized)
                {
                    Fusion.m_Global = new Global();
                    Fusion.m_Messages = new Messages(Fusion.m_Global);
                    Fusion.m_Helper_Framework = new Helper_Framework(Fusion.m_Global, Fusion.m_Messages);
                    Fusion.m_Fiware_RestApi_NetHttpClient = new Fiware_RestApi_NetHttpClient(Fusion.m_Global, Fusion.m_Messages);

                    // 3.3.06/20231221/msGIS_FIWARE_rt_008: Datasource URI.
                    // 3.3.08/20240109/msGIS_FIWARE_rt_012: Init Fiware_RestApi_NetHttpClient before Plugin Datasource OpenTable/GetTableNames.
                    // 3.3.09/20240110/msGIS_FIWARE_rt_014: Configurable URI.
                    Uri uriDatasourcePath = new Uri(Fusion.m_DatasourcePath, UriKind.Absolute);
                    m_UriDatasource = new Fiware_RestApi_NetHttpClient.UriDatasource
                    {
                        path = uriDatasourcePath,
                        types = Fusion.m_DatasourceTypes,
                        entities = Fusion.m_DatasourceEntities,
                        eventsource = Fusion.m_DatasourceEventsource,
                        spatialReference = ArcGIS.Core.Geometry.SpatialReferences.WGS84
                    };

                    m_IsInitialized = true;
                }
                else
                {
                    //if (!await Fusion.m_Messages.MsAskAsync($"The ProPluginDatasource is initialized already!{Environment.NewLine}Repeat initialisation?", "FIWARE"))
                    //    return false;
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
