﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Framework;

/// <summary>
/// 配置管理器，只读
/// </summary>
public static class ConfigManager
{
    static Dictionary<Type, IConfig> mConfigs = new Dictionary<Type, IConfig>();

    private static HashSet<Type> DONT_DESTROY_CONFIGS = new HashSet<Type>
    {
    };


    public const string c_directoryName = "Config";
    public const string c_expandName = "json";

    /// <summary>
    /// 配置缓存
    /// </summary>
    static Dictionary<string, Dictionary<string, SingleField>> s_configCache =
        new Dictionary<string, Dictionary<string, SingleField>>();

    public static bool IsConfigExist(string ConfigName)
    {
        return ResourcesConfigManager.GetIsExitRes(ConfigName);
    }


    public static T Get<T>() where T : ScriptableObject, IConfig, new()
    {
        var type = typeof(T);
        IConfig result = null;

        if (mConfigs.TryGetValue(type, out result))
        {
            return (T)result;
        }

        result = Load<T>();
        return (T)result;
    }

    public static T Load<T>() where T : ScriptableObject, IConfig, new()
    {
        var type = typeof(T);
        IConfig result = null;
        if (mConfigs.TryGetValue(type, out result))
        {
            return (T)result;
        }

        T obj = null;
        obj = ScriptableObject.CreateInstance<T>();
        var path = obj.Path;
        if (!IsConfigExist(path))
            return (T)null;

        obj = ResourceManager.Load<T>(path);
        if (obj)
        {
            mConfigs.Add(obj.GetType(), obj);
        }

        return obj;
    }

    public static void UnLoad<T>()
    {
        IConfig result = null;
        var type = typeof(T);
        if (mConfigs.TryGetValue(type, out result))
        {
            mConfigs.Remove(type);
            ResourceManager.DestroyAssetsCounter(result.Path);
        }
    }

    private static void ClearAllConfig()
    {
        foreach (var kvp in mConfigs)
        {
            ResourceManager.DestroyAssetsCounter(kvp.Value.Path);
        }

        mConfigs.Clear();
    }

    public static Dictionary<string, SingleField> GetData(string ConfigName)
    {
        if (s_configCache.ContainsKey(ConfigName))
        {
            return s_configCache[ConfigName];
        }

        string dataJson = "";

        //#if UNITY_EDITOR
        //if (!Application.isPlaying)
        //{
        //    dataJson = ResourceIOTool.ReadStringByResource(
        //        PathTool.GetRelativelyPath(c_directoryName,
        //                    ConfigName,
        //                    c_expandName));
        //}

        ////#else
        //else
        //{
        //    dataJson = ResourceManager.LoadText(ConfigName);
        //}
        //#endif
        dataJson = ResourceManager.LoadText(ConfigName);

        if (dataJson == "")
        {
            throw new Exception("ConfigManager GetData not find " + ConfigName);
        }
        else
        {
            Dictionary<string, SingleField> config = JsonTool.Json2Dictionary<SingleField>(dataJson);

            s_configCache.Add(ConfigName, config);
            return config;
        }
    }

    public static SingleField GetData(string ConfigName, string key)
    {
        return GetData(ConfigName)[key];
    }

    public static void CleanCache()
    {
        foreach (var item in s_configCache.Keys)
        {
            ResourceManager.DestroyAssetsCounter(item);
        }

        s_configCache.Clear();

        ClearAllConfig();
    }
}