using msGIS.ProApp_Common_FIWARE_3x;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace msGIS.ProApp_FiwareSummit
{
    internal static class RestApi_Entities
    {
        private static readonly string m_ModuleName = "RestApi_Entities";

        private static async Task<string> ReadSettingStringAsync(string settingsPath, string cfgKey, JToken cfgVal)
        {
            try
            {
                if (cfgVal.Type != JTokenType.String)
                {
                    string msg = $"Key \"{cfgKey}\" must be of type \"{JTokenType.String}\"!";
                    await Fusion.m_Messages.AlertAsyncMsg(msg, settingsPath, "Read Setting");
                    return "";
                }
                else
                {
                    return cfgVal.ToString();
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ReadSettingStringAsync");
                return "";
            }
        }

        private static async Task<int> ReadSettingIntegerAsync(string settingsPath, string cfgKey, JToken cfgVal)
        {
            try
            {
                if (cfgVal.Type != JTokenType.Integer)
                {
                    string msg = $"Key \"{cfgKey}\" must be of type \"{JTokenType.Integer}\"!";
                    await Fusion.m_Messages.AlertAsyncMsg(msg, settingsPath, "Read Setting");
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(cfgVal);
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ReadSettingIntegerAsync");
                return 0;
            }
        }

        private static async Task<List<object>> ReadSettingsFromJsonFileAsync(string settingsPath)
        {
            try
            {
                string jsonStrSettings = File.ReadAllText(settingsPath);
                JObject oJObjectSettings = JObject.Parse(jsonStrSettings);
                if ((oJObjectSettings == null) || (oJObjectSettings.Count == 0))
                {
                    await Fusion.m_Messages.AlertAsyncMsg("Empty settings!", settingsPath, "Read Settings");
                    return null;
                }

                return await ReadSettingsFromJsonObjectAsync(oJObjectSettings, settingsPath);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ReadSettingsFromJsonFileAsync");
                return null;
            }
        }

        internal static async Task<List<object>> ReadSettingsFromRestApiAsync(string datasourcePath)
        {
            try
            {
                string apiUrl = $"{datasourcePath}/ngsi-ld/v1/types";

                JObject oJObjectSettings = (JObject)await GetJsonFromRestApiAsync(apiUrl);
                if ((oJObjectSettings == null) || (oJObjectSettings.Count == 0))
                {
                    await Fusion.m_Messages.AlertAsyncMsg("Empty settings!", apiUrl, "Read Settings");
                    return null;
                }

                return await ReadSettingsFromJsonObjectAsync(oJObjectSettings, apiUrl);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ReadSettingsFromRestApiAsync");
                return null;
            }
        }

        private static async Task<List<object>> ReadSettingsFromJsonObjectAsync(JObject oJObjectSettings, string settingsPath)
        {
            try
            {
                string msg = "";

                string cfgKey = "type";
                if (!oJObjectSettings.ContainsKey(cfgKey))
                {
                    msg = $"Key <{cfgKey}> not found!";
                    await Fusion.m_Messages.AlertAsyncMsg(msg, settingsPath, "Read Settings");
                    return null;
                }
                Boolean result = oJObjectSettings.TryGetValue(cfgKey, out JToken oJTokenVal);
                if (!result)
                {
                    msg = $"Key <{cfgKey}> could not be obtained!";
                    await Fusion.m_Messages.AlertAsyncMsg(msg, settingsPath, "Read Settings");
                    return null;
                }
                string cfgVal = await ReadSettingStringAsync(settingsPath, cfgKey, oJTokenVal);
                string expected = "EntityTypeList";
                if (cfgVal != expected)
                {
                    msg = $"Key <{cfgKey}> has unexpected value {cfgVal} <> {expected}!";
                    await Fusion.m_Messages.AlertAsyncMsg(msg, settingsPath, "Read Settings");
                    return null;
                }

                cfgKey = "typeList";
                if (!oJObjectSettings.ContainsKey(cfgKey))
                {
                    msg = $"Key <{cfgKey}> not found!";
                    await Fusion.m_Messages.AlertAsyncMsg(msg, settingsPath, "Read Settings");
                    return null;
                }
                result = oJObjectSettings.TryGetValue(cfgKey, out JToken oJTokenVals);
                if (!result)
                {
                    msg = $"Key <{cfgKey}> could not be obtained!";
                    await Fusion.m_Messages.AlertAsyncMsg(msg, settingsPath, "Read Settings");
                    return null;
                }

                // Populate list wih entity types.
                List<object> listEntityTypes = new List<object>();
                if (oJTokenVals != null)
                {
                    foreach (JToken jTokenVal in oJTokenVals)
                    {
                        string arrVal = await ReadSettingStringAsync(settingsPath, cfgKey, jTokenVal);
                        listEntityTypes.Add(arrVal);
                    }
                }

                return listEntityTypes;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ReadSettingsFromJsonObjectAsync");
                return null;
            }
        }

        private static async Task<object> GetJsonFromRestApiAsync(string apiUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Make a GET request to the API endpoint
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    // Ensure the response is successful
                    response.EnsureSuccessStatusCode();

                    // Read the response content as a string
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON string to an object
                    object jsonObject = JsonConvert.DeserializeObject(responseContent);

                    // Return the deserialized object
                    return jsonObject;
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine("Error retrieving JSON from REST API: " + ex.Message);
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "GetJsonFromRestApiAsync");
                return null;
            }
        }

        internal static async Task<JArray> GetEntitiesFromRestApiAsync(string datasourcePath, string entityType)
        {
            try
            {
                if (string.IsNullOrEmpty(entityType))
                    throw new Exception("Empty entity type!");

                JArray jArrayEntities = new JArray();

                int limit = 60;                     // Limit max 2000
                int offset = 0;                     // Start from 0
                bool hasEntities = false;
                do
                {
                    // Read as long entities comming.
                    string apiUrl = $"{datasourcePath}/ngsi-ld/v1/entities?type={entityType}&offset={offset}&limit={limit}";
                    JArray jArrayPart = await GetJsonFromRestApiAsync(apiUrl) as JArray;
                    if (jArrayPart == null)
                        hasEntities = false;
                    else
                    {
                        hasEntities = (jArrayPart.Count > 0);
                        offset += jArrayPart.Count;

                        if (hasEntities)
                        {
                            foreach (JToken element in jArrayPart)
                            {
                                jArrayEntities.Add(element);
                            }
                            // await Fusion.m_UserControl_EntityTypes.ShowCountAsync(jArrayEntities.Count);
                        }
                    }
                } while (hasEntities);

                return jArrayEntities;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "GetEntitiesFromRestApiAsync");
                return null;
            }
        }

        internal static async Task<bool> BuildFeaturesFromJsonEntitiesAsync(Layer layerEntitiesPoints, EditOperation editOperation, JArray jArrayEntities)
        {
            try
            {
                // Create a new feature class
                bool result = await QueuedTask.Run(async () =>
                {
                    // Iterate over the JSON features
                    // ld:Hydrant:HYDRANTOGD.36612504","type":"Hydrant","OBJECTID":{"type":"Property","value":36612504},"location":{"type":"GeoProperty","value":{"type":"Point","coordinates":[16.292543,48.191422,199]}}},{"id":"urn:ngsi-
                    foreach (JToken jToken_Entity in jArrayEntities)
                    {
                        string msg;
                        if ((jToken_Entity != null) && (jToken_Entity.Type == JTokenType.Object))
                        {
                            // Dictionary<string, object> test = jToken_Entity["location"].ToObject<Dictionary<string, object>>();
                            JObject keyValuePairs = jToken_Entity.ToObject<JObject>();

                            string cfgKey = "location";
                            if (keyValuePairs == null)
                                continue;
                            if (!keyValuePairs.ContainsKey(cfgKey))
                            {
                                msg = $"Key <{cfgKey}> not found!";
                                await Fusion.m_Messages.AlertAsyncMsg(msg, "Read Entity");
                                return false;
                            }
                            bool result = keyValuePairs.TryGetValue(cfgKey, out JToken jToken_Location);
                            if (!result)
                            {
                                msg = $"Key <{cfgKey}> could not be obtained!";
                                await Fusion.m_Messages.AlertAsyncMsg(msg, "Read Entity");
                                return false;
                            }

                            if (jToken_Location == null)
                                continue;
                            keyValuePairs = jToken_Location.ToObject<JObject>();

                            cfgKey = "value";
                            if (keyValuePairs == null)
                                continue;
                            if (!keyValuePairs.ContainsKey(cfgKey))
                            {
                                msg = $"Key <{cfgKey}> not found!";
                                await Fusion.m_Messages.AlertAsyncMsg(msg, "Read Entity");
                                return false;
                            }
                            result = keyValuePairs.TryGetValue(cfgKey, out JToken jToken_Value);
                            if (!result)
                            {
                                msg = $"Key <{cfgKey}> could not be obtained!";
                                await Fusion.m_Messages.AlertAsyncMsg(msg, "Read Entity");
                                return false;
                            }

                            if (jToken_Value == null)
                                continue;
                            keyValuePairs = jToken_Value.ToObject<JObject>();

                            cfgKey = "type";
                            if (keyValuePairs == null)
                                continue;
                            if (!keyValuePairs.ContainsKey(cfgKey))
                            {
                                msg = $"Key <{cfgKey}> not found!";
                                await Fusion.m_Messages.AlertAsyncMsg(msg, "Read Entity");
                                return false;
                            }
                            result = keyValuePairs.TryGetValue(cfgKey, out JToken jToken_Type);
                            if (!result)
                            {
                                msg = $"Key <{cfgKey}> could not be obtained!";
                                await Fusion.m_Messages.AlertAsyncMsg(msg, "Read Entity");
                                return false;
                            }

                            if (jToken_Type == null)
                                continue;
                            if (jToken_Type.ToString() != "Point")
                            {
                                msg = $"Key <{cfgKey}> type not expected <> Point!";
                                await Fusion.m_Messages.AlertAsyncMsg(msg, "Read Entity");
                                return false;
                            }

                            cfgKey = "coordinates";
                            if (!keyValuePairs.ContainsKey(cfgKey))
                            {
                                msg = $"Key <{cfgKey}> not found!";
                                await Fusion.m_Messages.AlertAsyncMsg(msg, "Read Entity");
                                return false;
                            }
                            result = keyValuePairs.TryGetValue(cfgKey, out JToken jToken_Coordinates);
                            if (!result)
                            {
                                msg = $"Key <{cfgKey}> could not be obtained!";
                                await Fusion.m_Messages.AlertAsyncMsg(msg, "Read Entity");
                                return false;
                            }

                            //string jStringPoint = jToken_Coordinates.ToString();
                            //MapPoint mapPointFromJson = MapPointBuilderEx.FromJson(jStringPoint);
                            if (jToken_Coordinates == null)
                                continue;
                            if (jToken_Coordinates.Type != JTokenType.Array)
                            {
                                msg = $"Key <{cfgKey}> type not expected <> Array!";
                                await Fusion.m_Messages.AlertAsyncMsg(msg, "Read Entity");
                                return false;
                            }
                            JArray jArray_Coordinates = (JArray)jToken_Coordinates;

                            double x = Convert.ToDouble(jArray_Coordinates[0]);
                            double y = Convert.ToDouble(jArray_Coordinates[1]);
                            MapPoint mapPoint = MapPointBuilderEx.CreateMapPoint(x, y);

                            editOperation.Create(layerEntitiesPoints, mapPoint);
                        }

                    }

                    return true;
                });

                return true;
            }
            catch (Exception ex)
            {
                // Console.WriteLine("Error converting JSON to features: " + ex.Message);
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "BuildFeaturesFromJsonEntitiesAsync");
                return false;
            }
        }

        private static async Task RefreshRestApiAsync(string datasourcePath)
        {
            try
            {
                string apiUrl = $"{datasourcePath}/ngsi-proxy/eventsource/e9e01390-fae3-11ed-926f-1bdc1977e2d3";

                using (HttpClient client = new HttpClient())
                {
                    using (var stream = await client.GetStreamAsync(apiUrl))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            while (true)
                            {
                                Console.WriteLine(reader.ReadLine());
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "RefreshRestApiAsync");
            }
        }

        private static bool m_IsUpdateOnStage = false;
        internal static async Task StopUpdateAsync()
        {
            try
            {
                m_IsUpdateOnStage = false;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "StopUpdateAsync");
            }
        }
        internal static async Task StartUpdateAsync(string entityType)
        {
            try
            {
                switch (entityType)
                {
                    case "":
                        break;

                    default:
                        break;
                }

                // DateTime startTime = DateTime.Now;
                m_IsUpdateOnStage = false;
                while (m_IsUpdateOnStage)
                {
                    await Task.Delay(800);
                    // long timeoutSec = (DateTime.Now.Ticks - startTime.Ticks) / TimeSpan.TicksPerSecond;
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "StartUpdateAsync");
            }
        }

    }

}
