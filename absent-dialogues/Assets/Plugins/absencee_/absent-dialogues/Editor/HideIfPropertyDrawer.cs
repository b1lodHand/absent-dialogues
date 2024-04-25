using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HideIfAttribute))]
public class HideIfPropertyDrawer : PropertyDrawer
{
    #region Fields

    // Reference to the attribute on the property.
    HideIfAttribute hideIf;

    // Field that is being compared.
    SerializedProperty comparedField;

    #endregion

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!Check(property))
            return 0f;

        return base.GetPropertyHeight(property, label);
    }

    private bool Check(SerializedProperty property)
    {
        hideIf = attribute as HideIfAttribute;

        string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, hideIf.PropertyName) : 
            hideIf.PropertyName;

        comparedField = property.serializedObject.FindProperty(path);

        if (comparedField == null)
        {
            Debug.LogError($"Cannot find property with name: {path}");
            return true;
        }

        if(hideIf.DirectBool) return !comparedField.boolValue;

        switch (comparedField.propertyType)
        {
            case SerializedPropertyType.Boolean:
                return comparedField.boolValue != (bool)hideIf.TargetValue;

            case SerializedPropertyType.Enum:
                return !(comparedField.enumValueIndex.Equals((int)hideIf.TargetValue));

            case SerializedPropertyType.Integer:
                return comparedField.intValue != (int)hideIf.TargetValue;

            case SerializedPropertyType.Float:
                return comparedField.floatValue != (float)hideIf.TargetValue;

            case SerializedPropertyType.String:
                return comparedField.stringValue != (string)hideIf.TargetValue;

            case SerializedPropertyType.ObjectReference:
                return !comparedField.objectReferenceValue.Equals(hideIf.TargetValue);

            case SerializedPropertyType.ExposedReference:
                return !comparedField.objectReferenceValue.Equals(hideIf.TargetValue);

            case SerializedPropertyType.ManagedReference:
                return !comparedField.managedReferenceValue.Equals(hideIf.TargetValue);

            default:
                Debug.LogError($"This type is not supported: {comparedField.type}");
                return false;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (Check(property))
        {
            EditorGUI.PropertyField(position, property);
        } 
    }

}