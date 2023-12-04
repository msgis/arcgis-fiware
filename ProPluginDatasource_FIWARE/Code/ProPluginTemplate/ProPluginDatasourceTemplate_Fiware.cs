﻿using ArcGIS.Core.Data;
using ArcGIS.Core.Data.PluginDatastore;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msGIS.ProPluginDatasource_FIWARE
{
    // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.

    // Summary:
    //     This abstract class serves as one of the key extensibility points that comprise
    //     the Plugin Datastore Framework. Specifically, each instance of a concrete class
    //     that implements this abstraction acts as a conduit between a third-party data
    //     source and ArcGIS Pro via the deployment of a plug-in data source add-in.
    //     Currently, the framework only supports ArcGIS.Core.Data.DatasetType.Tables and
    //     ArcGIS.Core.Data.DatasetType.FeatureClasss in a read-only manner.

    // Class to implement a data store. This class must inherit from the PluginDatasourceTemplate base class.
    public class ProPluginDatasourceTemplate_Fiware : PluginDatasourceTemplate
    {
        // Initializes a new instance of the ArcGIS.Core.Data.PluginDatastore.PluginDatasourceTemplate class.
        public ProPluginDatasourceTemplate_Fiware() : base()
        {
            // Set the name and category for your custom data source
            //Name = "My Custom Data Source";
            //Category = "My Category";
        }

        private string m_DatasourcePath = "";
        private Dictionary<string, PluginTableTemplate> m_DicTypeTables;
        private IReadOnlyList<string> m_TableNames = null;

        // Summary:
        //     Formally opens the Plugin Datasource in order for the new data format to be integrated
        //     into ArcGIS Pro.
        //
        // Parameters:
        //   connectionPath:
        //     The connection path to the actual data source.
        //
        // Exceptions:
        //   T:System.Exception:
        //     Signals to the Framework that an exception derived from System.Exception has
        //     occurred.
        //
        // Remarks:
        //     • When the Plugin Datasource framework calls this method, the concrete implementation
        //     is expected to perform any necessary instantiation and initialization to commence
        //     the data integration process.
        //     • When a Plugin Datasource add-in is loaded into the system at runtime, the framework
        //     may instantiate multiple instances of the concrete PluginDatasourceTemplate implementation.
        //     In particular, when a plug-in data source with a specific ArcGIS.Core.Data.PluginDatastore.PluginDatasourceConnectionPath.PluginIdentifier
        //     is used to open a specific connectionPath, the framework will create a new instance
        //     of this concrete implementation followed by calling this Open method. If there
        //     are pertinent data that must be reflected across all instances of this concrete
        //     implementation, e.g., if the value of ArcGIS.Core.Data.PluginDatastore.PluginDatasourceTemplate.IsQueryLanguageSupported
        //     depends on a given data source associated with the connectionPath and if said
        //     value must be reflected across all instances in order for the framework to behave
        //     properly, then it is the responsibility of this concrete implementation to ensure
        //     the data are properly shared and/or updated.

        // This method should establish a connection to the underlying data source specified by connectionPath.
        // For example, if this Plugin Datasource reads MySQL databases, this method would establish a connection to a MySQL database.
        public override void Open(Uri connectionPath)
        {
            // string connectionString = $"Server={server};Database={database};User={user};Password={password};";
            // DatabaseConnectionProperties databaseConnectionProperties = new DatabaseConnectionProperties(EnterpriseDatabaseType.Unknown);

            //TODO Initialize your plugin instance. Individual instances
            //of your plugin may be initialized on different threads
            // throw new NotImplementedException();

            // 3.3.05/20231201/msGIS_FIWARE_rt_002: Nicht überwindbare Komplikation auf HttpClient mittels GetAsync(apiUrl) aus der abstrakten Klasse ArcPro PluginDatasourceTemplate zuzugreifen.
            m_DatasourcePath = connectionPath.ToString();
            m_TableNames = Fusion.m_ListEntityTypes;

            m_DicTypeTables = new Dictionary<string, PluginTableTemplate>();
            /*
            foreach (var tabName in GetTableNames())
            {
                m_DicTypeTables.Add(tabName, new ProPluginTableTemplate_Fiware(tabName));
            }
            */
        }

        // Summary:
        //     Formally closes the Plugin Datasource. This operation is the opposite of ArcGIS.Core.Data.PluginDatastore.PluginDatasourceTemplate.Open(System.Uri).
        //
        // Remarks:
        //     • When the framework calls this method, the concrete implementation is expected
        //     to perform any necessary housekeeping and cleanup operation to close the plug-in
        //     data source.
        //     • The framework will ignore any exception that is raised by the concrete implementation.

        // This method closes the connection to the underlying data source that was established by the Open call.
        // It should be used to release database connections, close file handles, and similar cleanup.
        public override void Close()
        {
            //TODO Cleanup required to close the plugin 
            //data source instance
            // throw new NotImplementedException();

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
                /*
                foreach (var table in m_DicTypeTables.Values)
                {
                    // ((ProPluginTableTemplate_Fiware)table).Dispose();
                }
                */
                m_DicTypeTables.Clear();
                m_DicTypeTables = null;
            }

        }

        // Summary:
        //     Gets an instance of concrete class that implements the ArcGIS.Core.Data.PluginDatastore.PluginTableTemplate
        //     abstraction associated with name in the plug-in data source.
        //
        // Parameters:
        //   name:
        //     The name of a data structure entity that is exposed to ArcGIS as a table or feature
        //     class via the Plugin Datasource.
        //
        // Returns:
        //     An instance of the concrete class that implements the ArcGIS.Core.Data.PluginDatastore.PluginTableTemplate
        //     abstraction.
        //
        // Exceptions:
        //   T:System.Exception:
        //     Signals to the framework that an exception derived from System.Exception has
        //     occurred.
        //
        // Remarks:
        //     Any input name argument for this method should be one of the returned values
        //     in ArcGIS.Core.Data.PluginDatastore.PluginDatasourceTemplate.GetTableNames.

        // This routine conceptually opens a table, although again the backing datastore need not be a database table.
        // The developer should create and return a plug-in table, which is a class that inherits from PluginTableTemplate.
        // Conceptually, a .NET client calls the PluginDatastore.OpenTable() method which calls this routine,
        // wraps up the returned PluginTableTemplate concrete instance as a ArcGIS.Core.Data.Table or ArcGIS.Core.Data.FeatureClass.
        public override PluginTableTemplate OpenTable(string tableName)
        {
            //TODO Open the given table/object in the plugin data source
            // throw new NotImplementedException();
            if (!this.GetTableNames().Contains(tableName))
                throw new Exception($"The table {tableName} was not found!");

            ProPluginTableTemplate_Fiware proPluginTableTemplate_Fiware = new ProPluginTableTemplate_Fiware(tableName);
            if (m_DicTypeTables.ContainsKey(tableName))
                throw new Exception($"The table {tableName} is ambiguous!");
            m_DicTypeTables.Add(tableName, proPluginTableTemplate_Fiware);
            return proPluginTableTemplate_Fiware;
        }

        // Summary:
        //     Gets the name of all the tables and feature classes that exist in the currently
        //     opened plug-in data source.
        //
        // Returns:
        //     A System.Collections.Generic.IReadOnlyList`1 of table names.
        //
        // Exceptions:
        //   T:System.Exception:
        //     Signals to the framework that an exception derived from System.Exception has
        //     occurred.
        //
        // Remarks:
        //     These plug-in tables and feature classes do not necessarily correspond to database
        //     tables on the back end. They can be any data structure or format in the third-party
        //     data source, but are presented to ArcGIS as a table via the Plugin Datasource.
        //     The concrete implementation will decide whether the returned table names are
        //     fully qualified or unqualified as long as the names can be successfully used
        //     as arguments to ArcGIS.Core.Data.PluginDatastore.PluginDatasourceTemplate.OpenTable(System.String).

        // GetTableNames should return a list of all of the tables and feature classes that can be opened by the Plugin Datasource.
        // Remember that the underlying data source might not be composed of database tables; these are entities that exposed to ArcGIS Pro as tables.
        // For example, a Plugin Datasource that exposes an Excel spreadsheet as a datasource might treat each pane in the spreadsheet as an individual table.
        // The format of the strings returned is up to the developer; the only stipulation is that the strings returned by GetTableNames can be used in calls to OpenTable.
        public override IReadOnlyList<string> GetTableNames()
        {
            // 3.3.05/20231201/msGIS_FIWARE_rt_002: Nicht überwindbare Komplikation auf HttpClient mittels GetAsync(apiUrl) aus der abstrakten Klasse ArcPro PluginDatasourceTemplate zuzugreifen.
            // var tableNames = new List<string>();
            IReadOnlyList<string> tableNames = m_TableNames;        // new List<string>();

            /*
            // asyncTask.Wait();
            // Use a separate thread to wait for the task to complete
            Task.Run(async () =>
            {
                Task<List<string>> asyncTask = RestApi_Fiware.ReadEntityTypesFromRestApiAsync(m_DatasourcePath);
                await asyncTask.ConfigureAwait(false);

                // Continue with the rest of the code after the task has completed
                List<string> listEntityTypes = asyncTask.Result;
                if ((asyncTask.IsCompleted) && (listEntityTypes != null))
                    m_TableNames = listEntityTypes;
            }).GetAwaiter().GetResult();
            */

            //TODO Return the names of all tables in the plugin
            //data source
            return tableNames;
        }

        // Summary:
        //     Gets a value indicating whether the underlying data source supports a query language
        //     (e.g., SQL).
        //
        // Returns:
        //     true if the underlying data source supports a query language; otherwise, false.
        //     The default is false.
        //
        // Remarks:
        //     If the underlying data source supports a query language (e.g., SQL) by returning
        //     true, the framework will exhibit the following behavior:
        //     • If ArcGIS.Core.Data.QueryFilter.WhereClause is specified in the argument to
        //     ArcGIS.Core.Data.Table.Search(ArcGIS.Core.Data.QueryFilter,System.Boolean), the
        //     framework will relay said where clause by setting ArcGIS.Core.Data.QueryFilter.WhereClause
        //     with the same value when it calls either ArcGIS.Core.Data.PluginDatastore.PluginTableTemplate.Search(ArcGIS.Core.Data.QueryFilter)
        //     or ArcGIS.Core.Data.PluginDatastore.PluginTableTemplate.Search(ArcGIS.Core.Data.SpatialQueryFilter).
        //     If the underlying data source does not support a query language (e.g., SQL) by
        //     returning false, the framework will exhibit the following behavior:
        //     • The framework will not relay the ArcGIS.Core.Data.QueryFilter.WhereClause regardless
        //     of whether or not a where clause is specified by the user. In other words, the
        //     ArcGIS.Core.Data.QueryFilter.WhereClause is an empty string when the framework
        //     calls either ArcGIS.Core.Data.PluginDatastore.PluginTableTemplate.Search(ArcGIS.Core.Data.QueryFilter)
        //     or ArcGIS.Core.Data.PluginDatastore.PluginTableTemplate.Search(ArcGIS.Core.Data.SpatialQueryFilter).
        //     • However, if a user-specified ArcGIS.Core.Data.QueryFilter.WhereClause exists
        //     and said where clause has a subexpression involving a ArcGIS.Core.Data.Field
        //     of type ArcGIS.Core.Data.FieldType.OID, the framework will transform the objectID
        //     value(s) into a list for ArcGIS.Core.Data.QueryFilter.ObjectIDs. For example:
        //     QueryFilter queryFilter = new QueryFilter()
        //     {
        //       WhereClause = String.Format(“{0} = {1}”, “OBJECTID”, 2888)  // the OBJECTID
        //     field is of type FieldType.OID.
        //     };
        //     using (RowCursor rowCursor = featureClass.Search(queryFilter))
        //     {}
        //     If ArcGIS.Core.Data.QueryFilter.ObjectIDs is specified by the user in their ArcGIS.Core.Data.QueryFilter,
        //     the framework will relay said ObjectIDs by setting ArcGIS.Core.Data.QueryFilter.ObjectIDs
        //     in the ArcGIS.Core.Data.QueryFilter that is passed to ArcGIS.Core.Data.PluginDatastore.PluginTableTemplate.Search(ArcGIS.Core.Data.QueryFilter).

        // This routine should return true if the underlying data source supports a query language.
        // The default is false. See the Queries section of this document for more information about queries.
        public override bool IsQueryLanguageSupported()
        {
            return base.IsQueryLanguageSupported();
        }

        // Summary:
        //     Gets a string description for datasetType.
        //
        // Parameters:
        //   datasetType:
        //     The data set type.
        //
        // Returns:
        //     A string description for datasetType. The default is datasetType.ToString().
        //
        // Remarks:
        //     Currently, the framework only supports ArcGIS.Core.Data.DatasetType.Table and
        //     ArcGIS.Core.Data.DatasetType.FeatureClass in a read-only manner.

        // This routine is used to return a description of the datasources supported by this plug-in.
        // For example, if your plug-in supports MongoDB databases, you might return “MongoDB database” and “MongoDB databases”, depending on the value of the inPluralForm parameter.
        // If no implementation is provided, the default return values are “Plugin Datasources” and “Plugin Datasource” for inPluralForm equal to true and false respectively.
        public override string GetDatasetDescription(DatasetType datasetType)
        {
            return base.GetDatasetDescription(datasetType);
        }

        // Summary:
        //     Gets a string description for the plug-in data source depending on inPluralForm.
        //
        // Parameters:
        //   inPluralForm:
        //     A value indicating whether the description should be in singular or plural form.
        //
        // Returns:
        //     A name description for the plug-in data source. The default is Plugin Datasources
        //     if inPluralForm is true, Plugin Datasource if false.

        // This routine is used to return a description of the datasets that are returned by this plug-in.
        // Currently, the framework only supports DatasetType.Table and DatasetType.FeatureClass.
        // If no implementation is provided, the default return values are “Table” and “Feature Class.”
        // Remember that while ArcGIS Pro treats these datasets as if they were tables, the actual implementation may not correspond to a database table.
        // This routine allows ArcGIS Pro to use different, more accurate, terminology.
        public override string GetDatasourceDescription(bool inPluralForm)
        {
            return base.GetDatasourceDescription(inPluralForm);
        }

        // Summary:
        //     Gets a value indicating whether the data source associated with connectionPath
        //     can be opened by this concrete implementation.
        //
        // Parameters:
        //   connectionPath:
        //     The connection path to the actual data source.
        //
        // Returns:
        //     true if the data source associated with connectionPath can be opened; otherwise,
        //     false. The default is false.
        //
        // Remarks:
        //     From time to time, some subsystems in ArcGIS Pro may need to query this method
        //     to determine whether a path to a data source can be opened by a data store, plugin
        //     or otherwise. For example, the geoprocessing subsystem may call this method as
        //     part of its "post tool execution". If this method returns true, geoprocessing
        //     may proceed to instantiate this plug-in data source in order to perform some
        //     system-specific tasks.

        // Some of the ArcGIS Pro subsystems may need to query this method to determine if a given data source can be opened by this data store.
        // For example, if your Plugin Datastore exposes an Excel spreadsheet as a datasource, you should return true if the connectionPath URI represents an Excel file.
        public override bool CanOpen(Uri connectionPath)
        {
            bool canOpenAsKnownDataSource = base.CanOpen(connectionPath);
            return canOpenAsKnownDataSource;
        }

    }
}
