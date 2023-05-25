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

namespace msGIS.ProApp_FiwareSummit
{
    internal static class Settings_EntityTypes
    {
        private static readonly string m_ModuleName = "Settings_EntityTypes";

        internal static string SettingsPath
        {
            get
            {
                try
                {
                    string dataPath = ArcGIS.Desktop.Core.Project.Current.HomeFolderPath;
                    return Path.Combine(dataPath, Fusion.m_FileNameSettings);
                }
                catch (Exception ex)
                {
                    Fusion.m_Messages.PushEx(ex, m_ModuleName, "SettingsPath");
                    return "";
                }
            }
        }

        internal static async Task<List<object>> AcquireSettingsEntityTypesAsync()
        {
            try
            {
                string settingsPath = SettingsPath;
                bool writeSettingsOnEmpty = false;
                List<object> listEntityTypes = new List<object>();

                if (!File.Exists(settingsPath))
                {
                    if (writeSettingsOnEmpty)
                        await WriteSettingsAsync(settingsPath);
                    else
                        await Fusion.m_Messages.AlertAsyncMsg("JSON file doesn't exist!", settingsPath, "AcquireSettingsEntityTypesAsync");
                }
                else
                {
                    listEntityTypes = await ReadSettingsAsync(settingsPath);
                }

                return listEntityTypes;
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "AcquireSettingsEntityTypesAsync");
                return null;
            }
        }

        private static async Task WriteSettingsAsync(string settingsPath)
        {
            try
            {
                JObject jObjectVals = new JObject
                {
                    { "type", JValue.CreateString("EntityTypeList") },
                    { "typeList", JValue.CreateString("...") },
                };

                string jsonStrSettings = jObjectVals.ToString(Formatting.Indented);
                File.WriteAllText(settingsPath, jsonStrSettings);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "WriteSettingsAsync");
            }
        }

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

        private static async Task<List<object>> ReadSettingsAsync(string settingsPath)
        {
            try
            {
                string jsonStrSettings = File.ReadAllText(settingsPath);
                string msg = "";

                JObject oJObjectSettings = JObject.Parse(jsonStrSettings);
                if ((oJObjectSettings == null) || (oJObjectSettings.Count == 0))
                {
                    await Fusion.m_Messages.AlertAsyncMsg("Empty settings!", settingsPath, "Read Settings");
                    return null;
                }

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
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ReadSettingsAsync");
                return null;
            }
        }

    }
}
