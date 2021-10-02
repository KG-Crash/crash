using System.Collections;
using System.Collections.Generic;
using FixMath.NET;
using UnityEditor;
using UnityEngine;

public static class Fix64EditorUtil
{
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

    public static void FixVectorField(Rect remainRect, SerializedProperty property, int itemCount, float spacing, float labelWidth)
    {        
        var itemWidth = (remainRect.size.x - spacing * (itemCount - 1)) / itemCount;

        var childProperty = property.FindPropertyRelative("x");
        remainRect.size = new Vector2(labelWidth, remainRect.size.y);
        EditorGUI.LabelField(remainRect, "X");
        remainRect.position = remainRect.position + new Vector2(labelWidth, 0);
        remainRect.size = new Vector2(itemWidth - labelWidth, remainRect.size.y);
        Fix64TextField(remainRect, childProperty);

        remainRect.position = remainRect.position + new Vector2(itemWidth - labelWidth + spacing, 0);
        childProperty = property.FindPropertyRelative("y");
        remainRect.size = new Vector2(labelWidth, remainRect.size.y);
        EditorGUI.LabelField(remainRect, "Y");
        remainRect.position = remainRect.position + new Vector2(labelWidth, 0);
        remainRect.size = new Vector2(itemWidth - labelWidth, remainRect.size.y);
        Fix64TextField(remainRect, childProperty);

        remainRect.position = remainRect.position + new Vector2(itemWidth - labelWidth + spacing, 0);
        childProperty = property.FindPropertyRelative("z");
        remainRect.size = new Vector2(labelWidth, remainRect.size.y);
        EditorGUI.LabelField(remainRect, "Z");
        remainRect.position = remainRect.position + new Vector2(labelWidth, 0);
        remainRect.size = new Vector2(itemWidth - labelWidth, remainRect.size.y);
        Fix64TextField(remainRect, childProperty);
    }
}

[CustomPropertyDrawer(typeof(Fix64))]
public class Fix64Editor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        Fix64EditorUtil.Fix64TextField(position, property);
        
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
        
        var remainRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);
        Fix64EditorUtil.FixVectorField(remainRect, property, 2, 4.0f, 12.0f);

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(FixVector3))]
public class FixVector3Editor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var remainRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);
        Fix64EditorUtil.FixVectorField(remainRect, property, 3, 4.0f, 12.0f);
        
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
