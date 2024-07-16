using UnityEngine;
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
    
    public static bool IsConfigExist(string ConfigName)
    {
        return ResourcesConfigManager.GetIsExitRes(ConfigName);
    }
    
    public static T Get<T>() where T : ScriptableObject, IConfig, new()
    {
        var type = typeof(T);

        if (mConfigs.TryGetValue(type, out var result))
        {
            return (T)result;
        }

        result = Load<T>();
        return (T)result;
    }

    public static T Load<T>() where T : ScriptableObject, IConfig, new()
    {
        var type = typeof(T);
        if (mConfigs.TryGetValue(type, out var result))
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
    
    public static void CleanCache()
    {
        foreach (var kvp in mConfigs)
        {
            ResourceManager.DestroyAssetsCounter(kvp.Value.Path);
        }

        mConfigs.Clear();
    }
}