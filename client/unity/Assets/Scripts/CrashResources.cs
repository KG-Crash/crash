using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class CrashResources
{
    private const string appStatePathPrefix = "AppState/"; 
    private const string uiCanvasPrefabPath = "UI/Canvas";
    private static Mesh quad = null;
    
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
        return Resources.Load<T>($"Tables/{typeof(T).Name}");
    }

    public static Mesh Quad()
    {
        if (quad == null)
        {
            quad = new Mesh
            {
                vertices = new[] {new Vector3(0, 0), new Vector3(1, 0), new Vector3(1, 1), new Vector3(0, 1)}
            };
            quad.SetIndices(new[] { 0, 1, 2, 1, 2, 3 }, MeshTopology.Triangles, 0);
            quad.UploadMeshData(true);
        }
        
        return quad;
    }
}