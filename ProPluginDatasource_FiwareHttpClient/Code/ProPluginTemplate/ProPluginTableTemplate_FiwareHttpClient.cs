using msGIS.ProApp_Common_FIWARE_3x;

using ArcGIS.Core.Data;
using ArcGIS.Core.Data.Exceptions;
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
        // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
        // 3.3.07/20231222/msGIS_FIWARE_rt_009: Plugin integration.
        private readonly string m_ModuleName = "ProPluginTableTemplate_FiwareHttpClient";

        private Fiware_RestApi_NetHttpClient.UriDatasource m_UriDatasource;
        private string m_TableName;
        private DataTable _table;
        private SpatialReference m_SpatialReference;

        public ProPluginTableTemplate_FiwareHttpClient(Fiware_RestApi_NetHttpClient.UriDatasource uriDatasource, string tableName, SpatialReference spatialReference = null) : base()
        {
            try
            {
                m_UriDatasource = uriDatasource;
                m_TableName = tableName;
                m_SpatialReference = spatialReference ?? SpatialReferences.WGS84;
                // Open();
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "ProPluginTableTemplate_FiwareHttpClient");
            }
        }


        #region IDisposable

        private bool _disposed = false;
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "Dispose");
            }
        }

        private void Dispose(bool disposing)
        {
            try
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
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "Dispose");
            }
        }
        #endregion


        public override IReadOnlyList<PluginField> GetFields()
        {
            try
            {
                var pluginFields = new List<PluginField>();
                //TODO Get the list of PluginFields for this currently opened 
                //plugin table/object
                return pluginFields;
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "GetFields");
                return null;
            }
        }

        public override string GetName()
        {
            try
            {
                //TODO Get the name of this currently opened plugin table/object
                //throw new NotImplementedException();
                return m_TableName;
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "GetName");
                return string.Empty;
            }
        }

        public override PluginCursorTemplate Search(QueryFilter queryFilter)
        {
            try
            {
                //TODO Perform a non-spatial search on this currently opened 
                //plugin table/object
                //Where clause will always be empty if 
                //PluginDatasourceTemplate.IsQueryLanguageSupported = false.
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "Search");
                return null;
            }
        }

        public override PluginCursorTemplate Search(SpatialQueryFilter spatialQueryFilter)
        {
            try
            {
                //TODO Perform a spatial search on this currently opened 
                //plugin table/object
                //Where clause will always be empty if 
                //PluginDatasourceTemplate.IsQueryLanguageSupported = false.
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "Search");
                return null;
            }
        }

        public override GeometryType GetShapeType()
        {
            try
            {
                //TODO return the correct GeometryType if the plugin table
                //is a feature class
                return GeometryType.Unknown;
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "GetShapeType");
                return GeometryType.Unknown;
            }
        }
    }
}
