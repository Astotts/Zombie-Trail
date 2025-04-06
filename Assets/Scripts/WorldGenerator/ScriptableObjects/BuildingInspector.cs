using Unity.Properties;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(WeightedBuilding))]
public class BuildingInspector : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        SerializedProperty prefabProperty = property.FindPropertyRelative("buildingSO");
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        if (prefabProperty.objectReferenceValue != null)
            label.text = prefabProperty.objectReferenceValue.name;
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var weightRect = new Rect(position.x, position.y, position.width / 4 - 5, position.height);
        var prefabRect = new Rect(position.x + position.width / 4, position.y, position.width / 4 * 3, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(weightRect, property.FindPropertyRelative("weight"), GUIContent.none);
        EditorGUI.PropertyField(prefabRect, prefabProperty, GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}