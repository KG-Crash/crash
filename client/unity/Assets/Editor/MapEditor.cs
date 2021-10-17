using KG;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KG.Map))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var map = (KG.Map)target;

        if (map._walkability.Length != map.cols * map.rows)
        {
            EditorGUILayout.HelpBox($"워커빌리티와({map._walkability.Length}) cols*rows({map.cols * map.rows}) 숫자가 안맞음. Update Walkabilty 해줘잉", MessageType.Warning);
        }

        if (GUILayout.Button("워커빌리티 업데이트"))
        {
            map.OnUpdateWalkability(); 
        }
    }
}