﻿using msGIS.ProApp_Common_FIWARE_3x;

using ArcGIS.Core.Data;
using ArcGIS.Core.Data.Exceptions;
using ArcGIS.Core.Data.PluginDatastore;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace msGIS.ProPluginDatasource_FiwareHttpClient
{
    public class ProPluginDatasourceTemplate_FiwareHttpClient : PluginDatasourceTemplate
    {
        // Summary:
        //     This abstract class serves as one of the key extensibility points that comprise
        //     the Plugin Datastore Framework. Specifically, each instance of a concrete class
        //     that implements this abstraction acts as a conduit between a third-party data
        //     source and ArcGIS Pro via the deployment of a plug-in data source add-in.
        //     Currently, the framework only supports ArcGIS.Core.Data.DatasetType.Tables and
        //     ArcGIS.Core.Data.DatasetType.FeatureClasss in a read-only manner.

        // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
        // 3.3.07/20231222/msGIS_FIWARE_rt_009: Plugin integration.
        private readonly string m_ModuleName = "ProPluginDatasourceTemplate_FiwareHttpClient";

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [DllImport("kernel32.dll")]
        internal static extern uint GetCurrentThreadId();
        private uint _thread_id;

        // private Uri m_UriDatasourcePath;
        private Dictionary<string, PluginTableTemplate> m_DicTables;

        // Initializes a new instance of the ArcGIS.Core.Data.PluginDatastore.PluginDatasourceTemplate class.
        public ProPluginDatasourceTemplate_FiwareHttpClient() : base()
        {
            try
            {
                // Set the name and category for your custom data source
                //Name = "My Custom Data Source";
                //Category = "My Category";
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "ProPluginDatasourceTemplate_FiwareHttpClient");
            }
        }

        /// <summary>
        /// Open the specified workspace
        /// </summary>
        /// <param name="connectionPath">The path to the workspace</param>
        /// <remarks>
        /// .NET Clients access Open via the ArcGIS.Core.Data.PluginDatastore.PluginDatastore class
        /// whereas Native clients (Pro internals) access via IWorkspaceFactory
        /// </remarks>
        public override void Open(Uri connectionPath)
        {
            try
            {
                //TODO Initialize your plugin instance. Individual instances
                //of your plugin may be initialized on different threads
                // throw new NotImplementedException();

                // Initialize
                //Strictly speaking, tracking your thread id is only necessary if
                //your implementation uses internals that have thread affinity.
                _thread_id = GetCurrentThreadId();

                // 3.3.06/20231221/msGIS_FIWARE_rt_008: Datasource URI.
                // ConnectionPath of override PluginDatasourceTemplate.URI.Open is not suitable for FIWARE due to complexly build URI with parameters set while proceeding tasks on tables and entries.
                // m_UriDatasourcePath = connectionPath;
                m_DicTables = new Dictionary<string, PluginTableTemplate>();

                bool isInitialized = false;
                Task.Run(async () =>
                {
                    // 3.3.08/20240109/msGIS_FIWARE_rt_012: Init Fiware_RestApi_NetHttpClient before Plugin Datasource OpenTable/GetTableNames.
                    // If a datasource table exists OpenTable/GetTableNames is called from Pro Plugin on project start.
                    if (!await Fusion.InitAsync())
                        return;

                    Task<bool> asyncTask = Fusion.InitAsync();
                    await asyncTask.ConfigureAwait(false);

                    // Continue with the rest of the code after the task has completed
                    if (asyncTask.IsCompleted)
                    {
                        isInitialized = asyncTask.Result;
                    }
                }).GetAwaiter().GetResult();

                if (!isInitialized)
                    throw new Exception("Datasource couldn't be initialized!");
                if (connectionPath == null)
                    throw new Exception("Empty connection!");
                if (connectionPath != Fusion.m_UriDatasource.path)
                {
                    // throw new Exception($"Connection doesn't match intended datasource!{Environment.NewLine}{connectionPath.OriginalString}{Environment.NewLine}{Fusion.m_UriDatasource.path.OriginalString}");
                }
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "Open");
            }
        }

        public override void Close()
        {
            try
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
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "Close");
            }
        }

        public override PluginTableTemplate OpenTable(string name)
        {
            try
            {
                //TODO Open the given table/object in the plugin
                //data source
                // throw new NotImplementedException();

                //This is only necessary if your internals have thread affinity
                //
                //If you are using shared data (eg "static") it is your responsibility
                //to manage access to it across multiple threads.
                if (_thread_id != GetCurrentThreadId())
                {
                    throw new ArcGIS.Core.CalledOnWrongThreadException();
                }

                string tableName = name;
                if (!this.GetTableNames().Contains(tableName))
                    throw new Exception($"The table {tableName} was not found!");

                // if (m_DicTables.ContainsKey(tableName))
                if (m_DicTables.Keys.Contains(tableName))
                    throw new Exception($"The table {tableName} is ambiguous!");

                // 3.3.07/20231222/msGIS_FIWARE_rt_010: Open Plugin table and read the data.
                ProPluginTableTemplate_FiwareHttpClient proPluginTableTemplate_FiwareHttpClient = new ProPluginTableTemplate_FiwareHttpClient(Fusion.m_UriDatasource, tableName);
                // m_DicTables.Add(tableName, proPluginTableTemplate_FiwareHttpClient);
                m_DicTables[tableName] = proPluginTableTemplate_FiwareHttpClient;

                return m_DicTables[tableName];      // proPluginTableTemplate_FiwareHttpClient;
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "OpenTable");
                return null;
            }
        }

        public override IReadOnlyList<string> GetTableNames()
        {
            try
            {
                //var tableNames = new List<string>();
                //TODO Return the names of all tables in the plugin
                //data source
                // return tableNames;

                // 3.3.05/20231201/msGIS_FIWARE_rt_002: Nicht überwindbare Komplikation auf HttpClient mittels GetAsync(apiUrl) aus der abstrakten Klasse ArcPro PluginDatasourceTemplate zuzugreifen.
                // 3.3.06/20231218/msGIS_FIWARE_rt_007: ProPluginDatasource_FiwareHttpClient.
                // 3.3.06/20231221/msGIS_FIWARE_rt_008: Datasource URI.
                List<string> tableNames = null;
                // Use asynchronous call due to lack of adequate synchronous "httpResponse.Content.ReadAsStringAsync" function, even if "esriHttpClient.Get(requestUri)" function is available.
                // Esri QueuedTask.Run is not suitable in a synchronous routine with due to dead lock on desired "GetAwaiter().GetResult()" modality for fulfilled object to return.
                // asyncTask.Wait();
                // Use a separate thread to wait for the task to complete
                Task.Run(async () =>
                {
                    // Get entity types from JSON.
                    Task<List<string>> asyncTask = Fusion.m_Fiware_RestApi_NetHttpClient.ReadEntityTypesFromRestApiAsync(Fusion.m_UriDatasource);
                    await asyncTask.ConfigureAwait(false);

                    // Continue with the rest of the code after the task has completed
                    if (asyncTask.IsCompleted)
                    {
                        tableNames = asyncTask.Result;
                    }
                }).GetAwaiter().GetResult();

                return tableNames;
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "GetTableNames");
                return null;
            }
        }

        public override bool IsQueryLanguageSupported()
        {
            try
            {
                //default is false
                //bool isQueryLanguageSupported = base.IsQueryLanguageSupported();
                //return isQueryLanguageSupported;
                return true;
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "IsQueryLanguageSupported");
                return false;
            }
        }
    }
}
