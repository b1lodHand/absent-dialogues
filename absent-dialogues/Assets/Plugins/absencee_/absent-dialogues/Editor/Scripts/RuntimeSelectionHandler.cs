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
        }

        private static void OnSelectionChanged()
        {
            if (!Application.isPlaying) return;

            GameObject gameObject = Selection.activeGameObject;

            if (!Selection.activeGameObject) return;
            if (!gameObject.TryGetComponent(out DialogueInstance instance)) return;
            if (!instance.ReferencedDialogue) return;

            DialogueEditorWindow.PopulateDialogueView(instance.Player.ClonedDialogue);
        }
    }
}
