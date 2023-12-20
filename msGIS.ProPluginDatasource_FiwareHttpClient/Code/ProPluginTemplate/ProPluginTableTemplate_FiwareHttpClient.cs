using ArcGIS.Core.Data;
using ArcGIS.Core.Data.PluginDatastore;
using ArcGIS.Core.Geometry;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msGIS.ProPluginDatasource_FiwareHttpClient
{
    public class ProPluginTableTemplate_FiwareHttpClient : PluginTableTemplate
    {
        // 3.3.06/20231218/msGIS_FIWARE_rt_007: ProPluginDatasource_FiwareHttpClient.
        private Uri m_UriDatasourcePath;
        private string m_TableName;
        private DataTable _table;
        private SpatialReference m_SpatialReference;

        public ProPluginTableTemplate_FiwareHttpClient(Uri uriDatasourcePath, string tableName, SpatialReference spatialReference = null) : base()
        {
            m_UriDatasourcePath = uriDatasourcePath;
            m_TableName = tableName;
            m_SpatialReference = spatialReference ?? SpatialReferences.WGS84;
            // Open();
        }


        #region IDisposable

        private bool _disposed = false;
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            //TODO free unmanaged resources here
            System.Diagnostics.Debug.WriteLine("Table being disposed");

            if (_disposed)
                return;

            if (disposing)
            {
                _table?.Clear();
                _table = null;
                m_SpatialReference = null;
            }
            _disposed = true;
        }
        #endregion


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
            //throw new NotImplementedException();
            return m_TableName;
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
