using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Shared;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UpgradeIconTable))]
public class UpgradeIconTableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Refresh Ability-Icon"))
        {
            var table = target as UpgradeIconTable;
            table.Refresh();
        }
    }
}
