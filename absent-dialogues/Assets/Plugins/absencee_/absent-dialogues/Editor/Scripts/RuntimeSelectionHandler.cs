using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor
{
    [InitializeOnLoad]
    public static class RuntimeSelectionHandler
    {
        static RuntimeSelectionHandler()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    DialogueEditorWindow.OnGUIDelayCall += OnSelectionChanged;
                    break;
            }
        }

        private static void OnSelectionChanged()
        {
            GameObject gameObject = Selection.activeGameObject;

            if (!Selection.activeGameObject) return;
            if (!gameObject.TryGetComponent(out DialogueInstance instance)) return;
            if (!instance.ReferencedDialogue) return;

            if (Application.isPlaying)
            {
                DialogueEditorWindow.PopulateDialogueView(instance.Player.ClonedDialogue);
            }

            else
            {
                DialogueEditorWindow.PopulateDialogueView(instance.ReferencedDialogue);
                DialogueEditorWindow.SaveLastDialogue();
            }
        }
    }
}
