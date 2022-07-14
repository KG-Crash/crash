using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class CrashResources
{
    private static readonly string _appStatePathPrefix = "AppState/"; 
    public static AppState[] LoadAppStatesFromResources()
    {
        return Assembly.GetAssembly(typeof(AppState)).GetTypes()
            .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(AppState)))
            .Select(x => x.Name)
            .Select(typeName => Resources.Load<AppState>($"{_appStatePathPrefix}{typeName}"))
            .ToArray();
    }
}