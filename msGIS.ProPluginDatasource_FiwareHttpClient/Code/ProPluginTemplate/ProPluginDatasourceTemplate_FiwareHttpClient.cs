using ArcGIS.Core.Data;
using ArcGIS.Core.Data.PluginDatastore;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msGIS.ProPluginDatasource_FiwareHttpClient
{
    public class ProPluginDatasourceTemplate_FiwareHttpClient : PluginDatasourceTemplate
    {
        // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
        // 3.3.06/20231218/msGIS_FIWARE_rt_007: ProPluginDatasource_FiwareHttpClient.

        private Uri m_UriDatasourcePath;
        private Dictionary<string, PluginTableTemplate> m_DicTables;

        public override void Open(Uri connectionPath)
        {
            //TODO Initialize your plugin instance. Individual instances
            //of your plugin may be initialized on different threads
            // throw new NotImplementedException();

            //initialize
            m_UriDatasourcePath = connectionPath;
            m_DicTables = new Dictionary<string, PluginTableTemplate>();

        }

        public override void Close()
        {
            //TODO Cleanup required to close the plugin 
            //data source instance
            // throw new NotImplementedException();

            //Dispose of any cached table instances here
            if (m_DicTables != null)
            {
                foreach (var table in m_DicTables.Values)
                {
                    ((ProPluginTableTemplate_FiwareHttpClient)table).Dispose();
                }
                m_DicTables.Clear();
                m_DicTables = null;
            }

        }

        public override PluginTableTemplate OpenTable(string name)
        {
            //TODO Open the given table/object in the plugin
            //data source
            // throw new NotImplementedException();

            string tableName = name;
            if (!this.GetTableNames().Contains(tableName))
                throw new Exception($"The table {tableName} was not found!");

            // if (m_DicTables.ContainsKey(tableName))
            if (m_DicTables.Keys.Contains(tableName))
                throw new Exception($"The table {tableName} is ambiguous!");

            SpatialReference spatialReference = SpatialReferences.WGS84;
            ProPluginTableTemplate_FiwareHttpClient proPluginTableTemplate_FiwareHttpClient = new ProPluginTableTemplate_FiwareHttpClient(m_UriDatasourcePath, tableName, spatialReference);
            // m_DicTables.Add(tableName, proPluginTableTemplate_FiwareHttpClient);
            m_DicTables[tableName] = proPluginTableTemplate_FiwareHttpClient;

            return proPluginTableTemplate_FiwareHttpClient;
        }

        public override IReadOnlyList<string> GetTableNames()
        {
            //var tableNames = new List<string>();
            //TODO Return the names of all tables in the plugin
            //data source
            // return tableNames;

            // 3.3.05/20231201/msGIS_FIWARE_rt_002: Nicht überwindbare Komplikation auf HttpClient mittels GetAsync(apiUrl) aus der abstrakten Klasse ArcPro PluginDatasourceTemplate zuzugreifen.
            // 3.3.06/20231218/msGIS_FIWARE_rt_007: ProPluginDatasource_FiwareHttpClient.
            List<string> tableNames = null;
            // Use asynchronous call due to lack of adequate synchronous "httpResponse.Content.ReadAsStringAsync" function, even if "esriHttpClient.Get(requestUri)" function is available.
            // Esri QueuedTask.Run is not suitable in a synchronous routine with due to dead lock on desired "GetAwaiter().GetResult()" modality for fulfilled object to return.
            // asyncTask.Wait();
            // Use a separate thread to wait for the task to complete
            Task.Run(async () =>
            {
                Task<List<string>> asyncTask = Fusion.m_Fiware_RestApi_NetHttpClient.ReadEntityTypesFromRestApiAsync(m_UriDatasourcePath);
                await asyncTask.ConfigureAwait(false);

                // Continue with the rest of the code after the task has completed
                if (asyncTask.IsCompleted)
                {
                    tableNames = asyncTask.Result;
                }
            }).GetAwaiter().GetResult();

            return tableNames;
        }

        public override bool IsQueryLanguageSupported()
        {
            //default is false
            bool isQueryLanguageSupported = base.IsQueryLanguageSupported();
            return isQueryLanguageSupported;
        }
    }
}
