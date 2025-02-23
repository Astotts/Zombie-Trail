
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SpriteGrid))]
public class Sprite2DInspector : PropertyDrawer
{
    static float previewWidth;
    static float previewHeight;
    static float previewSpace;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int rowNum = property.FindPropertyRelative("RowNum").intValue;
        
        return math.max(rowNum * 20, previewHeight + 20) + 20;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect previewRect = new(position.x, position.y, position.width / 3, position.height);
        DrawPreviewOptions(
            ref previewRect,
            property.FindPropertyRelative("RowNum"),
            property.FindPropertyRelative("ColNum"),
            property.FindPropertyRelative("Rows"));

        Rect gridRect = new(position.width / 3 + 20, position.y, position.width / 3 * 2, position.height);
        DrawGridInfo(
            ref gridRect,
            property.FindPropertyRelative("RowNum"),
            property.FindPropertyRelative("ColNum"),
            property.FindPropertyRelative("Rows")
        );

        EditorGUI.EndProperty();
    }

    private void DrawGridInfo(ref Rect position, SerializedProperty rowNum, SerializedProperty colNum, SerializedProperty gridProperty)
    {
        Rect labelRect = new(position.x, position.y, position.width, 18);
        EditorGUI.LabelField(labelRect, "Grid");

        GUIStyle numberStyle = EditorStyles.numberField;
        numberStyle.alignment = TextAnchor.MiddleCenter;

        float colFieldWidth = colNum.intValue.ToString().Length * 7.5f + 10f;
        float rowFieldWidth = rowNum.intValue.ToString().Length * 7.5f + 10f;

        Rect colFieldRect = new(position.x + position.width - colFieldWidth - 2 - rowFieldWidth - 5, position.y, colFieldWidth, 18);
        Rect rowFieldRect = new(position.x + position.width - rowFieldWidth - 2, position.y, rowFieldWidth, 18);

        colNum.intValue = EditorGUI.IntField(colFieldRect, colNum.intValue);
        rowNum.intValue = EditorGUI.IntField(rowFieldRect, rowNum.intValue);
        position.y += 20;   

        float spriteFieldWidth = (position.width - 20 - 5 * colNum.intValue) / colNum.intValue;
        Rect spriteFieldRect = new(
            position.x,
            position.y,
            spriteFieldWidth,
            18);
        
        gridProperty.arraySize = rowNum.intValue;

        for (int row = 0; row < rowNum.intValue; row++)
        {
            SerializedProperty spriteColumnProperty = gridProperty.GetArrayElementAtIndex(row);
            SerializedProperty spriteColumnArray = spriteColumnProperty.FindPropertyRelative("Columns");

            spriteColumnArray.arraySize = colNum.intValue;
            
            for (int col = 0; col < colNum.intValue; col++)
            {
                EditorGUI.PropertyField(spriteFieldRect, spriteColumnArray.GetArrayElementAtIndex(col), GUIContent.none);
                spriteFieldRect.x += spriteFieldWidth + 5;
            }
            spriteFieldRect.x -= (spriteFieldWidth + 5) * colNum.intValue;
            spriteFieldRect.y += 20;
        }

        position.y += 20 * rowNum.intValue;

        float colButtonHeight = rowNum.intValue * 20 / 2;
        float rowButtonWidth = (position.width - 20) / 2;

        Rect colIncreaseRect = new(position.x + position.width - 20, position.y - colButtonHeight * 2, 18, colButtonHeight);
        Rect colDecreaseRect = new(position.x + position.width - 20, position.y - colButtonHeight, 18, colButtonHeight);

        Rect rowIncreaseRect = new(position.x, spriteFieldRect.y, rowButtonWidth, 18);
        Rect rowDecreaseRect = new(position.x + rowButtonWidth, spriteFieldRect.y, rowButtonWidth, 18);

        GUIStyle fontStyle = EditorStyles.miniButton;
        fontStyle.fontSize += 5;
        fontStyle.fontStyle = FontStyle.Bold;
        float prevHeight = fontStyle.fixedHeight;
        fontStyle.fixedHeight = colButtonHeight;
        if (GUI.Button(colIncreaseRect, "+", fontStyle))
            colNum.intValue++;

        if (GUI.Button(colDecreaseRect, "-", fontStyle))
            colNum.intValue--;

        fontStyle.fixedHeight = prevHeight;

        if (GUI.Button(rowIncreaseRect, "+", fontStyle))
            rowNum.intValue++;

        if (GUI.Button(rowDecreaseRect, "-", fontStyle))
            rowNum.intValue--;
        
        gridProperty.arraySize = rowNum.intValue;

        for (int row = 0; row < rowNum.intValue; row++)
        {
            SerializedProperty spriteColumnProperty = gridProperty.GetArrayElementAtIndex(row);
            SerializedProperty spriteColumnArray = spriteColumnProperty.FindPropertyRelative("Columns");

            spriteColumnArray.arraySize = colNum.intValue;
        }

        fontStyle.fontStyle = FontStyle.Normal;
        fontStyle.fontSize -= 5;
        
        position.y += 20;
    }

    private void DrawPreviewOptions(ref Rect position, SerializedProperty rowNum, SerializedProperty colNum, SerializedProperty gridProperty)
    {
        Rect backGroundRect = new(position.x, position.y, position.width, position.height);
        EditorGUI.DrawRect(backGroundRect, new Color(0.2f, 0.2f, 0.2f));

        Rect labelRect = new(position.x, position.y, "Preview".Length * 8.5f, 18);
        EditorGUI.LabelField(labelRect, "Preview");

        GUIStyle fontStyle = EditorStyles.numberField;
        fontStyle.alignment = TextAnchor.MiddleCenter;

        float widthFieldWidth = previewWidth.ToString().Length * 7.5f + 10f;
        float heightFieldWidth = previewHeight.ToString().Length * 7.5f + 10f;
        float spaceFieldWidth = previewSpace.ToString().Length * 7.5f + 10f;

        Rect widthFieldRect = new(position.width - widthFieldWidth + 18 - spaceFieldWidth - 5 - heightFieldWidth - 5, position.y, widthFieldWidth, 18);
        Rect heightFieldRect = new(position.width - heightFieldWidth + 18 - spaceFieldWidth - 5, position.y, heightFieldWidth, 18);
        Rect spaceFieldRect = new(position.width - spaceFieldWidth + 18, position.y, spaceFieldWidth, 18);

        previewWidth = EditorGUI.FloatField(widthFieldRect, previewWidth);
        previewHeight = EditorGUI.FloatField(heightFieldRect, previewHeight);
        previewSpace = EditorGUI.FloatField(spaceFieldRect, previewSpace);

        position.y += 20;
        position.y += 8;

        float spriteWidth = (previewWidth - (colNum.intValue - 1) * previewSpace) / colNum.intValue;
        float spriteHeight = (previewHeight - (rowNum.intValue - 1) * previewSpace) / rowNum.intValue;

        Rect spriteRect = new(position.x + 18, position.y, spriteWidth, spriteHeight);

        for (int row = 0; row < rowNum.intValue; row++)
        {
            SerializedProperty spriteColumnProperty = gridProperty.GetArrayElementAtIndex(row);
            SerializedProperty spriteColumnArray = spriteColumnProperty.FindPropertyRelative("Columns");

            spriteColumnArray.arraySize = colNum.intValue;
            
            for (int col = 0; col < colNum.intValue; col++)
            {
                SerializedProperty spriteProperty = spriteColumnArray.GetArrayElementAtIndex(col);
                var sprite = spriteProperty.objectReferenceValue as Sprite;
                DrawTexturePreview(spriteRect, sprite);
                spriteRect.x += spriteWidth + previewSpace;
            }
            spriteRect.x -= (spriteWidth + previewSpace) * colNum.intValue;
            spriteRect.y += spriteHeight + previewSpace;
        }

        position.y += spriteHeight;
    }

    private void DrawTexturePreview(Rect position, Sprite sprite)
    {
        Vector2 fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
        Vector2 size = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

        Rect coords = sprite.textureRect;
        coords.x /= fullSize.x;
        coords.width /= fullSize.x;
        coords.y /= fullSize.y;
        coords.height /= fullSize.y;

        Vector2 ratio;
        ratio.x = position.width / size.x;
        ratio.y = position.height / size.y;
        float minRatio = Mathf.Min(ratio.x, ratio.y);

        Vector2 center = position.center;
        position.width = size.x * minRatio;
        position.height = size.y * minRatio;
        position.center = center;

        GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
    }
}