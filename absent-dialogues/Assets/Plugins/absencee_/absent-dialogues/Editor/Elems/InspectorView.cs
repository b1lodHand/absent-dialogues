using com.absence.attributes.editor;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor
{
    /// <summary>
    /// A visual element subtype which is responsible for rendering a node's inspector properties when selected.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.editor.InspectorView.html")]
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        internal NodeView m_currentNode;
        Editor editor = null;

        public event Action OnNodeValidation = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InspectorView()
        {
            
        }

        internal void UpdateSelection(NodeView nodeView)
        {
            Clear();

            m_currentNode = nodeView;

            if (nodeView == null) return; 

            Editor.CreateCachedEditor(nodeView.Node, null, ref editor);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor.target == null) return;

                if (Application.isPlaying) GUI.enabled = false;

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);

                editor.OnInspectorGUI();

                EditorGUILayout.EndVertical();

                if (EditorGUI.EndChangeCheck()) OnNodeValidation?.Invoke();

                if (Application.isPlaying) GUI.enabled = true;
            });
            Add(container);
        }
    }

}