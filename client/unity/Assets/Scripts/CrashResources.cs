using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class CrashResources
{
    private const string appStatePathPrefix = "AppState/"; 
    private const string uiCanvasPrefabPath = "UI/Canvas"; 
    
    public static AppState[] LoadAppStates()
    {
        var type = typeof(AppState);
        return Assembly.GetAssembly(type).GetTypes()
            .Where(t => !t.IsAbstract && t.IsSubclassOf(type))
            .Select(x => x.Name)
            .Select(typeName => Resources.Load<AppState>($"{appStatePathPrefix}{typeName}"))
            .ToArray();
    }
    
    public static Canvas LoadUICanvasPrefab()
    {
        return Resources.Load<Canvas>(uiCanvasPrefabPath);
    }

    public static CrashAppSettings LoadAppSettings()
    {
        return Resources.Load<CrashAppSettings>(nameof(CrashAppSettings));
    }

    public static UnityTable<T> LoadUnityTable<T>() where T : UnityTable<T>
    {
        return Resources.Load<T>($"Tables/{nameof(T)}");
    }
}