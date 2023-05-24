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

namespace msGIS.ProApp_FiwareSummit
{
    internal static class Settings_EntityTypes
    {
        private static readonly string m_ModuleName = "Settings_EntityTypes";

        private const string m_Key_MainSettings = "msGIS_Settings_EntityTypes";

        // Main Settings
        private const string m_CfgKey_MapTag = "MapTag";

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

        internal static async Task AcquireSettingsEntityTypesAsync()
        {
            try
            {
                string settingsPath = SettingsPath;
                if (!File.Exists(settingsPath))
                    await WriteSettingsAsync(settingsPath);
                else
                    await ReadSettingsAsync(settingsPath);
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "AcquireSettingsEntityTypesAsync");
            }
        }

        private static async Task WriteSettingsAsync(string settingsPath)
        {
            try
            {
                JObject jObjectVals = new JObject
                {
                    { m_CfgKey_MapTag, JValue.CreateString(Fusion.m_MapTag) },

                };
                JObject jObjectSettings = new JObject
                {
                    { m_Key_MainSettings, jObjectVals }
                };

                string jsonStrSettings = jObjectSettings.ToString(Formatting.Indented);
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
                    string msg = $"Key \"{m_Key_MainSettings}.{cfgKey}\" must be of type \"{JTokenType.String}\"!";
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
                    string msg = $"Key \"{m_Key_MainSettings}.{cfgKey}\" must be of type \"{JTokenType.Integer}\"!";
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

        private static async Task ReadSettingsAsync(string settingsPath)
        {
            try
            {
                string jsonStrSettings = File.ReadAllText(settingsPath);
                string msg = "";

                JObject oJObjectSettings = JObject.Parse(jsonStrSettings);
                if ((oJObjectSettings == null) || (oJObjectSettings.Count == 0))
                {
                    await Fusion.m_Messages.AlertAsyncMsg("Empty settings!", settingsPath, "Read Settings");
                    return;
                }
                if (!oJObjectSettings.ContainsKey(m_Key_MainSettings))
                {
                    msg = $"Key <{m_Key_MainSettings}> not found!";
                    await Fusion.m_Messages.AlertAsyncMsg(msg, settingsPath, "Read Settings");
                    return;
                }
                Boolean result = oJObjectSettings.TryGetValue(m_Key_MainSettings, out JToken oJTokenVals);
                if (!result)
                {
                    msg = $"Key <{m_Key_MainSettings}> could not be obtained!";
                    await Fusion.m_Messages.AlertAsyncMsg(msg, settingsPath, "Read Settings");
                    return;
                }

                JObject oJObjectVals = oJTokenVals.ToObject<JObject>();
                foreach (KeyValuePair<string, JToken> KeyValuePairData in oJObjectVals)
                {
                    string cfgKey = KeyValuePairData.Key;
                    JToken cfgVal = KeyValuePairData.Value;
                    if (cfgVal == null)
                        continue;

                    switch (cfgKey)
                    {
                        case m_CfgKey_MapTag:
                            Fusion.m_MapTag = await ReadSettingStringAsync(settingsPath, cfgKey, cfgVal);
                            break;

                        default:
                            msg = $"Key \"{m_Key_MainSettings}.{cfgKey}\" is not admissive!";
                            await Fusion.m_Messages.AlertAsyncMsg(msg, settingsPath, "Read Settings");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                await Fusion.m_Messages.PushAsyncEx(ex, m_ModuleName, "ReadSettingsAsync");
            }
        }

    }
}
