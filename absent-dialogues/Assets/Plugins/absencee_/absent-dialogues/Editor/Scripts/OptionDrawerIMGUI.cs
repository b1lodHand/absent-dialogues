//using com.absence.dialoguesystem.internals;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace com.absence.dialoguesystem.editor
//{
//    [CustomPropertyDrawer(typeof(Option))]
//    public class OptionDrawerIMGUI : PropertyDrawer
//    {
//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            return base.GetPropertyHeight(property, label);
//        }

//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            EditorGUI.BeginProperty(position, label, property);

//            var labelWidth = 50f;
//            var horizontalSpace = 10f;

//            var dynamicRect = new Rect(position.x, position.y, labelWidth, position.height);
//            EditorGUI.LabelField(dynamicRect, label);

//            dynamicRect.width = position.width - (labelWidth + horizontalSpace);
//            dynamicRect.x += labelWidth;
//            dynamicRect.x += horizontalSpace;

//            EditorGUI.BeginChangeCheck();
//            Undo.RecordObject(property.serializedObject.targetObject, "Option (Audio Clip)");

//            EditorGUI.PropertyField(dynamicRect, property.FindPropertyRelative("AudioClip"), GUIContent.none);

//            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(property.serializedObject.targetObject);
//            EditorGUI.EndProperty();
//        }
//    }
//}