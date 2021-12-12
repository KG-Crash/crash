using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// Create CrashSettingsProvider by deriving from SettingsProvider:
public class CrashSettingsProvider : SettingsProvider
{
    private static string ToVariableDisplayName(string crashVariableName)
    {
        return ObjectNames.NicifyVariableName(crashVariableName.Replace("_", ""));
    }
    
    private SerializedObject _customSettings;

    public readonly GUIContent _mapDrawCellsLabel =
        new GUIContent(ToVariableDisplayName(nameof(CrashSettings._mapDrawCells)));
    public readonly GUIContent _mapDrawEdgesLabel =
        new GUIContent(ToVariableDisplayName(nameof(CrashSettings._mapDrawEdges)));
    public readonly GUIContent _mapDrawRegionEdgesLabel =
        new GUIContent(ToVariableDisplayName(nameof(CrashSettings._mapDrawRegionEdges)));

    public CrashSettingsProvider(string path) : base(path, SettingsScope.Project)
    {        
    }

    public static bool IsSettingsAvailable()
    {
        return File.Exists(CrashSettings._crashSettingsPath);
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        // This function is called when the user clicks on the MyCustom element in the Settings window.
        _customSettings = CrashSettings.GetSerializedSettings();

    }

    public override void OnGUI(string searchContext)
    {
        _customSettings.Update();
        
        var mapDrawCellsProperty = _customSettings.FindProperty(nameof(CrashSettings._mapDrawCells));
        var mapDrawEdgesProperty = _customSettings.FindProperty(nameof(CrashSettings._mapDrawEdges));
        var mapDrawRegionEdgesProperty = _customSettings.FindProperty(nameof(CrashSettings._mapDrawRegionEdges));

        EditorGUILayout.PropertyField(mapDrawCellsProperty, _mapDrawCellsLabel);
        EditorGUILayout.PropertyField(mapDrawEdgesProperty, _mapDrawEdgesLabel);
        EditorGUILayout.PropertyField(mapDrawRegionEdgesProperty, _mapDrawRegionEdgesLabel);

        _customSettings.ApplyModifiedProperties();
    }

    // Register the SettingsProvider
    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        if (IsSettingsAvailable())
        {
            return new CrashSettingsProvider("Project/CrashSettingsProvider")
            {
                label = "Crash Settings"
            };
        }

        // Settings Asset doesn't exist yet; no need to display anything in the Settings window.
        return null;
    }
}
