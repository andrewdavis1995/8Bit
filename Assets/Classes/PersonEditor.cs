using Assets.Classes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Person))]
public class PersonEditor : PropertyDrawer
{

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // The 6 comes from extra spacing between the fields (2px each)
        return EditorGUIUtility.singleLineHeight * (11 + 2);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.LabelField(position, label);

        var nameRect = new Rect(position.x, position.y + 18, position.width, 16);
        var ageRect = new Rect(position.x, position.y + 36, position.width, 16);
        var genderRect = new Rect(position.x, position.y + 54, position.width, 16);
        var romanticRect = new Rect(position.x, position.y + 72, position.width, 16);
        var characterTypeRect = new Rect(position.x, position.y + 90, position.width, 16);

        var angerRect = new Rect(position.x, position.y + 120, position.width, 16);
        var loveRect = new Rect(position.x, position.y + 138, position.width, 16);
        var chattinessRect = new Rect(position.x, position.y + 156, position.width, 16);
        var friendlyRect = new Rect(position.x, position.y + 174, position.width, 16);
        var healthRect = new Rect(position.x, position.y + 192, position.width, 16);

        EditorGUI.indentLevel++;

        // basic info
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("Name"));
        EditorGUI.PropertyField(ageRect, property.FindPropertyRelative("Age"));
        EditorGUI.PropertyField(genderRect, property.FindPropertyRelative("Gender"));
        EditorGUI.PropertyField(romanticRect, property.FindPropertyRelative("RomanticallyAvailable"));
        EditorGUI.PropertyField(characterTypeRect, property.FindPropertyRelative("CharacterType"));

        // status
        EditorGUI.PropertyField(angerRect, property.FindPropertyRelative("Anger"));
        EditorGUI.PropertyField(loveRect, property.FindPropertyRelative("Love"));
        EditorGUI.PropertyField(chattinessRect, property.FindPropertyRelative("Chattiness"));
        EditorGUI.PropertyField(friendlyRect, property.FindPropertyRelative("Friendliness"));
        EditorGUI.PropertyField(healthRect, property.FindPropertyRelative("Health"));

    EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
}