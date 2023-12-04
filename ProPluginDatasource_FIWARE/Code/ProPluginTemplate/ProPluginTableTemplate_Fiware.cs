using ArcGIS.Core.Data;
using ArcGIS.Core.Data.PluginDatastore;
using ArcGIS.Core.Geometry;

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
    //     the Plugin Datastore Framework. Specifically, each instance of concrete class
    //     that implements this abstraction acts as a conduit between a data structure in
    //     a third-party data source and a ArcGIS.Core.Data.Table (or ArcGIS.Core.Data.FeatureClass)
    //     in ArcGIS Pro.
    //     A plug-in table does not necessarily correspond to a database table on the back
    //     end. It can be any data structure or format, but it presented to ArcGIS as a
    //     table.
    //     If the list of ArcGIS.Core.Data.PluginDatastore.PluginFields returned by ArcGIS.Core.Data.PluginDatastore.PluginTableTemplate.GetFields
    //     has a field whose type is ArcGIS.Core.Data.FieldType.Geometry, then this concrete
    //     implementation is considered a feature class; otherwise, a table.

    // This class is used for both tables and feature classes and must inherit from the ArcGIS.Core.Data.PluginDatastore.PluginTableTemplate base class.
    public class ProPluginTableTemplate_Fiware : PluginTableTemplate
    {
        private string m_TableName = "";

        // Summary:
        //     Initializes a new instance of the ArcGIS.Core.Data.PluginDatastore.PluginTableTemplate
        //     class.
        public ProPluginTableTemplate_Fiware(string tableName) : base()
        {
            m_TableName = tableName;
        }

        // Summary:
        //     Gets the name of this currently opened plug-in table or feature class.
        //
        // Returns:
        //     The name of this currently opened plug-in table or feature class.
        //
        // Exceptions:
        //   T:System.Exception:
        //     Signals to the framework that an exception derived from System.Exception has
        //     occurred.
        //
        // Remarks:
        //     The returned name string should be one of the values in the System.Collections.Generic.IReadOnlyList`1
        //     returned by ArcGIS.Core.Data.PluginDatastore.PluginDatasourceTemplate.GetTableNames.

        // Returns the name of the table. This string should be one of the names that is returned by GetTableNames() on the plug-in datasource concrete class.
        public override string GetName()
        {
            //TODO Get the name of this currently opened plugin table/object
            // throw new NotImplementedException();
            return m_TableName;
        }

        // Summary:
        //     Gets all of the ArcGIS.Core.Data.PluginDatastore.PluginFields for this currently
        //     opened plug-in table or feature class.
        //
        // Returns:
        //     A System.Collections.Generic.IReadOnlyList`1 of ArcGIS.Core.Data.PluginDatastore.PluginFields.
        //
        // Exceptions:
        //   T:System.Exception:
        //     Signals to the framework that an exception derived from System.Exception has
        //     occurred.
        //
        // Remarks:
        //     If the list of ArcGIS.Core.Data.PluginDatastore.PluginFields returned by this
        //     method has a field whose type is ArcGIS.Core.Data.FieldType.Geometry, then this
        //     concrete implementation is considered a feature class; otherwise, a table.

        // Returns the set of fields that are available on this table.
        // The framework will take the list of PluginField objects that are returned, wrap them up, and return them as ArcGIS.Core.Data.Field objects from the appropriate routines on TableDefinition.
        // If the list of PluginField objects contains a field whose type is FieldType.Geometry than this plug-in table is considered a feature class.
        // If no field of this type is returned, the plug-in table is treated as a non-spatial table by ArcGIS Pro.
        public override IReadOnlyList<PluginField> GetFields()
        {
            var pluginFields = new List<PluginField>();
            //TODO Get the list of PluginFields for this currently opened 
            //plugin table/object

            //special handling for OBJECTID and SHAPE
            string fieldName = "OBJECTID";
            string fieldAlias = "OID";
            FieldType fieldType = FieldType.OID;        // e.g. Geometry, String, Double, Integer, Date, etc.

            pluginFields.Add(new PluginField()
            {
                Name = fieldName,
                AliasName = fieldAlias,
                FieldType = fieldType
            });

            fieldName = "SHAPE";
            fieldAlias = "Geometry";
            fieldType = FieldType.Geometry;

            pluginFields.Add(new PluginField()
            {
                Name = fieldName,
                AliasName = fieldAlias,
                FieldType = fieldType
            });

            return pluginFields;
        }

        // Summary:
        //     Performs a non-spatial search on this plug-in table that satisfies the criteria
        //     set in the queryFilter.
        //
        // Parameters:
        //   queryFilter:
        //     A query filter that specifies the search criteria.
        //
        // Returns:
        //     A concrete instance of ArcGIS.Core.Data.PluginDatastore.PluginCursorTemplate
        //     that encapsulates the retrieved rows.
        //
        // Exceptions:
        //   T:System.Exception:
        //     Signals to the framework that an exception derived from System.Exception has
        //     occurred.
        //
        // Remarks:
        //     Please see the additional constraints that may be imposed on this method in the
        //     remarks section of ArcGIS.Core.Data.PluginDatastore.PluginDatasourceTemplate.IsQueryLanguageSupported.

        // This routine should perform a non-spatial query. More information about queries is included below.
        public override PluginCursorTemplate Search(QueryFilter queryFilter)
        {
            //TODO Perform a non-spatial search on this currently opened 
            //plugin table/object
            //Where clause will always be empty if 
            //PluginDatasourceTemplate.IsQueryLanguageSupported = false.
            throw new NotImplementedException();
        }

        // Summary:
        //     Performs a spatial search on this plug-in feature class that satisfies the criteria
        //     set in the spatialQueryFilter.
        //
        // Parameters:
        //   spatialQueryFilter:
        //     A spatial query filter that specifies the search criteria.
        //
        // Returns:
        //     A concrete instance of ArcGIS.Core.Data.PluginDatastore.PluginCursorTemplate
        //     that encapsulates the retrieved rows.
        //
        // Exceptions:
        //   T:System.Exception:
        //     Signals to the framework that an exception derived from System.Exception has
        //     occurred.
        //
        // Remarks:
        //     Please see the additional constraints that may be imposed on this method in the
        //     remarks section of ArcGIS.Core.Data.PluginDatastore.PluginDatasourceTemplate.IsQueryLanguageSupported.

        // This routine should perform a spatial query.
        // If this table is a non-spatial table rather than a feature class, this routine should throw an exception.
        public override PluginCursorTemplate Search(SpatialQueryFilter spatialQueryFilter)
        {
            //TODO Perform a spatial search on this currently opened 
            //plugin table/object
            //Where clause will always be empty if 
            //PluginDatasourceTemplate.IsQueryLanguageSupported = false.
            throw new NotImplementedException();
        }

        public override GeometryType GetShapeType()
        {
            //TODO return the correct GeometryType if the plugin table
            //is a feature class
            return GeometryType.Unknown;
        }

    }
}
