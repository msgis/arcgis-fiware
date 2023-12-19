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
        // 3.3.06/20231218/msGIS_FIWARE_rt_007: ProPluginDatasource_FiwareHttpClient.

        public override PluginRow GetCurrentRow()
        {
            var listOfRowValues = new List<object>();
            //TODO collect the values for the current row

            return new PluginRow(listOfRowValues);
        }

        public override bool MoveNext()
        {
            //TODO determine if there are more rows
            throw new NotImplementedException();
        }
    }
}
