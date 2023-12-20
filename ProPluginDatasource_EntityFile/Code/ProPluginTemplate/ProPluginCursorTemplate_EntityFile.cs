using ArcGIS.Core.Data;
using ArcGIS.Core.Data.PluginDatastore;
using ArcGIS.Core.Geometry;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msGIS.ProPluginDatasource_EntityFile
{
    // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
    // 3.3.05/20231207/msGIS_FIWARE_rt_005: ProPluginDatasource Integration for SimplePoint File-Format.

    public class ProPluginCursorTemplate_EntityFile : PluginCursorTemplate
    {
        private Queue<long> _oids;
        private IEnumerable<string> _columns;
        private SpatialReference _srout;
        private IPluginRowProvider _provider;
        private long _current = -1;
        private static readonly object _lock = new object();

        internal ProPluginCursorTemplate_EntityFile(IPluginRowProvider provider, IEnumerable<long> oids, IEnumerable<string> columns, SpatialReference srout)
        {
            _provider = provider;
            _oids = new Queue<long>(oids);
            _columns = columns;
            _srout = srout;
        }

        /// <summary>
        /// Get the current row
        /// </summary>
        /// <returns><see cref="PluginRow"/></returns>
        public override PluginRow GetCurrentRow()
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

        /// <summary>
        /// Advance the cursor to the next row
        /// </summary>
        /// <returns>True if there was another row</returns>
        public override bool MoveNext()
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
    }
}
