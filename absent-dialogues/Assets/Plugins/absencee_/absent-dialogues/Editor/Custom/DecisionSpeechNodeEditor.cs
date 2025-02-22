//using com.absence.attributes.editor;
//using com.absence.dialoguesystem.internals;
//using UnityEditor;
//using UnityEngine;

//namespace com.absence.dialoguesystem.editor
//{
//    [CustomEditor(typeof(DecisionSpeechNode), true, isFallback = false)]
//    public class DecisionSpeechNodeEditor : Editor
//    {
//        Editor initialEditor;

//        private void OnEnable()
//        {
//            Editor.CreateCachedEditor(target, typeof(absentEditorExtension), ref initialEditor);
//        }

//        private void OnDisable()
//        {
//            Editor.DestroyImmediate(initialEditor);
//            initialEditor = null;
//        }

//        public override void OnInspectorGUI()
//        {
//            initialEditor.OnInspectorGUI();

//            SerializedProperty optionListProp = serializedObject.FindProperty("Options");

//            EditorGUILayout.PropertyField(optionListProp);

//            //const float kOffset = 2f;

//            //Rect lastRect = GUILayoutUtility.GetLastRect();
//            //float totalHeight = lastRect.height;

//            //lastRect.height = EditorGUIUtility.singleLineHeight + kOffset;
//            //lastRect.y += (totalHeight - lastRect.height) - kOffset;

//            //EditorGUI.DrawRect(lastRect, Color.red);
//        }
//    }

//}