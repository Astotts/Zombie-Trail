using System;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CreateAssetMenu(menuName = "Stats/WaveManager", fileName = "New Wave Manager Stats")]
public class WaveManagerStats : ScriptableObject
{
    [field: SerializeField] public DisplayTime StartTime { get; private set; }
    [field: SerializeField] public int SecondsPerNight { get; private set; }
    [field: SerializeField] public int SecondsPerDay { get; private set; }
    [field: SerializeField] public string SkipKey { get; private set; }
    [field: SerializeField] public string DayTitle { get; private set; }
    [field: SerializeField] public string DaySubtitle { get; private set; }
}

[Serializable]
public class DisplayTime
{
    public int Hour => hour;
    public int Minute => minute;
    [SerializeField] int hour;
    [SerializeField] int minute;
}

[CustomPropertyDrawer(typeof(DisplayTime))]
public class IngredientDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var amountRect = new Rect(position.x, position.y, 25, position.height);
        var unitRect = new Rect(position.x + 30, position.y, 25, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("hour"), GUIContent.none);
        EditorGUI.PropertyField(unitRect, property.FindPropertyRelativeOrFail("minute"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}