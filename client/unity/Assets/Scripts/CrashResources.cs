using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class CrashResources
{
    private const string appStatePathPrefix = "AppState/"; 
    public static AppState[] LoadAppStates()
    {
        return Assembly.GetAssembly(typeof(AppState)).GetTypes()
            .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(AppState)))
            .Select(x => x.Name)
            .Select(typeName => Resources.Load<AppState>($"{appStatePathPrefix}{typeName}"))
            .ToArray();
    }
    
    private const string uiCanvasPrefabPath = "UI/Canvas"; 
    public static Canvas LoadUICanvasPrefab()
    {
        return Resources.Load<Canvas>(uiCanvasPrefabPath);
    }

    public static CrashOption LoadOption()
    {
        return Resources.Load<CrashOption>(nameof(CrashOption));
    }
}