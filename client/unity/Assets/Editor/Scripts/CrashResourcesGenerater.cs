using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CrashResourcesGenerater
{
    private const string appStatePath = "Assets/Resources/AppState/"; 
    
    [MenuItem("Crash/Generate All AppState")]
    private static void GenerateAppStates()
    {
        var appStateTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsSubclassOf(typeof(AppState)) && !x.IsAbstract)
            .ToArray();

        foreach (var type in appStateTypes)
        {
            var path = $"{appStatePath}{type.Name}.asset";
            var appStateObj = AssetDatabase.LoadAssetAtPath<AppState>(path);

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