using msGIS.ProApp_Common_MA31_GIS_3x;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
// using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Net.Http;

namespace msGIS.ProApp_FiwareSummit
{
    internal static class Settings_EntityTypes
    {
        private static readonly string m_ModuleName = "Settings_EntityTypes";

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

        internal static async Task<List<object>> ReadSettingsFromRestApiAsync(string apiUrl)
        {
            try
            {
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
                foreach (JToken jTokenVal in oJTokenVals)
                {
                    string arrVal = await ReadSettingStringAsync(settingsPath, cfgKey, jTokenVal);
                    listEntityTypes.Add(arrVal);
                }

                return listEntityTypes;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ReadSettingsFromJsonObjectAsync");
                return null;
            }
        }

        internal static async Task<object> GetJsonFromRestApiAsync(string apiUrl)
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
                // Handle any exceptions
                Console.WriteLine("Error retrieving JSON from REST API: " + ex.Message);
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "GetJsonFromRestApiAsync");
                return null;
            }
        }

        internal static async Task RefreshRestApiAsync()
        {
            try
            {
                string apiUrl = "https://fiwaredev.msgis.net/ngsi-proxy/eventsource/e9e01390-fae3-11ed-926f-1bdc1977e2d3";

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
    }

}
