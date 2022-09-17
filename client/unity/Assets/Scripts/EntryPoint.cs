using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class EntryPoint
{
    public static AppStateService appStateService { get; private set; }
    public static KG.UIPool uiPool { get; private set; }
    public static string uiBundleName => CrashResources.LoadAppSettings().uiBundleName;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AfterSceneLoad()
    {
        var option = CrashResources.LoadAppSettings();
        uiPool = new KG.UIPool(CrashResources.LoadUICanvasPrefab(), option.uiBundleName);
        appStateService = new AppStateService(CrashResources.LoadAppStates(), uiPool);
        
        if (option.moveEntrySceneWhenStart)
            _ = appStateService.LoadEntrySceneAsync();

        LoadUnityTables();
    }

    #region Unity Tables, 따로 빼야함
    
    private static Dictionary<Type, UnityTable> unityTables { get; set; }

    private static void LoadUnityTables()
    {
        unityTables = 
            CrashResources.LoadUnityTables()
            .ToDictionary(table => table.GetType(), table => table);
    }

    public static T GetTable<T>() where T : UnityTable
    {
        var type = typeof(T);
        if (unityTables.ContainsKey(type))
            return unityTables[type] as T;
        else
            return null;
    }

    public static bool TryGetTable<T>(out T value) where T : UnityTable
    {
        var type = typeof(T);
        if (unityTables.ContainsKey(type))
        {
            value = unityTables[type] as T;
            return true;
        }
        else
        {
            value = null;
            return false;
        }
    }
    
    #endregion
}