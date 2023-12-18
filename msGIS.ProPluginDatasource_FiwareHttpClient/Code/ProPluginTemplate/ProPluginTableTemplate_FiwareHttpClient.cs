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
    public class ProPluginTableTemplate_FiwareHttpClient : PluginTableTemplate
    {

        public override IReadOnlyList<PluginField> GetFields()
        {
            var pluginFields = new List<PluginField>();
            //TODO Get the list of PluginFields for this currently opened 
            //plugin table/object
            return pluginFields;
        }

        public override string GetName()
        {
            //TODO Get the name of this currently opened plugin table/object
            throw new NotImplementedException();
        }

        public override PluginCursorTemplate Search(QueryFilter queryFilter)
        {
            //TODO Perform a non-spatial search on this currently opened 
            //plugin table/object
            //Where clause will always be empty if 
            //PluginDatasourceTemplate.IsQueryLanguageSupported = false.
            throw new NotImplementedException();
        }

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
