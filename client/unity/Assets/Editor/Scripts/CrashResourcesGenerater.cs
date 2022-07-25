using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CrashResourcesGenerater
{
    private const string appStatePath = "Assets/Resources/AppState/"; 
    
    [MenuItem("Crash/Generate All AppState")]
    [InitializeOnLoadMethod]
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

    private const string optionPath = "Assets/Resources/CrashOption.asset";

    [MenuItem("Crash/Generate Option")]
    [InitializeOnLoadMethod]
    private static void GenerateOption()
    {
        var obj = ScriptableObject.CreateInstance<CrashOption>();
        AssetDatabase.CreateAsset(obj, optionPath);
        AssetDatabase.ImportAsset(optionPath);
    }
}