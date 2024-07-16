using System.Collections.Generic;
using FrameWork;

public class EditorConfig
{
    public const string c_directoryName = "Config";
    public const string c_expandName = "json";
    
    #region FindConfig
    public static string GetConfigPath(string configName)
    {
        return PathTool.GetAbsolutePath(ResLoadLocation.Resource,
            PathTool.GetRelativelyPath(c_directoryName,
                configName,
                c_expandName));
    }

    #endregion

    #region 保存配置

    public static void SaveData(string ConfigName, Dictionary<string, SingleField> data)
    {
        EditorUtil.WriteStringByFile(PathTool.GetAbsolutePath(ResLoadLocation.Resource,
                PathTool.GetRelativelyPath(c_directoryName,
                    ConfigName,
                    c_expandName)),
            JsonTool.Dictionary2Json<SingleField>(data));

        UnityEditor.AssetDatabase.Refresh();
    }

    public static Dictionary<string, object> GetEditorConfigData(string ConfigName)
    {
        string dataJson = ResourceIOTool.ReadStringByFile(PathTool.GetEditorPath(c_directoryName,
            ConfigName, c_expandName));

        if (dataJson == "")
        {
            return null;
        }
        else
        {
            return Json.Deserialize(dataJson) as Dictionary<string, object>;
        }
    }

    public static void SaveEditorConfigData(string ConfigName, Dictionary<string, object> data)
    {
        string configDataJson = Json.Serialize(data);

        EditorUtil.WriteStringByFile(
            PathTool.GetEditorPath(c_directoryName, ConfigName, c_expandName),
            configDataJson);

        UnityEditor.AssetDatabase.Refresh();
    }

    #endregion
}