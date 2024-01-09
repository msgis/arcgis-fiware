using msGIS.ProApp_Common_FIWARE_3x;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ArcGIS.Core.Data;
using ArcGIS.Core.Data.Exceptions;
using ArcGIS.Core.Data.PluginDatastore;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msGIS.ProPluginDatasource_FiwareHttpClient
{
    // Summary:
    // (Custom) interface the sample uses to extract row information from the plugin table
    internal interface IPluginRowProvider
    {
        PluginRow FindRow(long oid, IEnumerable<string> columnFilter, SpatialReference sr);
    }

    public class ProPluginTableTemplate_FiwareHttpClient : PluginTableTemplate, IDisposable, IPluginRowProvider
    {
        // 3.3.05/20231128/msGIS_FIWARE_rt_001: Integration ArcGIS PRO.
        // 3.3.07/20231222/msGIS_FIWARE_rt_009: Plugin integration.
        private readonly string m_ModuleName = "ProPluginTableTemplate_FiwareHttpClient";

        private Fiware_RestApi_NetHttpClient.UriDatasource m_UriDatasource;
        private string m_TableName;
        private DataTable _table;
        private RBush.RBush<RBushCoord3D> _rtree;
        private RBush.Envelope _extent;
        private Envelope _gisExtent;
        private SpatialReference _sr;
        private bool _hasZ = false;

        public ProPluginTableTemplate_FiwareHttpClient(Fiware_RestApi_NetHttpClient.UriDatasource uriDatasource, string tableName) : base()
        {
            try
            {
                m_UriDatasource = uriDatasource;
                m_TableName = tableName;

                _rtree = new RBush.RBush<RBushCoord3D>();
                _sr = uriDatasource.spatialReference ?? SpatialReferences.WGS84;

                OpenTableData();
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
                    _rtree?.Clear();
                    _rtree = null;
                    _sr = null;
                    _gisExtent = null;
                }
                _disposed = true;
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "Dispose");
            }
        }
        #endregion


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

        public override bool IsNativeRowCountSupported() => true;

        public override long GetNativeRowCount() => _rtree?.Count ?? _table.Rows.Count;

        public override PluginCursorTemplate Search(QueryFilter queryFilter)
        {
            try
            {
                //TODO Perform a non-spatial search on this currently opened 
                //plugin table/object
                //Where clause will always be empty if 
                //PluginDatasourceTemplate.IsQueryLanguageSupported = false.
                // throw new NotImplementedException();

                return this.SearchInternal(queryFilter);
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
                // throw new NotImplementedException();

                return this.SearchInternal(spatialQueryFilter);
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
                // return GeometryType.Unknown;

                //Note: empty tables treated as non-geometry
                return _table.Columns.Contains(Fusion.m_DataColumn_Geom) ? GeometryType.Point : GeometryType.Unknown;
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "GetShapeType");
                return GeometryType.Unknown;
            }
        }

        public override Envelope GetExtent()
        {
            try
            {
                if (this.GetShapeType() != GeometryType.Unknown)
                {
                    if (_gisExtent == null)
                    {
                        _gisExtent = _extent.ToEsriEnvelope(_sr, _hasZ);
                    }
                }
                return _gisExtent;
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "GetExtent");
                return null;
            }
        }

        public override IReadOnlyList<PluginField> GetFields()
        {
            try
            {
                var pluginFields = new List<PluginField>();
                // TODO Get the list of PluginFields for this currently opened 
                // plugin table/object

                foreach (var col in _table.Columns.Cast<DataColumn>())
                {
                    var fieldType = ArcGIS.Core.Data.FieldType.String;
                    // special handling for OBJECTID and SHAPE
                    if (col.ColumnName == Fusion.m_DataColumn_ID)
                    {
                        fieldType = ArcGIS.Core.Data.FieldType.OID;
                    }
                    else if (col.ColumnName == Fusion.m_DataColumn_Geom)
                    {
                        fieldType = ArcGIS.Core.Data.FieldType.Geometry;
                    }
                    else if ((col.ColumnName == Fusion.m_DataColumn_X) || (col.ColumnName == Fusion.m_DataColumn_Y))
                    {
                        // columns: X or Y
                        fieldType = ArcGIS.Core.Data.FieldType.Double;
                    }

                    pluginFields.Add(new PluginField()
                    {
                        Name = col.ColumnName,
                        AliasName = col.Caption,
                        FieldType = fieldType
                    });
                }

                return pluginFields;
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "GetFields");
                return null;
            }
        }

        #region IPluginRowProvider

        /// <summary>
        /// Custom interface specific to the way the sample is implemented.
        /// </summary>
        public PluginRow FindRow(long oid, IEnumerable<string> columnFilter, SpatialReference srout)
        {
            try
            {
                Geometry shape = null;

                List<object> values = new List<object>();
                var row = _table.Rows.Find(oid);
                //The order of the columns in the returned rows ~must~ match
                //GetFields. If a column is filtered out, an empty placeholder must
                //still be provided even though the actual value is skipped
                var columnNames = this.GetFields().Select(col => col.Name.ToUpper()).ToList();

                foreach (var colName in columnNames)
                {
                    if (columnFilter.Contains(colName))
                    {
                        // special handling for shape
                        if (colName == Fusion.m_DataColumn_Geom)
                        {
                            var buffer = row[Fusion.m_DataColumn_Geom] as Byte[];
                            shape = MapPointBuilderEx.FromEsriShape(buffer, _sr);
                            if (srout != null)
                            {
                                if (!srout.Equals(_sr))
                                    shape = GeometryEngine.Instance.Project(shape, srout);
                            }
                            values.Add(shape);
                        }
                        else
                        {
                            values.Add(row[colName]);
                        }
                    }
                    else
                    {
                        values.Add(System.DBNull.Value);//place holder
                    }
                }
                return new PluginRow() { Values = values };
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "FindRow");
                return null;
            }
        }

        #endregion IPluginRowProvider

        #region Private

        private PluginCursorTemplate SearchInternal(QueryFilter qf)
        {
            try
            {
                var oids = this.ExecuteQuery(qf);
                var columns = this.GetQuerySubFields(qf);

                return new ProPluginCursorTemplate_FiwareHttpClient(this,
                                                oids,
                                                columns,
                                                qf.OutputSpatialReference);
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "SearchInternal");
                return null;
            }
        }

        /// <summary>
        /// Implement querying with a query filter
        /// </summary>
        /// <param name="qf"></param>
        /// <returns></returns>
        private List<long> ExecuteQuery(QueryFilter qf)
        {
            try
            {
                // are we empty?
                if (_table.Rows.Count == 0)
                    return new List<long>();

                SpatialQueryFilter sqf = null;
                if (qf is SpatialQueryFilter)
                {
                    sqf = qf as SpatialQueryFilter;
                }

                List<long> result = new List<long>();
                bool emptyQuery = true;

                // fidset - this takes precedence over anything else in
                // the query. If a fid set is specified then all selections
                // for the given query are intersections from the fidset
                if (qf.ObjectIDs.Count() > 0)
                {
                    emptyQuery = false;

                    result = null;
                    result = _table.AsEnumerable().Where(
                      row => qf.ObjectIDs.Contains((long)row[Fusion.m_DataColumn_ID]))
                      .Select(row => (long)row[Fusion.m_DataColumn_ID]).ToList();

                    // anything selected?
                    if (result.Count() == 0)
                    {
                        //no - specifying a fidset trumps everything. The client
                        //specified a fidset and nothing was selected so we are done
                        return result;
                    }
                }

                // where clause
                if (!string.IsNullOrEmpty(qf.WhereClause))
                {
                    emptyQuery = false;
                    var sort = Fusion.m_DataColumn_ID;          // default
                    if (!string.IsNullOrEmpty(qf.PostfixClause))
                    {
                        // The underlying System.Data.DataTable used by the sample supports "ORDER BY"
                        // It should be a comma-separated list of column names and a default direction
                        // COL1 ASC, COL2 DESC  (note: "ASC" is not strictly necessary)
                        // Anything else and there will be an exception
                        sort = qf.PostfixClause;
                    }

                    // do the selection
                    var oids = _table.Select(qf.WhereClause, sort)
                                 .Select(row => (long)row[Fusion.m_DataColumn_ID]).ToList();

                    // consolidate whereclause selection with fidset
                    if (result.Count > 0 && oids.Count() > 0)
                    {
                        var temp = result.Intersect(oids).ToList();
                        result = null;
                        result = temp;
                    }
                    else
                    {
                        result = null;
                        result = oids;
                    }

                    // anything selected?
                    if (result.Count() == 0)
                    {
                        //no - where clause returned no rows or returned no rows
                        //common to the specified fidset
                        return result;
                    }
                }

                // filter geometry for spatial select
                if (sqf != null)
                {
                    if (sqf.FilterGeometry != null)
                    {
                        emptyQuery = false;

                        bool filterIsEnvelope = sqf.FilterGeometry is Envelope;
                        // search spatial index first
                        var extent = sqf.FilterGeometry.Extent;
                        var candidates = _rtree.Search(extent.ToRBushEnvelope());

                        // consolidate filter selection with current fidset
                        if (result.Count > 0 && candidates.Count > 0)
                        {
                            var temp = candidates.Where(pt => result.Contains(pt.ObjectID)).ToList();
                            candidates = null;
                            candidates = temp;
                        }
                        // anything selected?
                        if (candidates.Count == 0)
                        {
                            // no - filter query returned no rows or returned no rows
                            // common to the specified fidset
                            return new List<long>();
                        }

                        // o we need to refine the spatial search?
                        if (filterIsEnvelope &&
                          (sqf.SpatialRelationship == SpatialRelationship.Intersects ||
                          sqf.SpatialRelationship == SpatialRelationship.IndexIntersects ||
                          sqf.SpatialRelationship == SpatialRelationship.EnvelopeIntersects))
                        {
                            // no. This is our final list
                            return candidates.Select(pt => pt.ObjectID).OrderBy(oid => oid).ToList();
                        }

                        // refine based on the exact geometry and relationship
                        List<long> oids = new List<long>();
                        foreach (var candidate in candidates)
                        {
                            if (GeometryEngine.Instance.HasRelationship(
                                    sqf.FilterGeometry, candidate.ToMapPoint(_sr),
                                      sqf.SpatialRelationship))
                            {
                                oids.Add(candidate.ObjectID);
                            }
                        }
                        // anything selected?
                        if (oids.Count == 0)
                        {
                            // no - further processing of the filter geometry query
                            // returned no rows
                            return new List<long>();
                        }
                        result = null;
                        // oids has already been consolidated with any specified fidset
                        result = oids;
                    }
                }

                // last chance - did we execute any type of query?
                if (emptyQuery)
                {
                    // no - the default is to return all rows
                    result = null;
                    result = _table.Rows.Cast<DataRow>()
                      .Select(row => (long)row[Fusion.m_DataColumn_ID]).OrderBy(x => x).ToList();
                }
                return result;
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "ExecuteQuery");
                return null;
            }
        }

        private List<string> GetQuerySubFields(QueryFilter qf)
        {
            try
            {
                //Honor Subfields in Query Filter
                string columns = qf.SubFields ?? "*";
                List<string> subFields;
                if (columns == "*")
                {
                    subFields = this.GetFields().Select(col => col.Name.ToUpper()).ToList();
                }
                else
                {
                    var names = columns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    subFields = names.Select(n => n.ToUpper()).ToList();
                }

                return subFields;
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "GetQuerySubFields");
                return null;
            }
        }

        #endregion Private

        #region Open

        private void OpenTableData()
        {
            try
            {
                bool taskResult = false;
                Task.Run(async () =>
                {
                    Task<bool> asyncTask = GetTableDataAsync();
                    await asyncTask.ConfigureAwait(false);

                    // Continue with the rest of the code after the task has completed
                    if (asyncTask.IsCompleted)
                    {
                        taskResult = asyncTask.Result;
                    }
                }).GetAwaiter().GetResult();
                if (!taskResult)
                    throw new Exception("Failed to get table data!");
            }
            catch (Exception ex)
            {
                Fusion.m_Messages.PushEx(ex, m_ModuleName, "OpenTableData");
            }
        }

        private async Task<bool> GetTableDataAsync()
        {
            Helper_Working m_Helper_Working = null;
            try
            {
                // 3.3.07/20231222/msGIS_FIWARE_rt_010: Open Plugin table and read the data.
                string entityType = m_TableName;

                // 3.3.08/20240109/msGIS_FIWARE_rt_011: Progress ERROR: The calling thread must be STA, because many UI components require this.
                // Can't use Helper_Progress - QueuedTask blocks the asyncTask!
                // ERROR: The calling thread must be STA, because many UI components require this.
                // Single-Threaded Apartments (STAs) In an STA, only the thread that created the apartment may access the objects in it. A thread can't access other apartments or alter the concurrency model of an apartment it created.
                Fusion.m_Global.AppDispatcher.Invoke(() =>
                {
                    m_Helper_Working = new Helper_Working(Fusion.m_Global, Fusion.m_Messages, Fusion.m_Helper_Framework);
                    m_Helper_Working.ShowDelayedWorkingAsyncVoid(entityType);
                });

                JArray jArrayEntities = await Fusion.m_Fiware_RestApi_NetHttpClient.GetEntitiesFromRestApiAsync(m_UriDatasource, entityType);
                if (jArrayEntities == null)
                    return false;
                if (jArrayEntities.Count == 0)
                    await Fusion.m_Messages.AlertAsyncMsg("No entities acquired!", "GetTableDataAsync");

                List<MapPoint> listFeatures = await Fusion.m_Fiware_RestApi_NetHttpClient.BuildFeaturesFromJsonEntitiesAsync(jArrayEntities);
                if (listFeatures == null)
                    return false;


                // Initialize our data table
                // Columns (0=OBJECTID, 1=POINT_X, 2=POINT_Y)
                _table = new DataTable();
                //dataTable.PrimaryKey = new DataColumn(Fusion.m_DataColumn_ID, typeof(long));
                var oid = new DataColumn(Fusion.m_DataColumn_ID, typeof(long))
                {
                    AutoIncrement = true,
                    AutoIncrementSeed = 1
                };
                _table.Columns.Add(oid);
                _table.PrimaryKey = new DataColumn[] { oid };

                // We have spatial data (the X and Y coordinates of a point), therefore the data can be treated as a feature class (not as a table).
                _table.Columns.Add(new DataColumn(Fusion.m_DataColumn_X, typeof(double)));
                _table.Columns.Add(new DataColumn(Fusion.m_DataColumn_Y, typeof(double)));

                // Add a shape column to treat the data as a feature class.
                _table.Columns.Add(new DataColumn(Fusion.m_DataColumn_Geom, typeof(System.Byte[])));
                //do we have a Z?
                _hasZ = false;

                //For spatial data...
                //Domain to verify coordinates (2D)
                //default to the Spatial Reference domain
                RBush.Envelope sr_extent = new RBush.Envelope(
                  MinX: _sr.Domain.XMin,
                  MinY: _sr.Domain.YMin,
                  MaxX: _sr.Domain.XMax,
                  MaxY: _sr.Domain.YMax
                );

                //default to the Spatial Reference domain
                _extent = sr_extent;

                // Get data entries and load the datatable
                // int ind = 0;
                foreach (MapPoint mapPoint in listFeatures)
                {
                    //string x = Convert.ToString(mapPoint.X, CultureInfo.InvariantCulture);
                    //string y = Convert.ToString(mapPoint.Y, CultureInfo.InvariantCulture);

                    var row = _table.NewRow();
                    //row[1] = System.DBNull.Value;
                    //row[2] = System.DBNull.Value;
                    row[1] = mapPoint.X;
                    row[2] = mapPoint.Y;

                    // ensure the coordinate is within bounds
                    var coord = new ArcGIS.Core.Geometry.Coordinate3D(mapPoint.X, mapPoint.Y, mapPoint.Z);
                    if (!sr_extent.Contains2D(coord))
                        throw new GeodatabaseFeatureException(
                          "The feature falls outside the defined spatial reference!");

                    // store it
                    row[Fusion.m_DataColumn_Geom] = coord.ToMapPoint().ToEsriShape();

                    // add it to the index
                    var rbushCoord = new RBushCoord3D(coord, (long)row[Fusion.m_DataColumn_ID]);
                    _rtree.Insert(rbushCoord);

                    // update max and min for use in the extent
                    if (_rtree.Count == 1)
                    {
                        // first record
                        _extent = rbushCoord.Envelope;
                    }
                    else
                    {
                        _extent = rbushCoord.Envelope.Union2D(_extent);
                    }

                    _table.Rows.Add(row);

                    // ind++;
                }

                return true;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "GetTableDataAsync");
                return false;
            }
            finally
            {
                if (m_Helper_Working != null)
                {
                    await Fusion.m_Global.AppDispatcher.Invoke(async() =>
                    {
                        await m_Helper_Working.FinishWorkingAsync();
                        m_Helper_Working = null;
                    });
                }
            }
        }

        #endregion Open

    }
}
