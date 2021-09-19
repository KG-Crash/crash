using System.Collections;
using System.Collections.Generic;
using FixMath.NET;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Fix64))]
public class Fix64Editor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {        
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        Fix64TextField(position, property);
        
        EditorGUI.EndProperty();
    }

    public static Fix64 Fix64TextField(Rect remainRect, SerializedProperty property)
    {
        var valueProperty = property.FindPropertyRelative("m_rawValue");
        var value = valueProperty.longValue;
        
        EditorGUI.BeginChangeCheck();
        
        var fix64 = Fix64TextField(remainRect, Fix64.FromRaw(value));

        if (EditorGUI.EndChangeCheck())
        {
            valueProperty.longValue = fix64.RawValue;
        }

        return fix64;
    }
    
    public static Fix64 Fix64TextField(Rect remainRect, Fix64 fix64)
    {
        var str = EditorGUI.DelayedTextField(remainRect, fix64.ToString());
        if (Fix64.TryParse(str, out var new64))
        {
            return new64;
        }
        else
        {
            return fix64;
        }
    }
}

[CustomPropertyDrawer(typeof(FixVector3))]
public class FixVector3Editor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(FixVector2))]
public class FixVector2Editor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(FixBounds2))]
public class FixBounds2Editor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(FixBounds3))]
public class FixBounds3Editor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        EditorGUI.EndProperty();
    }
}
