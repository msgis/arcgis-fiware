using ArcGIS.Core.Data;
using ArcGIS.Core.Data.PluginDatastore;
using ArcGIS.Core.Geometry;

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

        /*
        private string m_DatasourcePath = "";
        private Dictionary<string, PluginTableTemplate> m_DicTypeTables;
        private IReadOnlyList<string> m_TableNames = null;
        */

        public override void Open(Uri connectionPath)
        {
            //TODO Initialize your plugin instance. Individual instances
            //of your plugin may be initialized on different threads
            throw new NotImplementedException();

            // 3.3.05/20231201/msGIS_FIWARE_rt_002: Nicht überwindbare Komplikation auf HttpClient mittels GetAsync(apiUrl) aus der abstrakten Klasse ArcPro PluginDatasourceTemplate zuzugreifen.
            /*
            m_DatasourcePath = connectionPath.ToString();
            m_TableNames = Fusion.m_ListEntityTypes;
            m_DicTypeTables = new Dictionary<string, PluginTableTemplate>();
            */
            /*
            foreach (var tabName in GetTableNames())
            {
                m_DicTypeTables.Add(tabName, new ProPluginTableTemplate_EntityFile(tabName));
            }
            */

        }

        public override void Close()
        {
            //TODO Cleanup required to close the plugin 
            //data source instance
            throw new NotImplementedException();

            /*
            m_DatasourcePath = "";
            if (m_TableNames != null)
            {
                if (Fusion.m_ListEntityTypes != null)
                {
                    Fusion.m_ListEntityTypes.Clear();
                    Fusion.m_ListEntityTypes = null;
                }
                m_TableNames = null;
            }

            //Dispose of any cached table instances here
            if (m_DicTypeTables != null)
            {
                foreach (var table in m_DicTypeTables.Values)
                {
                    // ((ProPluginTableTemplate_EntityFile)table).Dispose();
                }
                m_DicTypeTables.Clear();
                m_DicTypeTables = null;
            }
            */

        }

        public override PluginTableTemplate OpenTable(string name)
        {
            //TODO Open the given table/object in the plugin
            //data source
            throw new NotImplementedException();

            /*
            if (!this.GetTableNames().Contains(tableName))
                throw new Exception($"The table {tableName} was not found!");

            SpatialReference spatialReference = null;
            ProPluginTableTemplate_EntityFile proPluginTableTemplate_EntityFile = new ProPluginTableTemplate_EntityFile(m_DatasourcePath, tableName, spatialReference);
            if (m_DicTypeTables.ContainsKey(tableName))
                throw new Exception($"The table {tableName} is ambiguous!");
            m_DicTypeTables.Add(tableName, proPluginTableTemplate_EntityFile);
            return proPluginTableTemplate_EntityFile;
            */
        }

        public override IReadOnlyList<string> GetTableNames()
        {
            var tableNames = new List<string>();

            //TODO Return the names of all tables in the plugin
            //data source
            return tableNames;

            // 3.3.05/20231201/msGIS_FIWARE_rt_002: Nicht überwindbare Komplikation auf HttpClient mittels GetAsync(apiUrl) aus der abstrakten Klasse ArcPro PluginDatasourceTemplate zuzugreifen.
            // var tableNames = new List<string>();
            // IReadOnlyList<string> tableNames = m_TableNames;        // new List<string>();

            /*
            // asyncTask.Wait();
            // Use a separate thread to wait for the task to complete
            Task.Run(async () =>
            {
                Task<List<string>> asyncTask = RestApi_EntityFile.ReadEntityTypesFromRestApiAsync(m_DatasourcePath);
                await asyncTask.ConfigureAwait(false);

                // Continue with the rest of the code after the task has completed
                List<string> listEntityTypes = asyncTask.Result;
                if ((asyncTask.IsCompleted) && (listEntityTypes != null))
                    m_TableNames = listEntityTypes;
            }).GetAwaiter().GetResult();
            */

        }

        public override bool IsQueryLanguageSupported()
        {
            //default is false
            return base.IsQueryLanguageSupported();
        }
    }
}
