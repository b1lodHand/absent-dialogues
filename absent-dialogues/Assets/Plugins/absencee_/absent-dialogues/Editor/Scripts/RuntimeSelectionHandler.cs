using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor
{
    /// <summary>
    /// It handles the selection events of <see cref="IUseDialogueInScene"/> game objects.
    /// </summary>
    [InitializeOnLoad]
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.editor.RuntimeSelectionHandler.html")]
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
            if (!gameObject.TryGetComponent(out IUseDialogueInScene displayer)) return;
            if (displayer.ReferencedDialogue == null) return;

            if (Application.isPlaying)
            {
                DialogueEditorWindow.PopulateDialogueView(displayer.ClonedDialogue);
            }

            else
            {
                DialogueEditorWindow.PopulateDialogueView(displayer.ReferencedDialogue);
                DialogueEditorWindow.SaveLastDialogue();
            }
        }
    }
}
