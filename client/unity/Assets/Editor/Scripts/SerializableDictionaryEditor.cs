using System;
using System.Collections.Generic;
using KG;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
public class UnityDictionaryDrawer : PropertyDrawer
{
    private Dictionary<string, ReorderableList> _reorderableLists = new Dictionary<string, ReorderableList>();

    private static DictKeyValue GetTypeOfObject(string obj)
    {
        switch (obj)
        {
            case "Int":
                return DictKeyValue.Integer;
            case "Enum":
                return DictKeyValue.Enum;
            case "Long":
                return DictKeyValue.Long;
            case "Float":
                return DictKeyValue.Float;
            case "Double":
                return DictKeyValue.Double;
            case "String":
                return DictKeyValue.String;
            default:
                return DictKeyValue.Undef;
        }
    }
    
    private static void AddKeyEmptyValuePair(ReorderableList list)
    {
        var property = list.serializedProperty;
        var index = property.arraySize;
        property.arraySize++;
        var element = property.GetArrayElementAtIndex(index);
        var elementKey = element.FindPropertyRelative("_key");

        var targetKey = elementKey.type;
        var keyType = GetTypeOfObject(targetKey);
        
        switch (keyType)
        {
            case DictKeyValue.Enum:
                var maxEnumIndex = 0;
                for (int i = 0; i < property.arraySize; i++)
                {
                    var e = property.GetArrayElementAtIndex(i);
                    var keyIntValue = e.FindPropertyRelative("_key");
                    maxEnumIndex = Math.Max(maxEnumIndex, keyIntValue.enumValueIndex);
                }
                elementKey.enumValueIndex = maxEnumIndex + 1;
                break;
            case DictKeyValue.Integer:
                var maxIntValue = int.MinValue;
                for (int i = 0; i < property.arraySize; i++)
                {
                    var e = property.GetArrayElementAtIndex(i);
                    var keyIntValue = e.FindPropertyRelative("_key");
                    maxIntValue = Math.Max(maxIntValue, keyIntValue.intValue);
                }

                elementKey.intValue = maxIntValue + 1;
                break;
            case DictKeyValue.Long:
                var maxLongValue = long.MinValue;
                for (int i = 0; i < property.arraySize; i++)
                {
                    var e = property.GetArrayElementAtIndex(i);
                    var keyIntValue = e.FindPropertyRelative("_key");
                    maxLongValue = Math.Max(maxLongValue, keyIntValue.longValue);
                }

                elementKey.longValue = maxLongValue + 1;
                break;
            case DictKeyValue.Float:
                var maxFloatValue = float.MinValue;
                for (int i = 0; i < property.arraySize; i++)
                {
                    var e = property.GetArrayElementAtIndex(i);
                    var keyIntValue = e.FindPropertyRelative("_key");
                    maxFloatValue = Mathf.Max(maxFloatValue, keyIntValue.floatValue);
                }

                elementKey.floatValue = maxFloatValue + 1;
                break;
            case DictKeyValue.Double:
                var maxDoubleValue = double.MinValue;
                for (int i = 0; i < property.arraySize; i++)
                {
                    var e = property.GetArrayElementAtIndex(i);
                    var keyIntValue = e.FindPropertyRelative("_key");
                    maxDoubleValue = Math.Max(maxDoubleValue, keyIntValue.doubleValue);
                }

                elementKey.doubleValue = maxDoubleValue + 1;
                break;
            case DictKeyValue.String:
                var startKey = "key";
                for (int i = 0; i < property.arraySize; i++)
                {
                    var e = property.GetArrayElementAtIndex(i);
                    var keyStrValue = e.FindPropertyRelative("_key");
                    if (keyStrValue.stringValue == startKey)
                    {
                        startKey += "_";
                    }
                }

                elementKey.stringValue = startKey;
                break;
            default:
                throw new NotImplementedException();
        }

        property.serializedObject.ApplyModifiedProperties();
    }

    private enum DictKeyValue
    {
        Undef,
        Integer,
        Enum,
        Long,
        Float,
        Double,
        String,
        RefObject
    }

    public static ReorderableList NewReorderableList(SerializedObject serializedObject, SerializedProperty property)
    {
        var reorder = new ReorderableList(serializedObject, property);
        reorder.onAddCallback = list =>
        {
            AddKeyEmptyValuePair(list);
        };
        reorder.elementHeight = 20.0f * 2;
        reorder.drawElementCallback = (rect, index, active, focused) =>
        {
            var pairProperty = property.GetArrayElementAtIndex(index);

            var keyProperty = pairProperty.FindPropertyRelative("_key");
            var valueProperty = pairProperty.FindPropertyRelative("_value");

            rect.size = new Vector2(rect.size.x, 20.0f);
            EditorGUI.PropertyField(rect, keyProperty);
            rect.position += new Vector2(0.0f, rect.size.y);
            EditorGUI.PropertyField(rect, valueProperty);
        };
        return reorder;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty myDataList = property.FindPropertyRelative("_pairs");
        if (!_reorderableLists.ContainsKey(property.propertyPath) || _reorderableLists[property.propertyPath].index >
            _reorderableLists[property.propertyPath].count - 1)
        {
            _reorderableLists[property.propertyPath] = NewReorderableList(myDataList.serializedObject, myDataList);
        }

        return _reorderableLists[property.propertyPath].GetHeight();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.IndentedRect(position);
        var list = _reorderableLists[property.propertyPath];
        list.DoList(position);

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(SerializableDictionary<,>.SerializableKeyValuePair))]
public class UnityDictionaryInternalDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 40.0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var keyProperty = property.FindPropertyRelative("_key");
        var valueProperty = property.FindPropertyRelative("_value");

        position.size = new Vector2(position.size.x, 20.0f);
        EditorGUI.PropertyField(position, keyProperty);
        position.position += new Vector2(0.0f, position.size.y);
        EditorGUI.PropertyField(position, valueProperty);

        EditorGUI.EndProperty();
    }
}