using msGIS.ProApp_Common_FIWARE_3x;

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
    public class ProPluginCursorTemplate_FiwareHttpClient : PluginCursorTemplate
    {
        // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
        // 3.3.07/20231222/msGIS_FIWARE_rt_009: Plugin integration.
        private readonly string m_ModuleName = "ProPluginCursorTemplate_FiwareHttpClient";

        private Queue<long> _oids;
        private IEnumerable<string> _columns;
        private SpatialReference _srout;
        private IPluginRowProvider _provider;
        private long _current = -1;
        private static readonly object _lock = new object();

        internal ProPluginCursorTemplate_FiwareHttpClient(IPluginRowProvider provider, IEnumerable<long> oids, IEnumerable<string> columns, SpatialReference srout)
        {
            try
            {
                _provider = provider;
                _oids = new Queue<long>(oids);
                _columns = columns;
                _srout = srout;
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "ProPluginCursorTemplate_FiwareHttpClient");
            }
        }

        public override PluginRow GetCurrentRow()
        {
            try
            {
                //var listOfRowValues = new List<object>();
                //TODO collect the values for the current row
                //return new PluginRow(listOfRowValues);

                long id = -1;
                //The lock shouldn't be necessary if your cursor is a per thread instance
                //(like the sample is)
                lock (_lock)
                {
                    id = _current;
                }
                return _provider.FindRow(id, _columns, _srout);
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "GetCurrentRow");
                return null;
            }
        }

        public override bool MoveNext()
        {
            try
            {
                //TODO determine if there are more rows
                //throw new NotImplementedException();

                if (_oids.Count == 0)
                    return false;

                //The lock shouldn't be necessary if your cursor is a per thread instance
                //(like the sample is)
                lock (_lock)
                {
                    _current = _oids.Dequeue();
                }
                return true;
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "MoveNext");
                return false;
            }
        }
    }
}
