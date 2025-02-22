//using com.absence.dialoguesystem.internals;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace com.absence.dialoguesystem.editor
//{
//    [CustomPropertyDrawer(typeof(List<Option>), true)]
//    public class OptionListPropertyDrawer : PropertyDrawer
//    {
//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            return EditorGUI.GetPropertyHeight(property, label, true);
//        }

//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            SerializedProperty iterator = property;
//            bool enterChildren = true;
//            while (iterator.NextVisible(enterChildren))
//            {
//                continue;

//                GUIContent content = new GUIContent()
//                {
//                    text = iterator.displayName,
//                    tooltip = iterator.tooltip,
//                };

//                EditorGUI.PropertyField(position, iterator, content, true);

//                enterChildren = false;
//            }
//        }
//    }
//}
