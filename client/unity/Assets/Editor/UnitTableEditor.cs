using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitTable))]
public class UnitTableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var targetAs = target as UnitTable;
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Clear"))
            {
                targetAs.Clear();
            }
            if (GUILayout.Button("Clear null Unit"))
            {
                ClearNullUnit(targetAs);
            }   
        }
        EditorGUILayout.EndHorizontal();
    }

    public static void ClearNullUnit(UnitTable unitTable)
    {
        var keys = unitTable.GetEnumerable().Where(x => x.Value == null).Select(x => x.Key)
            .ToArray();
        foreach (var key in keys)
        {
            unitTable.RemoveKey(key);
        }
    }
}