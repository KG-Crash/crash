using UnityEditor;
using UnityEngine;

public class CrashSettings : ScriptableObject
{
    public const string _crashSettingsPath = "Assets/Editor/Objects/CrashSettings.asset";

    [SerializeField]
    public bool _mapDrawCells;

    [SerializeField]
    public bool _mapDrawEdges;
    
    [SerializeField]
    public bool _mapDrawRegionEdges;

    private static CrashSettings _cashed = null;
    
    public static CrashSettings GetOrCreateSettings()
    {
        _cashed = AssetDatabase.LoadAssetAtPath<CrashSettings>(_crashSettingsPath);
        if (_cashed == null)
        {
            var settings = CreateInstance<CrashSettings>();
            settings._mapDrawCells = false;
            settings._mapDrawEdges = false;
            settings._mapDrawRegionEdges = false;
            AssetDatabase.CreateAsset(settings, _crashSettingsPath);
            AssetDatabase.SaveAssets();

            _cashed = settings;
        }
        return _cashed;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }
}