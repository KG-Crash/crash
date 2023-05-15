using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CrashResourcesGenerater
{
    private const string appStatePath = "Assets/Resources/AppState/"; 
    
    [MenuItem("Crash/Generate All AppStateSettings")]
    private static void GenerateAppStates()
    {
        var appStateSettingTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsSubclassOf(typeof(AppStateSettings)) && !x.IsAbstract)
            .ToArray();

        foreach (var type in appStateSettingTypes)
        {
            var path = $"{appStatePath}{type.Name}.asset".Replace("Settings", "");
            var appStateObj = AssetDatabase.LoadAssetAtPath<AppStateSettings>(path);

            if (appStateObj != null) continue;
            
            var obj = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(obj, path);
            AssetDatabase.ImportAsset(path);
        }
    }
    
    private const string optionPath = "Assets/Resources/CrashAppSettings.asset";

    [MenuItem("Crash/Generate CrashAppSettings")]
    private static void GenerateOption()
    {
        if (AssetDatabase.LoadAssetAtPath<CrashAppSettings>(optionPath) == null)
        {
            var obj = ScriptableObject.CreateInstance<CrashAppSettings>();
            AssetDatabase.CreateAsset(obj, optionPath);
            AssetDatabase.ImportAsset(optionPath);
        }
    }
}